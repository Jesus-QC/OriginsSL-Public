using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace OriginsBot.Commands;

public class CommandHandler
{
    private readonly InteractionService _commands;
    private readonly DiscordSocketClient _discord;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;

    public CommandHandler(InteractionService commands, DiscordSocketClient discord, IConfiguration configuration, IServiceProvider services)
    {
        _commands = commands;
        _discord = discord;
        _configuration = configuration;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
        _discord.InteractionCreated += InteractionCreated;
        _discord.ButtonExecuted += ButtonExecuted;
        _discord.Ready += Ready;
        _commands.SlashCommandExecuted += _commands_SlashCommandExecuted;
        _commands.AutocompleteHandlerExecuted += _commands_AutocompleteHandlerExecuted;
    }

    private Task _commands_AutocompleteHandlerExecuted(IAutocompleteHandler arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    private Task _commands_SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    // Generic variants of interaction contexts can be used to create interaction specific modules, but you need to make sure that the destination command resides in a module
    // with the matching context type. See -> ComponentOnlyModule
    private async Task ButtonExecuted(SocketMessageComponent arg)
    {
        SocketInteractionContext<SocketMessageComponent> ctx = new (_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task Ready()
    {
        await RegisterCommands();
        _discord.Ready -= Ready;
    }

    private async Task InteractionCreated(SocketInteraction arg)
    {
        SocketInteractionContext ctx = new (_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task RegisterCommands()
    {
        await _commands.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("guild"), true);
    }
}