using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using diplomska.Services;

namespace diplomska.Pages.Services
{
    public class SystemLogsModel : PageModel
    {
        private readonly ISystemLogs _systemLogs;

        public SystemLogsModel(ISystemLogs systemLogs)
        {
            _systemLogs = systemLogs;
        }

        public List<LogEntry> LogEntries { get; set; }

        public void OnGet()
        {
            // For now, mock up some log entries. Replace this with actual log retrieval.
            LogEntries = new List<LogEntry>
            {
                new LogEntry { Timestamp = DateTime.Now, Message = "System started successfully." },
                new LogEntry { Timestamp = DateTime.Now.AddMinutes(-10), Message = "An error occurred.", Exception = "NullReferenceException: Object reference not set to an instance of an object." }
            };
        }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
    }
}
