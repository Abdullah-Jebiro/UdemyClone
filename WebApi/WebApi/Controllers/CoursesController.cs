using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DbEntities;
using Models.Dtos;
using Models.Exceptions;
using Services.File;
using Services.Repos;
using System.Net;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/Courses")]
    [ApiController]
  
    public class CoursesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFilesService _filesService;

        public CoursesController(IUnitOfWork unitOfWork, IMapper mapper , IFilesService filesService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _filesService = filesService;
        }

        /// <summary>
        /// Gets courses
        /// </summary>
        /// <param name="pageNumber">The page number of the results</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <param name="instructorId">Optional instructor ID to filter by</param>
        /// <param name="categoryId">Optional category ID to filter by</param>
        /// <param name="languageId">Optional language ID to filter by</param>
        /// <param name="levelId">Optional level ID to filter by</param>
        /// <param name="name">Optional course name to filter by</param>
        /// <param name="maxPrice">Optional maximum price to filter by</param>
        /// <param name="minPrice">Optional minimum price to filter by</param>
        /// <param name="onlyUserCourses">Whether to return only the courses belonging to the user</param>
        /// <returns>A list of courses with their status for the user</returns>
        [HttpGet]
        public async Task<ActionResult> GetCourse(         
            int? instructorId,
            int? categoryId,
            int? languageId,
            int? levelId,
            string? name,
            double? maxPrice,
            double minPrice = 0,
            int pageNumber = 1,
            int pageSize = 10,
            bool onlyUserCourses = false
        )
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            if ( userId == 0 )
            {
                return Unauthorized();
            }

            // Call the course service to retrieve the courses with their status for the user
            var (Courses, paginationData) = await _unitOfWork.Courses.GetCoursesWithStatusAsync(
                userId,
                pageNumber,
                pageSize,
                categoryId,
                instructorId,
                languageId,
                levelId,
                name,
                maxPrice,
                minPrice,
                onlyUserCourses
            );

            if (Courses == null)
                return NotFound("No courses found.");


            Response.Headers.Add("x-pagination", paginationData.ToString());

            return Ok(Courses);
        }



        /// <summary>
        /// Gets the courses associated with the current user
        /// </summary>
        /// <returns>A list of courses associated with the current user</returns>
        [HttpGet("Users")]
        public async Task<ActionResult> GetUserCourses()
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            // Call the service to get the courses associated with the current user
            var courses = await _unitOfWork.Courses.GetUserCourses(userId);


            if (courses == null)
                return NotFound($"No courses found for User with ID {userId}.");



            // Map the courses to the DTO format and return them
            return Ok(_mapper.Map<IReadOnlyList<CourseDto>>(courses));
        }



        /// <summary>
        /// Gets the courses associated with the current instructor
        /// </summary>
        /// <returns>A list of courses associated with the current instructor</returns>
        [HttpGet("Instructor")]
        public async Task<ActionResult> GetInstructorCourses(int pageNumber=1, int pageSize=10)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var (courses, paginationData) = await _unitOfWork.Courses.GetInstructorCourses(userId, pageNumber,pageSize);
            if (courses == null)
                return NotFound("No courses found for instructor with ID {instructorId}.");
            Response.Headers.Add("x-pagination", paginationData.ToString());

            return Ok(_mapper.Map<IReadOnlyList<CourseForInstructorDto>>(courses));
        }




        /// <summary>
        /// Gets a course associated with the current user for updating
        /// </summary>
        /// <param name="courseId">The ID of the course to retrieve</param>
        /// <returns>The course associated with the current user in a format suitable for updating</returns>
        [HttpGet("Instructor/{courseId}")]
        public async Task<ActionResult> GetCourseForUpdate(int courseId)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());

            // Call the service to find the course with the specified ID and associated with the current user
            var course = await _unitOfWork.Courses.FindAsync(
                c => c.UserId == userId && c.CourseId == courseId);

            if (course == null)
                return NotFound($"The course with ID {courseId} was not found.");

            return Ok(_mapper.Map<CourseForCreateDto>(course));
        }



        /// <summary>
        /// Retrieves a course by its ID.
        /// </summary>
        /// <param name="courseId">The ID of the course to retrieve.</param>
        /// <returns>The course with the specified ID.</returns>
        [HttpGet("{courseId}")]
        public async Task<ActionResult> GetCourse(int courseId)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var course = await _unitOfWork.Courses.GetCourse(userId, courseId);
            if (course == null)
                return NotFound($"The courses with ID {courseId} was not found.");

            return Ok(course);
        }



        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="dto">The DTO containing the course information.</param>
        /// <returns>The newly created course.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateCourse(CourseForCreateDto dto)
        {
            // Map the DTO to a Course object and set the user ID
            var course = _mapper.Map<Course>(dto);
            course.UserId = Convert.ToInt32(User.Identity.GetUserId());

            // Add the course to the repository and return a CreatedAtAction response with the course information
            course = await _unitOfWork.Courses.AddAsync(course);

            return CreatedAtAction(
                nameof(GetCourse),
                new { courseId = course.CourseId },
                _mapper.Map<CourseDto>(course)
            );
        }



        /// <summary>
        /// Updates a course with the specified ID for the current authenticated user.
        /// </summary>
        /// <param name="dto">The DTO object containing the updated course data.</param>
        /// <param name="courseId">The ID of the course to update.</param>
        /// <returns>Returns an IActionResult indicating the success or failure of the operation.</returns>
        [HttpPut("{courseId}")]
        public async Task<ActionResult> Update(CourseForUpdateDto dto, int courseId)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());
            var course = await _unitOfWork.Courses.FindAsync(
                c => c.UserId == userId && c.CourseId == courseId
            );

            if (course == null)
               return NotFound($"The course with ID {courseId} was not found.");

            if (course.ThumbnailUrl != dto.ThumbnailUrl)
            {
                await _filesService.DeleteAsync(course.ThumbnailUrl);
                course.ThumbnailUrl = dto.ThumbnailUrl;
            }

            _mapper.Map(dto, course);
            await _unitOfWork.Courses.UpdateAsync(course);
            return Ok();
        }



        /// <summary>
        /// Delete a specific course from the database and storage using its ID.
        /// </summary>
        /// <param name="courseId">The ID of the course to be deleted.</param>
        /// <returns>If the deletion is successful,NoContent is returned.       
        /// If the course is not found, NotFound is returned. 
        /// If the user is not authorized, Unauthorized is returned,   
        /// If the course is associated with one or more users, BadRequest is returned with a message indicating.
        /// Unable to delete the course as it has been purchased by one or more users. Only those who have purchased the course can access it. </returns>
        [HttpDelete("{courseId}")]
        public async Task<ActionResult> Delete(int courseId)
        {
            int userId = Convert.ToInt32(User.Identity.GetUserId());

            // Delete all related records in the cart table
            await _unitOfWork.Carts.DeleteRangeAsync(c => c.CourseId == courseId);

            // Delete the course with the specified ID
            await _unitOfWork.Courses.DeleteAsync(courseId, userId);

            return NoContent();
        }
    }
}