using Microsoft.AspNetCore.Authentication.Cookies;

namespace OriginsBot.Website;

public static class WebsiteBuilder
{
    public static void Run(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/signout";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.Cookie.MaxAge = TimeSpan.FromDays(7);
            options.Cookie.IsEssential = true;
        }).AddSteam("Steam", options =>
        {
            options.ApplicationKey = "D7D8C6898D3C0ADD733403A9BAD41D97";
        });
        
        builder.Services.AddControllers();

        WebApplication app = builder.Build();

        app.UseRouting();

        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.None, Secure = CookieSecurePolicy.Always, });

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}