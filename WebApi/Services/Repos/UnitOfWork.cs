
using Data;
using Models.DbEntities;
using Services.CoursesRepo;
using Services.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IBaseRepository<Video> Videos { get; private set; }
        public IBaseRepository<Category> Categories { get; private set; }
        public IBaseRepository<UserCourses> UsersCourses { get; private set; }
        public IBaseRepository<Level> Levels { get; private set; }
        public IBaseRepository<CartItem> Carts { get; private set; }
        public IBaseRepository<Language> Languages { get; private set; }
        public IBaseRepository<InstructorAccount> InstructorsAccounts { get; private set; }
        public ICoursesRepository Courses { get; private set; }
        public UnitOfWork(ApplicationDbContext context, IFilesService _filesService)
        {

           _context = context;
            Courses = new CoursesRepository(_context, _filesService);
            Videos = new BaseRepository<Video>(_context);
            Categories = new BaseRepository<Category>(_context);
            UsersCourses = new BaseRepository<UserCourses>(_context);
            Languages=  new BaseRepository<Language>(_context);
            Levels = new BaseRepository<Level>(_context);
            Carts= new BaseRepository<CartItem>(_context);
            InstructorsAccounts= new BaseRepository<InstructorAccount>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}