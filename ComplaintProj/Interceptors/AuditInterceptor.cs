using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ComplaintProj.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ComplaintProj.Interceptors 
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                ProcessEntries(eventData.Context);
            }
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context != null)
            {
                ProcessEntries(eventData.Context);
            }
            return base.SavingChanges(eventData, result);
        }

        private void ProcessEntries(DbContext context)
        {
           
            var now = DateTime.UtcNow;

            var user = _httpContextAccessor.HttpContext?.User;
            var currentUsername = user?.Identity?.Name
                           ?? user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                           ?? "Anonymous";

           
            var entries = context.ChangeTracker.Entries<ComplaintModel>()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted);

            var auditEntries = new List<AuditLogModel>();

            foreach (var entry in entries)
            {
              
                if (entry.Entity is AuditLogModel) continue;

                var auditLog = new AuditLogModel
                {
                    UserId = currentUsername,
                    TableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                    DateTime = now
                };

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;

                    auditLog.Type = "Insert";
                    auditLog.NewValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;

                    auditLog.Type = "Update";
                    auditLog.OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    auditLog.NewValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = now;

                    auditLog.Type = "Delete (Soft)";
                    auditLog.OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                }

                auditEntries.Add(auditLog);
            }

            if (auditEntries.Any())
            {
                context.Set<AuditLogModel>().AddRange(auditEntries);
            }
        }
    }
}
