using Microsoft.EntityFrameworkCore;
using diploma.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography.Xml;

namespace diploma.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<User> diplomska_Users { get; set; }
        public DbSet<Transport> diplomska_Transports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("diplomska_Users");
            modelBuilder.Entity<Transport>().ToTable("diplomska_Transports");

            modelBuilder.Entity<Transport>()
                .HasOne<User>() // no navigation property defined (optional)
                .WithMany()
                .HasForeignKey(t => t.DoločenSkladiščnik)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
