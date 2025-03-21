using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FOE_CourseRegistrationSystem.Data;

namespace FOE_CourseRegistrationSystem.Services
{
    public class CloseExpiredSessionsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CloseExpiredSessionsService> _logger;

        public CloseExpiredSessionsService(IServiceProvider serviceProvider, ILogger<CloseExpiredSessionsService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // ✅ Close expired sessions
                        var expiredSessions = _context.RegistrationSessions
                            .Where(rs => rs.IsOpen && rs.EndDate < DateTime.UtcNow)
                            .ToList();

                        if (expiredSessions.Any())
                        {
                            foreach (var session in expiredSessions)
                            {
                                session.IsOpen = false;
                            }

                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"✅ {expiredSessions.Count} expired registration sessions have been closed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error while closing expired sessions: {ex.Message}");
                }

                // ✅ Run every 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
