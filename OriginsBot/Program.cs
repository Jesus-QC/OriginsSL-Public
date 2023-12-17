using System.Web;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OriginsBot.Commands;
using OriginsBot.Security;
using OriginsBot.Website;

namespace OriginsBot;

internal static class Program
{
    private static void Main(string[] args)
    {
#pragma warning disable CS4014
        RunBotAsync();
#pragma warning restore CS4014 
        WebsiteBuilder.Run(args);
    }

    private static async Task RunBotAsync()
    {
        await using ServiceProvider services = ConfigureServices();

        DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();
        InteractionService commands = services.GetRequiredService<InteractionService>();
        IConfiguration config = services.GetRequiredService<IConfiguration>();
        CommandHandler handler = services.GetRequiredService<CommandHandler>();
        DatabaseHandler databaseHandler = services.GetRequiredService<DatabaseHandler>();
        
        databaseHandler.Initialize();
        
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

        client.Ready += () => OnReady(client);

        await client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private static Task OnReady(BaseSocketClient client)
    {
        return client.SetStatusAsync(UserStatus.Idle);
    }

    private static ServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("discord.json").Build())
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<DatabaseHandler>()
            .BuildServiceProvider();
}