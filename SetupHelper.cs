using RegistrationLoginApi.Data.DataModels;
using RegistrationLoginApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Threading;
using BCryptNet = BCrypt.Net.BCrypt;
namespace RegistrationLoginApi.Test{
    public class SetupHelper{
        public async static Task<IAppDbContext> SetupDbContext(string contextName, bool mockData){
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: contextName)
            .Options;

            var myDbContext = new AppDbContext(options);
            if(!mockData)
                return myDbContext;

            await PopulateDbContext(myDbContext);
            return myDbContext;
        }
        public async static Task PopulateDbContext(IAppDbContext context){
            var user = new User{
                Id = 1,
                Username = "tester",
                FirstName = "bob",
                LastName = "builder",
                PasswordHash = BCryptNet.HashPassword("test")
            };
            context.Users.Add(user);
            await context.SaveChangesAsync(new CancellationToken());
            context.Entry(user).State = EntityState.Detached;
        }
    }
}