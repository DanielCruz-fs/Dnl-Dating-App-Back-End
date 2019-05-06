using Microsoft.AspNetCore.Mvc;
using DatingAppBack.Data;
using DatingAppBack.Dtos;
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
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto )
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await this.repo.UserExists(userForRegisterDto.Username))
                return BadRequest(new { message = $"User: {userForRegisterDto.Username} is already taken" });

            var userToCreate = new User { Username = userForRegisterDto.Username };

            var createdUser = await this.repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }
    }
}
