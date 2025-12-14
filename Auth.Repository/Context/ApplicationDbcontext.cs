

using Auth.Domain._0.Base;
using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppGroups.Entity;
using Auth.Domain.AppProjects.Aggregate;
using Auth.Domain.AppProjects.Entity;
using Auth.Domain.AppRoles.Aggregate;
using Auth.Domain.AppRoles.Entity;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Domain.AppUsers.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Auth.Repository.Context
{
    public class ApplicationDbcontext : IdentityDbContext<
                                        AppUser,
                                        AppRole,
                                        Guid,
                                        AppClaim,
                                        AppRoleUser,
                                        IdentityUserLogin<Guid>,   // <— placeholder, ignored later
                                        IdentityRoleClaim<Guid>,
                                        IdentityUserToken<Guid>>
    {
        //public DbSet<AppUser> AppUsers { get; set; }
        //public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppClaim> AppClaims { get; set; }
        public DbSet<AppLogin> AppLogins { get; set; }
        public DbSet<AppRoleUser> AppRoleUsers { get; set; }
        public DbSet<AppProject> AppProjects { get; set; }
        public DbSet<AppUrl> AppUrls { get; set; }
        public DbSet<AppRefreshToken> AppRefreshTokens { get; set; }
        public DbSet<AppGroup> AppGroups { get; set; }
        public DbSet<AppGroupUrl> AppGroupUrls { get; set; }
        public DbSet<AppGroupUser> AppGroupUsers { get; set; }

        public ApplicationDbcontext() { }
        

        public ApplicationDbcontext(DbContextOptions opt) : base(opt) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

      
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
           

            modelBuilder.Entity<AppRoleUser>()
               .HasKey(r => new { r.UserId, r.RoleId });

            

            modelBuilder.Entity<AppClaim>()
                   .HasKey(c => c.Id);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                // Apply only to your base classes
                if (!typeof(EntityBase<>).IsAssignableFrom(clrType.BaseType))
                    continue;

                var builder = modelBuilder.Entity(clrType);

                // ID configuration
                var idProp = clrType.GetProperty("Id");
                if (idProp != null)
                {
                    builder.HasKey("Id");

                    if (idProp.PropertyType == typeof(Guid))
                        builder.Property("Id").HasDefaultValueSql("NEWID()");
                    else if (idProp.PropertyType == typeof(int))
                        builder.Property("Id").ValueGeneratedOnAdd();
                }

                // CreationDateTime
                builder.Property("CreationDateTime")
                    .HasDefaultValueSql("GETUTCDATE()");
            }
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=CV_Project;User Id=sa;Password=@dm!n123456;Encrypt=True;TrustServerCertificate=True", cf =>
            {
                cf.UseHierarchyId();
                cf.UseNetTopologySuite();
            });



                  optionsBuilder.UseSqlServer(cf =>
                  {
                      cf.UseHierarchyId();
                      cf.UseNetTopologySuite();
                  });
            optionsBuilder.UseLazyLoadingProxies();


            base.OnConfiguring(optionsBuilder);
        }
    }
}
