using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementApi.Controllers // changed namespace
{
    [ApiController]
    [Route("api/users")]
    /**
        * Controllers below were first drafted with Copilot, then slightly edited.
        * Debugging with copilot helped switch over to the thread safe concurrent dictionary methods
        * Also added simpler validation using ModelState
    */
    public class UserController : ControllerBase
    {

        ConcurrentDictionary<int, User> users = UserData.Users;

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            if (!users.TryGetValue(id, out var user))
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (users.ContainsKey(user.Id))
            {
                return Conflict("User with this ID already exists.");
            }
            users[user.Id] = user;
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (!users.TryRemove(id, out _))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!users.TryGetValue(user.Id, out var existingUser))
            {
                return NotFound();
            }
            // TryUpdate ensures atomic update
            if (!users.TryUpdate(user.Id, user, existingUser))
            {
                return Conflict("User could not be updated due to a concurrent change.");
            }
            return Ok(user.Id);
        }

    }
}