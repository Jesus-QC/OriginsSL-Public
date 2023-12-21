using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OriginsHR.Commands;

namespace OriginsHR;

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
        ButtonHandler buttonHandler = services.GetRequiredService<ButtonHandler>();
        
        await handler.InitializeAsync();
        buttonHandler.Initialize();

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
            .AddSingleton<ButtonHandler>()
            .BuildServiceProvider();
}