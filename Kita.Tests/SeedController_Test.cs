using Kita.Infrastructure.Identity;
using Kita.Infrastructure.Persistence;
using Kita.Infrastructure.Services;
using Kita.Server.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Kita.Tests;

public class SeedController_Test
{
    [Fact]
    public async Task CreateDefaultUsers()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var context = new ApplicationDbContext(options);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["DefaultPasswords:RegisteredUser"] = "M0ckP$$word",
                ["DefaultPasswords:Administrator"] = "M0ckP$$word"
            }).Build();
        using var roles = IdentityHelper.GetRoleManager(new RoleStore<IdentityRole>(context));
        using var users = IdentityHelper.GetUserManager(new UserStore<ApplicationUser>(context));
#pragma warning disable CS0618 // SeedService currently accepts the legacy hosting interface.
        var environment = Mock.Of<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
#pragma warning restore CS0618
        var controller = new SeedController(new SeedService(roles, users, environment, configuration, context));

        await controller.CreateDefaultUsers();

        var admin = await users.FindByEmailAsync("admin@email.com");
        var user = await users.FindByEmailAsync("user@email.com");
        Assert.NotNull(admin);
        Assert.NotNull(user);
        Assert.True(await users.IsInRoleAsync(admin, "Administrator"));
        Assert.True(await users.IsInRoleAsync(user, "RegisteredUser"));
        Assert.Null(await users.FindByEmailAsync("notexisting@email.com"));

        await controller.CreateDefaultUsers();
        Assert.Equal(2, await context.Users.CountAsync());
    }
}
