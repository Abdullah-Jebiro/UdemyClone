using Data;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Models.DbEntities;
using Models.Dtos;
using Models.Dtos.Pagination;
using Models.Enums;
using Models.Exceptions;
using Services.File;
using Services.Repos;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Services.CoursesRepo
{
    public class CoursesRepository : BaseRepository<Course>, ICoursesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFilesService _filesService;

        public CoursesRepository(ApplicationDbContext context, IFilesService filesService) : base(context)
        {
            _context = context;
            _filesService = filesService;

        }


        private async Task<string> GetCourseStatusAsync(int userId, int courseId)
        {
            var isBought = await _context.UserCourses.AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
            if (isBought)
            {
                return CourseStatus.Bought.ToString();
            }

            var isInCart = await _context.Cart.AnyAsync(cart => cart.UserId == userId && cart.CourseId == courseId);
            if (isInCart)
            {
                return CourseStatus.InCart.ToString();
            }

            // default state is "NoAction"
            return CourseStatus.NoAction.ToString();
        }

        public async Task<CoursesWithDetailDto> GetCourse(int userId, int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Language)
                .Include(c => c.Level)
                .Include(c => c.User)
                .SingleOrDefaultAsync(c => c.CourseId == courseId && !c.IsDelete);
         

            var result = new CoursesWithDetailDto
            {
                CourseId = course.CourseId,
                About = course.About,
                ThumbnailUrl = course.ThumbnailUrl,
                Description = course.Description,
                Price = course.Price,
                Name = course.Name,
                Language = course.Language.Name,
                Level = course.Level.Name,
                InstructorId = course.User.Id,
                Instructor = course.User.UserName!,
                ProfilePictureUrl = course.User.ProfilePictureUrl,
                StudentsCount = await _context.UserCourses.CountAsync(uc => uc.CourseId == courseId),
                VideosCount = await _context.Videos.CountAsync(v => v.CourseId == courseId),
                CoursesCountForInstructor = await _context.Courses.CountAsync(c => c.UserId == course.UserId),
                StudentsCountForInstructor = await _context.UserCourses.CountAsync(uc => uc.Course.UserId == course.UserId),
                Status = await GetCourseStatusAsync(userId, courseId),
            };

            return result;
        }


        public async Task<(IEnumerable<CoursesWithStatusDto>, PaginationMetaData)> GetCoursesWithStatusAsync(
        int userId,
        int pageNumber,
        int pageSize,
        int? categoryId,
        int? instructorId,
        int? languageId,
        int? levelId,
        string? name,
        double? maxPrice,
        double minPrice,
        bool onlyUserCourses)
        {


            var coursesQuery = _context.Courses as IQueryable<Course>;


            if (!onlyUserCourses)
            {
                coursesQuery= coursesQuery.Where(c => !c.IsDelete);
            }

            if (languageId.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.LanguageId == languageId.Value);
            }

            if (levelId.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.LevelId == levelId.Value);
            }

            if (instructorId.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.UserId == instructorId.Value);
            }

            if (categoryId.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.CategoryId == categoryId.Value);
            }

            if (maxPrice.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.Price <= maxPrice.Value);
            }

            if (minPrice > 0)
            {
                coursesQuery = coursesQuery.Where(c => c.Price >= minPrice);
            }

            if (!string.IsNullOrEmpty(name))
            {
                var nameLower = name.ToLower();
                coursesQuery = coursesQuery.Where(c => c.Name.ToLower().Contains(nameLower));
            }

            var courses = coursesQuery.Select(course => new CoursesWithStatusDto
            {
                CategoryId = course.CategoryId,
                CourseId = course.CourseId,
                About = course.About,
                ThumbnailUrl = course.ThumbnailUrl,
                Price = course.Price,
                Name = course.Name,
                VideosCount = course.Videos.Count(v => v.CourseId == course.CourseId),
                Status = _context.UserCourses.Any(uc => uc.UserId == userId && uc.CourseId == course.CourseId)
                                ? CourseStatus.Bought.ToString()
                                : _context.Cart.Any(cart => cart.UserId == userId && cart.CourseId == course.CourseId)
                                    ? CourseStatus.InCart.ToString()
                                    : CourseStatus.NoAction.ToString()
            });


            if (onlyUserCourses)
            {
                courses = courses.Where(c => c.Status == CourseStatus.Bought.ToString());
            }


            var result = courses
                .OrderBy(c => c.CourseId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var totalCourses = await courses.CountAsync();

            var paginationMetaData = new PaginationMetaData(totalCourses, pageSize, pageNumber);

            return (result, paginationMetaData);
        }

        public async Task<(IEnumerable<Course>, PaginationMetaData)> GetInstructorCourses(
            int instructorId,
            int pageNumber,
            int pageSize)
        {
            var courses = _context.Courses.Where(c => c.UserId == instructorId);


            var totalCourses = courses.Count();
            var paginationMetaData = new PaginationMetaData(totalCourses, pageSize, pageNumber);

            var result = await courses
                .Include(c => c.Language)
                .Include(c => c.Level)
                .Include(c => c.UserCourses)
                .Include(c => c.Category)
                .Include(c => c.Videos)
                .OrderBy(c => c.CourseId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (result, paginationMetaData);
        }



        public async Task<IEnumerable<Course>> GetUserCourses(int userId)
        {
            var result = _context.Courses
             .Join(
                 _context.UserCourses.Where(u => u.UserId == userId),
                 c => c.CourseId,
                 u => u.CourseId,
                 (c, u) => new { Course = c, UserCourse = u })
             .OrderBy(r => r.Course.Name)
             .Select(r => new Course
             {
                 CourseId = r.Course.CourseId,
                 About = r.Course.About,
                 ThumbnailUrl = r.Course.ThumbnailUrl,
                 Description = r.Course.Description,
                 LanguageId = r.Course.LanguageId,
                 Price = r.Course.Price,
                 LevelId = r.Course.LevelId,
                 Name = r.Course.Name,
                 CategoryId = r.Course.CategoryId,
                 IsDelete = r.Course.IsDelete,

             });
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<InstructorDto>> GetInstructorsAsync()
        {
            var coursesWithVideos = _context.Courses.Where(c => c.Videos.Any());

            var instructorDtos = coursesWithVideos.Select(c => new InstructorDto
            {
                InstructorId = c.UserId,
                Name = c.User.UserName!
            });

            return await instructorDtos.Distinct().ToListAsync();
        }



        public async Task DeleteAsync(int courseId, int userId)
        {

            // Find the course with the specified ID
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"The course with ID {courseId} was not found.");
            }

            // Check if the user is authorized to delete the course (i.e., the user is the instructor who created this course)
            bool isAuthorized = await _context.Courses.AnyAsync(c => c.CourseId == course.CourseId && c.UserId == userId);
            if (!isAuthorized)
            {
                throw new ApiException(HttpStatusCode.Unauthorized, $"You are not authorized to delete the course with ID {courseId}.");
            }
            // Check if the course is associated with one or more users
            bool isPurchased = await _context.UserCourses.AnyAsync(c => c.CourseId == courseId);

            if (isPurchased)
            {
                // If the course is associated with one or more users, soft-delete it
                course.IsDelete = true;
                _context.Courses.Update(course);
                await _context.SaveChangesAsync(); // Add this line to save the changes to the database
                throw new ApiException(HttpStatusCode.BadRequest, "Unable to delete the course as it has been purchased by one or more users. Only those who have purchased the course can access it.");
            }
            else
            {
                // Delete the associated thumbnail and video files from storage
                await _filesService.DeleteAsync(course.ThumbnailUrl);
                await _filesService.DeleteAsync(course.CourseId.ToString());

                // Remove the course from the database and save the changes
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}