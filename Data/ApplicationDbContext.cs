using diplomska.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace diplomska.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Transport> Transport { get; set; }
        public DbSet<Izkladisceno> Izkladisceno { get; set; }
        public DbSet<LoadingChecklist> LoadingChecklists { get; set; }
        public DbSet<ChecklistItem> checklistItems { get; set; }
        public DbSet<ChecklistAnswer> ChecklistAnswers { get; set; }
        public DbSet<ArchivedTransport> ArchivedTransports { get; set; }

    }
}
