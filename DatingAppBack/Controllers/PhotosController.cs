using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingAppBack.Data;
using DatingAppBack.Dtos;
using DatingAppBack.Helpers;
using DatingAppBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingAppBack.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private Cloudinary cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig )
        {
            //Cloudinary set up
            this.repo = repo;
            this.mapper = mapper;
            this.cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                this.cloudinaryConfig.Value.CloudName,
                this.cloudinaryConfig.Value.ApiKey,
                this.cloudinaryConfig.Value.ApiSecret
            );

            this.cloudinary = new Cloudinary(acc);

        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await this.repo.GetPhoto(id);
            var photo = this.mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await this.repo.GetUser(userId);
            var file = photoForCreationDto.File;
            //we get back mth result from cloudinary
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {   //using 'cause we manipulate data streams over time
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = this.cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = this.mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await this.repo.SaveAll())
            {
                var photoToReturn = this.mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not upload photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await this.repo.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await this.repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("this is already the main photo");

            var currentMainPhoto = await this.repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;
            if (await this.repo.SaveAll())
                return Ok( new { status = "set to main"} );

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await this.repo.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await this.repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("can not delete your main photo");

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = this.cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                    this.repo.Delete(photoFromRepo);
            }
            if (photoFromRepo.PublicId == null)
            {
                this.repo.Delete(photoFromRepo);
            }
            if (await this.repo.SaveAll())
                return Ok();

            return BadRequest("failed to delete photo");
        }
    }
}
