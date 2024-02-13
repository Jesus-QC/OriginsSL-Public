using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using IResult = Discord.Interactions.IResult;

namespace OriginsHRInternal.Commands;

public class CommandHandler(InteractionService commands, DiscordSocketClient discord, IConfiguration configuration, IServiceProvider services)
{
    public async Task InitializeAsync()
    {
        await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
        discord.InteractionCreated += InteractionCreated;
        discord.ButtonExecuted += ButtonExecuted;
        discord.Ready += Ready;
        commands.SlashCommandExecuted += SlashCommandExecuted;
        commands.AutocompleteHandlerExecuted += AutocompleteHandlerExecuted;
    }

    private static Task AutocompleteHandlerExecuted(IAutocompleteHandler arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    private static Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    // Generic variants of interaction contexts can be used to create interaction specific modules, but you need to make sure that the destination command resides in a module
    // with the matching context type. See -> ComponentOnlyModule
    private async Task ButtonExecuted(SocketMessageComponent arg)
    {
        SocketInteractionContext<SocketMessageComponent> ctx = new (discord, arg);
        await commands.ExecuteCommandAsync(ctx, services);
    }

    private async Task Ready()
    {
        await RegisterCommands();
        discord.Ready -= Ready;
    }

    private async Task InteractionCreated(SocketInteraction arg)
    {
        SocketInteractionContext ctx = new (discord, arg);
        await commands.ExecuteCommandAsync(ctx, services);
    }

    private async Task RegisterCommands()
    {
        await commands.RegisterCommandsToGuildAsync(configuration.GetValue<ulong>("guild"));
    }
}