using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OriginsBot.Commands;

namespace OriginsBot;

internal static class Program
{
    static void Main(string[] args)
    {
        RunAsync().GetAwaiter().GetResult();
    }

    static async Task RunAsync()
    {
        await using ServiceProvider services = ConfigureServices();

        DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();
        InteractionService commands = services.GetRequiredService<InteractionService>();
        IConfiguration config = services.GetRequiredService<IConfiguration>();
        CommandHandler handler = services.GetRequiredService<CommandHandler>();
        
        await handler.InitializeAsync();

        await client.LoginAsync(TokenType.Bot, config["token"]);

        client.Log += msg =>
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        };

        commands.Log += msg =>
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        };

        await client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private static ServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();
}