using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuizPrepAi.Data;
using QuizPrepAi.Models.Settings;

namespace QuizPrepAi.Services
{
    public class SeedService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AppSettings _appSettings;

        public SeedService(ApplicationDbContext dbContext,IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync()
        }

        private async Task SeedRolesAsync()
        {
            if (_dbContext.Roles.Any()) return;

            var adminRole = _appSettings.QuizPrepAiSettings.Defa
        }
    }
}
