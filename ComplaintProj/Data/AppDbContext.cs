using ComplaintProj.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComplaintProj.Data
{
    public class AppDbContext: IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<ComplaintModel> Complaints { get; set; }
        public DbSet<AuditLogModel> AuditLogs { get; set; }
    }
}
