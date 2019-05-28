using AutoMapper;
using DatingAppBack.Data;
using DatingAppBack.Dtos;
using DatingAppBack.Helpers;
using DatingAppBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingAppBack.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await this.repo.GetMessage(id);
            if (messageFromRepo == null)
                return NotFound();
            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            var messagesFromRepo = await this.repo.GetMessagesForUser(messageParams);

            var messagesToReturn = this.mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(new
            {
                currentPage = messagesFromRepo.CurrentPage,
                pageSize = messagesFromRepo.PageSize,
                totalCount = messagesFromRepo.TotalCount,
                totalPages = messagesFromRepo.TotalPages,
                messages = messagesToReturn
            });
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messagefromRepo = await this.repo.GetMessageThread(userId, recipientId);
            var messageThread = this.mapper.Map<IEnumerable<MessageToReturnDto>>(messagefromRepo);

            return Ok(messageThread);

        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            //fetch the sender for dto automapper magic
            var sender = await this.repo.GetUser(userId);
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;
            //fetch the sender for dto automapper magic
            var recipient = await this.repo.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null)
                return BadRequest("Could not find user.");
            var message = this.mapper.Map<Message>(messageForCreationDto);

            this.repo.Add(message);


            if (await this.repo.SaveAll())
            {
                // Automapper automatically includes senser and recipient properties to MessageToReturnDto
                // 'cause they are still in memory
                //we map here to get and return the real id from sql
                var messageToReturn = this.mapper.Map<MessageToReturnDto>(message); 

                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);
            }

            throw new Exception("Creating the message failed.");
        }
    }
}
