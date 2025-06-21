using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<AdoptionApplication> AdoptionApplications { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } // Or Admin if different

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AdoptionApplication → Pet (Many-to-One)
            modelBuilder.Entity<AdoptionApplication>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.AdoptionApplications)
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            // AdoptionApplication → Admin (Many-to-One)
            modelBuilder.Entity<AdoptionApplication>()
                .HasOne(a => a.Admin)
                .WithMany()
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
