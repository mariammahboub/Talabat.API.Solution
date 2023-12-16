using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = "mariam mahboub",
                    Email = "mahboubmariam18@gmail.com",
                    UserName = "mariammahboub",
                    PhoneNumber = "01091973412"
                };
                await userManager.CreateAsync(user, "P@$$w0rd");
            }
        }
    }
}
