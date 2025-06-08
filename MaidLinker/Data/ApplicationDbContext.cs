using MaidLinker.Data.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaidLinker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

            // No Code Here 
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserProfile>().HasQueryFilter(f => f.AccountTypeId != 0); // Ignore Admin User From All Queries
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<GeneralSettings> GeneralSettings { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<PractitionerType> PractitionerTypes { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Maid> Maids { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}