using Labb_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb_3.Data
{
    public class InitialDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<PersonInterest> PersonInterests { get; set; }
        public DbSet<InterestLink> InterestLinks { get; set; }

        public InitialDbContext(DbContextOptions<InitialDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonInterest>()
                .HasKey(pi => new { pi.PersonId, pi.InterestId });

            modelBuilder.Entity<PersonInterest>()
                .HasOne(pi => pi.Person)
                .WithMany(p => p.PersonInterests)
                .HasForeignKey(pi => pi.PersonId);

            modelBuilder.Entity<PersonInterest>()
                .HasOne(pi => pi.Interest)
                .WithMany(i => i.PersonInterests)
                .HasForeignKey(pi => pi.InterestId);

            modelBuilder.Entity<InterestLink>()
                .HasOne(il => il.Interest)
                .WithMany(i => i.Links)
                .HasForeignKey(il => il.InterestId);
        }
    }
}
