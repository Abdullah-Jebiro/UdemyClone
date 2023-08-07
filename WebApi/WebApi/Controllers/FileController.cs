using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Exceptions;
using Models.ResponseModels;
using Services.File;
using Services.Repos;
using System;
using System.IO;
using System.Net;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFilesService _filesService;
        private readonly IUnitOfWork _unitOfWork;

        public FilesController(IFilesService filesService, IUnitOfWork unitOfWork)
        {
            _filesService = filesService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Uploads an image file.
        /// </summary>
        /// <param name="file">The image file to upload.</param>
        /// <param name="directory">The image directory to upload.</param>
        /// <returns>A response indicating whether the upload was successful and the name of the image.</returns>
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file, string directory)
        {
            // Upload the image 
            string imageName = await _filesService.UploadImage(file, directory);
            // Return the name of the uploaded image in a BaseResponse object
            return Ok(new BaseResponse<string>(imageName, "Image uploaded successfully"));
        }

        /// <summary>
        /// Retrieves an image file by its name.
        /// </summary>
        /// <param name="imageName">The name of the image file to retrieve.</param>
        [AllowAnonymous]
        [HttpGet("Images")]
        public async Task<IActionResult> GetImage(string imageName)
        {
            // Retrieve the image using the files service
            var imageStream = await _filesService.GetFile(imageName);

            if (imageStream==null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "image not found ");

            }

            // Return the image as a JPEG file
            return File(imageStream, "image/jpeg");
        }

        /// <summary>
        /// API endpoint to upload a video file.
        /// </summary>
        /// <param name="file">The video file to upload.</param>
        /// <param name="directory">The video directory to upload.</param>
        /// <returns>A response indicating whether the upload was successful and the name of the video.</returns>
        [HttpPost("UploadVideo")]
        public async Task<IActionResult> UploadVideo(IFormFile file, string directory)
        {
            string videoName = await _filesService.UploadVideo(file, directory);
            return Ok(new BaseResponse<string>(videoName, "Successfully uploaded video."));
        }

        /// <summary>
        /// Retrieves a video file by name, provided that the user is authorized to view it.
        /// </summary>
        /// <remarks>
        /// This endpoint requires the user to be authenticated and authorized to view the requested video. 
        /// If the user is not authorized, the endpoint returns an Unauthorized status code (401).
        /// If the requested video does not exist, the endpoint returns a NotFound status code (404).
        /// </remarks>
        /// <param name="videoName">The name of the video file to retrieve.</param>
        /// <returns>The requested video file.</returns>
        [AllowAnonymous]
        [HttpGet("Videos")]
        public async Task<IActionResult> GetVideo(string videoName)
        {
            // Find the video by its name
            var video = await _unitOfWork.Videos.FindAsync(v => v.VideoUrl == videoName);
            if (video == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Video not found ");
            }

            //TODO
            #region Check if the user is authorized to view the video
            //Check if the user is authorized to view the video
            //int userId = Convert.ToInt32(User.Identity.GetUserId());           
            //bool isAuthorized = await _unitOfWork.UsersCourses
            //    .ExistsAsync(uc => uc.UserId == userId && uc.CourseId == video.CourseId) ||
            //    await _unitOfWork.Courses
            //    .ExistsAsync(c => c.CourseId == video.CourseId && c.UserId == userId);

            //if (!isAuthorized)
            //{
            //    return Unauthorized();
            //}

            // Retrieve the video file from the file service
            #endregion

            var videoFile = await _filesService.GetFile(videoName);
            if (videoFile == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Video not found ");
            }

            // Return the video file as a FileStreamResult with the appropriate MIME type and file name
            var stream = new MemoryStream(videoFile);
            return File(stream, "video/mp4");

        }
    }
}