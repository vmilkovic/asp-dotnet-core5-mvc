using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RockyDataAccess.Data;
using RockyModels;
using RockyUtility;
using System;
using System.Linq;

namespace RockyDataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (!_roleManager.RoleExistsAsync(WC.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WC.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WC.CustomerRole)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = WC.EmailAdmin,
                Email = WC.EmailAdmin,
                EmailConfirmed = true,
                FullName = "Admin",
                PhoneNumber = ""
            }, "Admin12345").GetAwaiter().GetResult();

            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == WC.EmailAdmin);

            if (user == null)
            {
                return;
            }

            _userManager.AddToRoleAsync(user, WC.AdminRole).GetAwaiter().GetResult();
        }
    }
}
