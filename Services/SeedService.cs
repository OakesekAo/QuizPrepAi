using Microsoft.AspNetCore.Identity;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public SeedService(ApplicationDbContext dbContext, IOptions<AppSettings> appSettings, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            await UpdateDatabaseAsync();
            await SeedRolesAsync();
            await SeedUserAsync();
            //await SeedCollections();
        }

        public async Task UpdateDatabaseAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (_dbContext.Roles.Any()) return;

            var adminRole = _appSettings.QuizPrepAiSettings.DefaultCredentials.Role;
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        private async Task SeedUserAsync()
        {
            if (_userManager.Users.Any()) return;

            var credentials = _appSettings.QuizPrepAiSettings.DefaultCredentials;
            var newUser = new IdentityUser()
            {
                Email = credentials.Email,
                UserName = credentials.Email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(newUser, credentials.Password);
            await _userManager.CreateAsync(newUser, credentials.Role);
        }
    }
}
