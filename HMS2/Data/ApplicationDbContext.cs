using HMS2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HMS2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Appointment> Appointments
        {
            get; set;
        }
        public DbSet<Department> Departments
        {
            get; set;
        }


        public async Task<int> SaveChangesAsync()
        {
            string user = httpContextAccessor.HttpContext.User.Identity.Name;
            AddTimestamps(user);
            return await base.SaveChangesAsync();
        }


        private void AddTimestamps(string owner)
        {
            foreach (EntityEntry<BaseEntity> entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = owner;
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = owner;
                        entry.Entity.UpdatedDate = DateTime.Now;
                        break;
                }
            }
        }

        

    }
}
