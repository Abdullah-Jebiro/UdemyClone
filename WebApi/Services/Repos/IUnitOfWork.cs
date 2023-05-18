
using Models.DbEntities;
using Services.CoursesRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repos
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Video> Videos { get;}
        IBaseRepository<Category> Categories { get;}
        IBaseRepository<UserCourses> UsersCourses { get;}
        IBaseRepository<Level> Levels { get; }
        IBaseRepository<Language> Languages { get;}  
        IBaseRepository<CartItem> Carts { get;}       
        IBaseRepository<InstructorAccount> InstructorsAccounts { get;}       
        ICoursesRepository Courses { get; }
    }
}