using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.DbEntities;
using Models.Identity;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext
        <ApplicationUser,
           ApplicationRole,
           int,
           ApplicationUserClaim,
           ApplicationUserRole,
           ApplicationUserLogin,
           ApplicationRoleClaim,
           ApplicationUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }


        public virtual DbSet<UserCourses> UserCourses { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<CartItem> Cart { get; set; }
        public virtual DbSet<InstructorAccount> InstructorsAccounts { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Level>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Language>().HasQueryFilter(c => !c.IsDeleted);

            builder.Entity<InstructorAccount>(entity =>
            {
                entity.HasKey(i=>i.UserId);
                entity.HasOne(i => i.User)
                    .WithOne()
                    .HasForeignKey<InstructorAccount>("UserId");
                
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });

            builder.Entity<CartItem>(entity =>
            {
                entity.HasOne(ci => ci.Course)
                .WithMany()
                .HasForeignKey(ci => ci.CourseId)         
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<UserCourses>(entity =>
            {
                entity.HasKey(uc => uc.UserCoursesId);

                entity.HasOne(uc => uc.User)
                    .WithMany(u => u.UserCourses)
                    .HasForeignKey(uc => uc.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(uc => uc.Course)
                    .WithMany(c => c.UserCourses)
                    .HasForeignKey(uc => uc.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });

            builder.Entity<ApplicationUserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity
                    .HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                entity
                    .HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                entity.ToTable("UserRoles");
            });


            builder.Entity<ApplicationUserClaim>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<ApplicationUserLogin>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<ApplicationRoleClaim>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<ApplicationUserToken>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }
    }
}