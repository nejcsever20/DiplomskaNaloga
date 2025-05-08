using System;
using System.IO;
using System.Threading.Tasks;

namespace diplomska.Services
{
    public class UserActivityLogger
    {
        private readonly string _logFilePath = "user_activity_log.txt"; // path for the txt file

        public async Task LogAsync(string userId, string activity, string action)
        {
            // Log to text file
            await LogToTextFileAsync(userId, activity, action);

            // Log to database (you can modify it to your needs)
            await LogToDatabaseAsync(userId, activity, action);
        }

        private async Task LogToTextFileAsync(string userId, string activity, string action)
        {
            try
            {
                string logEntry = $"{DateTime.Now}: User {userId} {activity} ({action})";
                await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log to file: {ex.Message}");
            }
        }

        private async Task LogToDatabaseAsync(string userId, string activity, string action)
        {
            // Implement your database logging logic here
            // Example: Add a log entry into a database (you might use Entity Framework or Dapper)
        }
    }
}