using Microsoft.AspNetCore.Mvc;
using DatingAppBack.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppBack.Models;

namespace DatingAppBack.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;

        public AuthController(IAuthRepository repo)
        {
            this.repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            username = username.ToLower();
            if (await this.repo.UserExists(username))
                return BadRequest(new { message = $"User: {username} is already taken" });
            var userToCreate = new User { Username = username };

            var createdUser = await this.repo.Register(userToCreate, password);

            return StatusCode(201);
        }
    }
}
