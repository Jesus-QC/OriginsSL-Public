using Discord;
using Discord.WebSocket;

namespace OriginsBot;

internal static class Program
{
    private static readonly DiscordSocketClient Client = new();

    private static void Main(string[] args)
    {
        new LevelingImageBuilder("username", 7778, 80, 7085, 9999).Build();
        return;
        RunAsync(args[0]).GetAwaiter().GetResult();
    }

    private static async Task RunAsync(string token)
    {
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        await Task.Delay(-1);
    }
}