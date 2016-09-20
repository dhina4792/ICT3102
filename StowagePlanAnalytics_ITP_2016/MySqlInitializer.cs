using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StowagePlanAnalytics_ITP_2016.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace StowagePlanAnalytics_ITP_2016
{
    public class MySqlInitializer : IDatabaseInitializer<ApplicationDbContext>
    {
        public void InitializeDatabase(ApplicationDbContext context)
        {
            if (!context.Database.Exists())
            {
                // if database did not exist before - create it
                context.Database.Create();
                Seed(context);
            }
            else
            {
                // query to check if MigrationHistory table is present in the database 
                var migrationHistoryTableExists = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<int>(
                string.Format(
                  "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{0}' AND table_name = '__MigrationHistory'",
                  "StowageDB"));

                // if MigrationHistory table is not there (which is the case first time we run) - create it
                if (migrationHistoryTableExists.FirstOrDefault() == 0)
                {
                    context.Database.Delete();
                    context.Database.Create();
                    Seed(context);
                }
            }
        }

        public void Seed(ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            // check for existing user called "Admin" in DB
            if (!context.Users.Any(t => t.UserName.Equals("Admin")))
            {
                // create new Admin account
                var user1 = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "Admin@localhost.com"
                };
                // add user with the password "P@ssword" into DB
                userManager.Create(user1, "P@ssw0rd");

                // create new Manager account
                var user2 = new ApplicationUser
                {
                    UserName = "Manager",
                    Email = "Manager@localhost.com"
                };
                // add user with the password "P@ssword" into DB
                userManager.Create(user2, "P@ssw0rd");

                // create new role called "Admin"
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = "Admin"
                });
                // create new role called "Manager"
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = "Manager"
                });
                context.SaveChanges();
                // Assign role to account
                userManager.AddToRole(user1.Id, "Admin");
                userManager.AddToRole(user2.Id, "Manager");
            }
        }
    }
}