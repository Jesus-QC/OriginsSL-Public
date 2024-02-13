using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OriginsHRInternal.Commands;

namespace OriginsHRInternal;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        await RunBotAsync();
    }

    private static async Task RunBotAsync()
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

        client.Ready += async () =>
        {
            await LeaveOfAbsenceHandler.InitializeAsync(client, config);
            await OnReady(client);
        };

        await client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private static async Task OnReady(BaseSocketClient client)
    {
        await client.SetStatusAsync(UserStatus.Idle);
    }

    private static ServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("discord.json").Build())
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All
            }))
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();
}