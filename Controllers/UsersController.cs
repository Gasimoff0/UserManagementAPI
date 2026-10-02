using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Models;

namespace UserManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Use a thread-safe ConcurrentDictionary for in-memory storage.
        private static readonly ConcurrentDictionary<Guid, User> _users = new();

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_users.Values);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            if (_users.TryGetValue(id, out var user))
            {
                return Ok(user);
            }

            return NotFound(new { Message = $"User with Id {id} not found." });
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Copilot debugging utilization: Resolved a potential issue where the client might send 
            // an all-zero Guid. We ensure a new Id is always generated, and CreatedAt is set server-side.
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;

            _users.TryAdd(user.Id, user);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(Guid id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_users.TryGetValue(id, out var existingUser))
            {
                return NotFound(new { Message = $"User with Id {id} not found." });
            }

            // Update fields
            existingUser.FullName = updatedUser.FullName;
            existingUser.Email = updatedUser.Email;
            existingUser.Age = updatedUser.Age;
            // Note: We don't update CreatedAt

            return Ok(existingUser);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            // Copilot debugging utilization: Used TryRemove to avoid KeyNotFoundException that 
            // could occur in multi-threaded scenarios if the key is removed between a ContainsKey check and Remove call.
            if (_users.TryRemove(id, out _))
            {
                return NoContent();
            }

            return NotFound(new { Message = $"User with Id {id} not found." });
        }
    }
}
