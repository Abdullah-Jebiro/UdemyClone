using Models.DbEntities;
using Models.Dtos;
using Models.Dtos.Pagination;
using Services.Repos;

namespace Services.CoursesRepo
{
    public interface ICoursesRepository : IBaseRepository<Course>
    {
        Task<IEnumerable<Course>> GetUserCourses(int userId);
        Task<CoursesWithDetailDto> GetCourse(int userId, int courseId);
        Task<IEnumerable<InstructorDto>> GetInstructorsAsync();
        Task<(IEnumerable<Course>, PaginationMetaData)> GetInstructorCourses(
            int instructorId,
            int pageNumber,
            int pageSize
        );

        Task<(IEnumerable<CoursesWithStatusDto>, PaginationMetaData)> GetCoursesWithStatusAsync(
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
            bool onlyUserCourses

        );

        Task DeleteAsync(int courseId, int userId);

    }
}
