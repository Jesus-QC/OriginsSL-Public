using System;
using System.Threading.Tasks;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using Discord.Rest;
using NWAPIPermissionSystem;
using OriginsSL.Modules.ServerStatusMessage;

namespace OriginsSL.Modules.TrustedMembers;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(ClientCommandHandler))]
public class TrustedVoteBanCommand : ICommand
{
    public static TrustedMembersConfig Config;
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer player = CursedPlayer.Get(sender);

        if (!player.IsHost && !sender.CheckPermission("origins.trusted.vote.ban"))
        {
            response = "You don't have perms to do that!";
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "Usage: <color=red>voteban</color> <color=yellow>playerusername</color>";
            return false;
        }
        
        string target = string.Join(" ", arguments);
        
        if (!CursedPlayer.TryGet(target, out CursedPlayer targetPlayer))
        {
            response = "Player not found!";
            return false;
        }
        
        Task.Run(async () =>
        {
#pragma warning disable CS4014
            if (PollManager.PollManager.RunPoll(player.RealNickname, $"(trusted member) ban <color=red>{targetPlayer.DisplayNickname}</color>").GetAwaiter().GetResult())
            {
                targetPlayer.Ban("You have been banned due to a vote ban poll by a trusted member.\nIf you believe this was abused contact us on discord\ndiscord.gg/scporigins\n\n[Kicked by a modification]", 300);

                RestGuild guild = await ServerStatusMessageModule.DiscordRestClient.GetGuildAsync(ServerStatusMessageModule.Config.GuildId);
                RestTextChannel channel = await guild.GetTextChannelAsync(Config.ChannelId);
                await channel.SendMessageAsync($"<@{Config.RoleId}>\n# Trusted Member Ban\n**{player.DisplayNickname}** has banned **{targetPlayer.DisplayNickname}** from the server.");

            }
#pragma warning restore CS4014
        });
        
        response = "Poll published.";
        return true;
    }

    public string Command { get; } = "voteban";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Votes to ban a player.";
}
