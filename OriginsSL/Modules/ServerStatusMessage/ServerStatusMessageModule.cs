using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Server;
using Discord;
using Discord.Rest;
using OriginsSL.Loader;

namespace OriginsSL.Modules.ServerStatusMessage;

public class ServerStatusMessageModule : OriginsModule
{
    public static ServerStatusMessageConfig Config { get; internal set; }
    
    private static readonly List<CancellationTokenSource> CancellationTokenSources = [];

    private static DiscordRestClient _discordRestClient;
    
    public override void OnLoaded() => Start();
    
    private static void Start()
    {
        foreach (CancellationTokenSource token in CancellationTokenSources)
            token.Cancel();
        
        CancellationTokenSources.Clear();
        
        CancellationTokenSource cancellationTokenSource = new();
        Task.Run(() => Timer(cancellationTokenSource), cancellationTokenSource.Token);
        
        CancellationTokenSources.Add(cancellationTokenSource);
    }
    
    private static async Task Timer(CancellationTokenSource cancellationTokenSource)
    {
        _discordRestClient = new DiscordRestClient();
        await _discordRestClient.LoginAsync(TokenType.Bot, Config.BotToken);
        
        RestGuild restGuild = await _discordRestClient.GetGuildAsync(Config.GuildId);
        RestTextChannel restTextChannel = await restGuild.GetTextChannelAsync(Config.ChannelId);
        RestUserMessage newMessage = await GetMessageToEditAsync(_discordRestClient, restTextChannel);
        
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            await Task.Delay(5000, cancellationTokenSource.Token);
            await newMessage.ModifyAsync(x =>
            {
                x.Content = GetTimestamp();
                x.Embed = BuildEmbed();
            });

            while (CursedServer.IsInIdleMode)
            {
                await Task.Delay(1000, cancellationTokenSource.Token);
            }
        }
    }

    private static async Task<RestUserMessage> GetMessageToEditAsync(DiscordRestClient client, IRestMessageChannel restTextChannel)
    {
        foreach (RestMessage msg in await restTextChannel.GetMessagesAsync(5).FlattenAsync())
        {
            if (msg.Author.Id != client.CurrentUser.Id || msg.Embeds.All(x => x.Title != Config.EmbedTitle))
                continue;

            await msg.DeleteAsync();
        }
        
        return await restTextChannel.SendMessageAsync(GetTimestamp());
    }
    
    private static string GetTimestamp()
    {
        if (CursedServer.IsInIdleMode)
            return "Refreshing when players connect.";
        
        return "Refreshing <t:" + DateTimeOffset.UtcNow.AddSeconds(5).ToUnixTimeSeconds() + ":R>";
    }

    private static Embed BuildEmbed()
    {
        return new EmbedBuilder()
            .WithTitle(Config.EmbedTitle)
            .AddField("Status", "```yml\n+ Online```", true)
            .AddField("Players", $"```cs\n- {CursedPlayer.Count} / {CursedServer.MaxPlayerSlots}```", true)
            .AddField("Version", $"```cs\n- {GameCore.Version.VersionString}```", true)
            .AddField("Player List", "```cs\n" + GetPlayerList() + "```", true)
            .WithFooter("Origins SL - made with 🩷 by jesusqc")
            .Build();
    }

    private static string GetPlayerList()
    {
        if (CursedPlayer.Count == 0)
            return " - No players online :c";
        
        string playerList = string.Empty;
        foreach (CursedPlayer player in CursedPlayer.Dictionary.Values)
        {
            playerList += "- " + player.DisplayNickname + "\n";
        }

        return playerList;
    }
}