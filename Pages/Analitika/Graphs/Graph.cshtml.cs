using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diplomska.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace diplomska.Pages.Analitika.Graphs
{
    [Authorize(Roles = "Admin, Analitika")]
    public class GraphModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GraphModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // JSON data to be used in Razor page for Chart.js
        public string AggregateSumsJson { get; set; } = "[]";
        public string WarehouseCountsJson { get; set; } = "[]";
        public string WorkerCountsJson { get; set; } = "[]";

        // New checklist graphs JSON strings
        public string ChecklistYesNoCountsJson { get; set; } = "[]";
        public string ChecklistCommentsCountsJson { get; set; } = "[]";

        // Additional new JSON properties for transport counts per worker
        public string TransportNumbersPerWorkerJson { get; set; } = "[]";
        public string UnfinishedTransportCountsJson { get; set; } = "[]";

        public string CallbackByReasonDateJson { get; set; }
        public string CallbackByReasonIzkladiscenoJson { get; set; }

        public string InternalExternalLoginCountsJson { get; set; } = "[]";
        public string UserRolesCountsJson { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Aggregate sums: StTransporta, Kolicina, Palete
            var stTransportaSum = await _context.Transport
                .Where(t => t.StTransporta.HasValue)
                .SumAsync(t => (long?)t.StTransporta) ?? 0;

            var izkladiscenoSums = await _context.Izkladisceno
                .GroupBy(i => 1)
                .Select(g => new
                {
                    TotalKolicina = g.Sum(x => x.Kolicina) ?? 0,
                    TotalPalete = g.Sum(x => x.Palete) ?? 0
                }).FirstOrDefaultAsync();

            var aggregateSums = new List<object>
            {
                new { Label = "StTransporta", Value = stTransportaSum },
                new { Label = "Kolicina", Value = izkladiscenoSums?.TotalKolicina ?? 0 },
                new { Label = "Palete", Value = izkladiscenoSums?.TotalPalete ?? 0 }
            };

            AggregateSumsJson = JsonSerializer.Serialize(aggregateSums);

            // 2. Warehouse (Skladisce) counts
            var warehouseCounts = await _context.Transport
                .Where(t => !string.IsNullOrEmpty(t.Skladisce))
                .GroupBy(t => t.Skladisce)
                .Select(g => new { Warehouse = g.Key, Count = g.Count() })
                .ToListAsync();

            WarehouseCountsJson = JsonSerializer.Serialize(warehouseCounts);

            // 1. Transport numbers per worker asynchronously
            var transportNumbersPerWorker = await _context.Transport
                .Where(t => !string.IsNullOrEmpty(t.DolocenSkladiscnikId))
                .GroupBy(t => t.DolocenSkladiscnikId)
                .Select(g => new { WorkerId = g.Key, Count = g.Count() })
                .ToListAsync();

            TransportNumbersPerWorkerJson = JsonSerializer.Serialize(transportNumbersPerWorker);

            // 2. Unfinished transport counts per worker asynchronously
            var unfinishedTransportCounts = await _context.Transport
                .Where(t => !string.IsNullOrEmpty(t.DolocenSkladiscnikId) && !t.KonecNaklada.HasValue)
                .Join(_context.Users,
                    t => t.DolocenSkladiscnikId,
                    u => u.Id,
                    (t, u) => new { u.Email })
                .GroupBy(x => x.Email)
                .Select(g => new
                {
                    WorkerEmail = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            UnfinishedTransportCountsJson = JsonSerializer.Serialize(unfinishedTransportCounts);

            // 3. Worker counts with user info (clean EF Core, async)
            var groupedWorkerCounts = await _context.Izkladisceno
                .Where(i => !string.IsNullOrEmpty(i.SkladiscnikId))
                .GroupBy(i => i.SkladiscnikId)
                .Select(g => new { WorkerId = g.Key, Count = g.Count() })
                .ToListAsync();

            var workerCountsList = new List<object>();
            foreach (var item in groupedWorkerCounts)
            {
                var user = await _context.Users.FindAsync(item.WorkerId);
                workerCountsList.Add(new
                {
                    WorkerId = item.WorkerId,
                    UserName = user?.UserName,
                    Email = user?.Email,
                    Count = item.Count
                });
            }

            WorkerCountsJson = JsonSerializer.Serialize(workerCountsList);

            // 4. Checklist Yes/No counts:
            var checklistYesNoData = await _context.LoadingChecklists
                 .Where(lc => !string.IsNullOrEmpty(lc.TransportNumber))
                 .SelectMany(lc => lc.ChecklistAnswer.DefaultIfEmpty(), (lc, ca) => new
                 {
                     TransportNumber = lc.TransportNumber,
                     Answer = ca != null ? ca.Answer : null
                 })
                 .ToListAsync();

            var checklistYesNoGrouped = checklistYesNoData
                .GroupBy(x => x.TransportNumber)
                .Select(g => new
                {
                    TransportNumber = g.Key,
                    YesCount = g.Count(x => !string.IsNullOrEmpty(x.Answer) && x.Answer.ToLower() == "yes"),
                    NoCount = g.Count(x => !string.IsNullOrEmpty(x.Answer) && x.Answer.ToLower() == "no")
                })
                .ToList();

            ChecklistYesNoCountsJson = JsonSerializer.Serialize(checklistYesNoGrouped);


            // 5. Checklist Comments counts:
            var checklistCommentsData = await _context.LoadingChecklists
                .Where(lc => !string.IsNullOrEmpty(lc.TransportNumber))
                .SelectMany(lc => lc.ChecklistAnswer.DefaultIfEmpty(), (lc, ca) => new
                {
                    TransportNumber = lc.TransportNumber,
                    Comment = ca != null ? ca.Comment : null
                })
                .ToListAsync();

            var checklistCommentsGrouped = checklistCommentsData
                .GroupBy(x => x.TransportNumber)
                .Select(g => new
                {
                    TransportNumber = g.Key,
                    CommentCount = g.Count(x => !string.IsNullOrWhiteSpace(x.Comment))
                })
                .ToList();

            ChecklistCommentsCountsJson = JsonSerializer.Serialize(checklistCommentsGrouped);

            // 6. Callback counts grouped by CallbackReason and date
            var rawCallbackReasonAndDate = await _context.ArchivedTransports
                .Where(a => a.IsCallback && !string.IsNullOrEmpty(a.CallbackReason) && a.PlaniranPrihod.HasValue)
                .GroupBy(a => new
                {
                    a.CallbackReason,
                    Date = a.PlaniranPrihod.Value.Date
                })
                .Select(g => new
                {
                    g.Key.CallbackReason,
                    g.Key.Date,
                    Count = g.Count()
                })
                .OrderBy(e => e.Date)
                .ToListAsync();  // Run this as an async DB query

            // Now do the formatting in memory
            var callbackReasonAndDate = rawCallbackReasonAndDate
                .Select(g => new
                {
                    g.CallbackReason,
                    Date = g.Date.ToString("yyyy-MM-dd"),
                    g.Count
                })
                .ToList();



            var callbackByReasonAndIzkladisceno = await (
                from transport in _context.ArchivedTransports  // Make sure this matches your DbSet name
                join izk in _context.Izkladisceno on transport.Id equals izk.TransportId
                where transport.IsCallback && !string.IsNullOrEmpty(transport.CallbackReason)
                group izk by new { transport.CallbackReason, izk.Skladiscnik } into g
                select new
                {
                    CallbackReason = g.Key.CallbackReason,
                    Skladiscnik = g.Key.Skladiscnik ?? "Unknown",
                    Count = g.Count()
                })
                .OrderBy(x => x.Skladiscnik)
                .ThenBy(x => x.CallbackReason)
                .ToListAsync();

                        CallbackByReasonDateJson = JsonSerializer.Serialize(callbackReasonAndDate);
                        CallbackByReasonIzkladiscenoJson = JsonSerializer.Serialize(callbackByReasonAndIzkladisceno);


            // 10. get all user logins from AspnetUserLogins
            var users = await _context.Users.ToListAsync();

            int internalCount = 0;
            int externalCount = 0;

            foreach (var user in users)
            {
                var logins = await _userManager.GetLoginsAsync(user);

                if (logins.Count == 0)
                    internalCount++;

                else
                    externalCount++;
            }

            var loginCounts = new[]
            {
                new { LoginType= "Internal", UserCount = internalCount},
                new {LoginType = "External", UserCount = externalCount}
            };

            InternalExternalLoginCountsJson = JsonSerializer.Serialize(loginCounts);

            //11. role per user capita
            var roles = _roleManager.Roles.ToList();
            var roleCounts = new List<object>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name); 
                roleCounts.Add(new { Role = role.Name, UserCount = usersInRole.Count });
            }

            UserRolesCountsJson = JsonSerializer.Serialize(roleCounts);
            return Page();
        }
    }
}
