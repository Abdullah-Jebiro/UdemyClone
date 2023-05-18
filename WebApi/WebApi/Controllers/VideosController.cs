using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Services.File;
using Services.Repos;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/Courses/{courseId}/Videos")]
    [ApiController]

    public class VideosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<VideosController> _logger;
        private readonly IFilesService _filesService;

        public VideosController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<VideosController> logger, IFilesService filesService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _filesService = filesService;
        }

        /// <summary>
        /// Retrieves all videos for a given course and returns them as a list of VideoDto objects.
        /// </summary>
        /// <param name="courseId">The ID of the course for which to retrieve the videos.</param>
        /// <returns>An action result containing a list of VideoDto objects for the given course.
        /// A VideoDto object represents a video and has the following properties:
        /// - VideoId: An integer that represents the unique identifier of the video.
        /// - VideoTitle: A string that represents the title of the video.
        /// - VideoUrl: A string that represents the URL of the video.
        /// </returns>
        [HttpGet(Name = "GetVideos")]
        public async Task<ActionResult> GetVideos(int courseId)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            // Checks if the user is authorized to access the videos for the given course.
            bool isAuthorized = await _unitOfWork.UsersCourses.ExistsAsync(uc => uc.UserId == userId && uc.CourseId == courseId)
                || await _unitOfWork.Courses.ExistsAsync(c => c.CourseId == courseId && c.UserId == userId);

            if (!isAuthorized)
            {
                return BadRequest();
            }

            var videos = await _unitOfWork.Videos.FindAllAsync(v => v.CourseId == courseId, null, v => v.VideoTitle);

            if (videos == null)
            {
                return NotFound();
            }

            var videosDto = _mapper.Map<IReadOnlyList<VideoDto>>(videos);
            return Ok(videosDto);
        }

        /// <summary>
        /// Retrieves information for all videos for a given course and returns them as a list of VideoInfoDto objects.
        /// </summary>
        /// <param name="courseId">The ID of the course for which to retrieve video information.</param>
        /// <returns>An action result containing a list of VideoInfoDto (only the video ID and title properties) objects for the given course.</returns>
        [HttpGet("InfoVideos")]
        public async Task<ActionResult> GetInfoVideos(int courseId)
        {
            var videos = await _unitOfWork.Videos.FindAllAsync(
                v => v.CourseId == courseId,
                null,
                v => v.VideoTitle);

            if (videos.Count() == 0)
            {
                return NotFound();
            }

            var videoInfoDtos = _mapper.Map<IReadOnlyList<VideoInfoDto>>(videos);
            return Ok(videoInfoDtos);
        }

        /// <summary>
        /// Get the video for update by ID and course ID.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>The video for update.</returns>
        [HttpGet("{videoId}")]
        public async Task<ActionResult> GetVideoForUpdate(int courseId, int videoId)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            bool ISAuth = await _unitOfWork.UsersCourses.ExistsAsync(uc => uc.UserId == userId && uc.CourseId == courseId)
                || await _unitOfWork.Courses.ExistsAsync(c => c.CourseId == courseId && c.UserId == userId);
            if (!ISAuth)
            {
                return BadRequest();        
            }

            var video = await _unitOfWork.Videos.FindAsync(videos => videos.VideoId == videoId);
            if (video == null)
                return BadRequest();

            return Ok(_mapper.Map<VideoForUpdateDto>(video));

        }

        /// <summary>
        /// Creates a new video for a given course.
        /// </summary>
        /// <param name="dto">A VideoForCreateDto object that contains the necessary information to create a new video.</param>
        /// <returns>An action result containing the newly created video as a VideoDto { 
        /// VideoId: An integer that represents the unique identifier of the video.
        /// VideoTitle: A string that represents the title of the video.
        /// VideoUrl: A string that represents the URL of the video } object.</returns>
        [HttpPost]
        public async Task<ActionResult> Create(VideoForCreateDto dto)
        {
            bool IsCourse = await _unitOfWork.Courses.ExistsAsync(c => c.CourseId == dto.CourseId);
            if (!IsCourse) { return BadRequest(); }

            var video = await _unitOfWork.Videos.AddAsync(_mapper.Map<Video>(dto));

            return CreatedAtAction("GetVideos", new
            {
                courseId = video.CourseId
            }, _mapper.Map<VideoDto>(video));
        }

        /// <summary>
        /// Updates the title and URL of a video for a given course.
        /// </summary>
        /// <param name="dto">A VideoForUpdateDto object that contains the new title and URL for the video.</param>
        /// <param name="courseId">The ID of the course that the video belongs to.</param>
        /// <param name="videoId">The ID of the video to update.</param>
        /// <returns>An action result indicating the success or failure of the update operation.</returns>
        [HttpPut("{videoId}")]
        public async Task<ActionResult> Update(VideoForUpdateDto dto, int courseId, int videoId)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var course = await _unitOfWork.Courses.FindAsync(c => c.UserId == userId && c.CourseId == courseId);
            if (course == null)
                return BadRequest();

            var video = await _unitOfWork.Videos.FindAsync(v => v.VideoId == videoId);
            if (video == null)
                return BadRequest();

            video.VideoTitle = dto.VideoTitle;

            // Delete the associated video file from storage      
            if (video.VideoUrl != dto.VideoUrl)
            {
                await _filesService.DeleteAsync(video.VideoUrl);
                video.VideoUrl = dto.VideoUrl;
            }  
            await _unitOfWork.Videos.UpdateAsync(video);
            
            return Ok();
        }


        /// <summary>
        /// Deletes a video with the specified ID from the database and the associated file from storage.
        /// </summary>
        /// <param name="videoId">The ID of the video to delete.</param>
        /// <returns>An action result indicating whether the deletion was successful.</returns>
        [HttpDelete("{videoId}")]
        public async Task<ActionResult> Delete(int videoId)
        {
            var video = await _unitOfWork.Videos.FindAsync(v => v.VideoId == videoId);
            if (video == null)
            {
                return NotFound();
            }

            int userId = Convert.ToInt32(User.Identity.GetUserId());
            bool isAuthorized = await _unitOfWork.Courses.ExistsAsync(c => c.CourseId == video.CourseId && c.UserId == userId);

            if (!isAuthorized)
            {
                return Unauthorized();
            }

            // Delete the video from the database
            await _unitOfWork.Videos.DeleteAsync(video);

            // Delete the associated video file from storage
            await _filesService.DeleteAsync(video.VideoUrl);
           
            return NoContent();
        }
    }
}
     