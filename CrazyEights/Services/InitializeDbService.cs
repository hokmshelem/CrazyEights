using CrazyEights.Data;
using CrazyEights.Models;
using CrazyEights.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrazyEights.Services
{
    public class InitializeDbService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<InitializeDbService> _logger;

        public InitializeDbService(AppDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ILogger<InitializeDbService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InizializeAsync()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    await _context.Database.MigrateAsync();
                }

                // Create application roles
                if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                }

                if (!await _roleManager.RoleExistsAsync(SD.Role_Player))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Player));
                }

                if (!await _userManager.Users.AnyAsync())
                {
                    var admin = new ApplicationUser
                    {
                        PlayerName = "admin",
                        UserName = "admin",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                    };
                    await _userManager.CreateAsync(admin, SD.Password);
                    await _userManager.AddToRolesAsync(admin, new[] { SD.Role_Admin, SD.Role_Player });

                    var barb = new ApplicationUser
                    {
                        PlayerName = "barb",
                        UserName = "barb",
                        Email = "barb@example.com",
                        EmailConfirmed = true,
                    };
                    await _userManager.CreateAsync(barb, SD.Password);
                    await _userManager.AddToRoleAsync(barb, SD.Role_Player);

                    var john = new ApplicationUser
                    {
                        PlayerName = "joHn",
                        UserName = "john",
                        Email = "john@example.com",
                        EmailConfirmed = true,
                    };
                    await _userManager.CreateAsync(john, SD.Password);
                    await _userManager.AddToRoleAsync(john, SD.Role_Player);

                    var todd = new ApplicationUser
                    {
                        PlayerName = "todd",
                        UserName = "todd",
                        Email = "todd@example.com",
                        EmailConfirmed = true,
                    };
                    await _userManager.CreateAsync(todd, SD.Password);
                    await _userManager.AddToRoleAsync(todd, SD.Role_Player);

                    var peter = new ApplicationUser
                    {
                        PlayerName = "peter",
                        UserName = "peter",
                        Email = "peter@example.com",
                        EmailConfirmed = true,
                    };
                    await _userManager.CreateAsync(peter, SD.Password);
                    await _userManager.AddToRoleAsync(peter, SD.Role_Player);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while migrating or initializing the database");
            }
        }
    }
}
