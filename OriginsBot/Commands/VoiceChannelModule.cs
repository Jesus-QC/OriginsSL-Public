using Discord;
using Discord.Interactions;

namespace OriginsBot.Commands;

public class VoiceChannelModule : InteractionModuleBase<SocketInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("nexus", "Allows you to join the Nexus voice channel.")]
    public async Task NexusCommand([Summary(description:"Member which you'd like to add or remove the role")]IUser member)
    {
        string response = await ToggleRole(member, 1203491135771902003) ? "Added" : "Removed";
        await RespondAsync(response + " nexus role to user " + member.Mention, ephemeral: true);
    }
    
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("spinnyfy", "Allows you to join the Spinnyfy voice channel.")]
    public async Task SpinnyCommand([Summary(description:"Member which you'd like to add or remove the role")]IUser member)
    {
        string response = await ToggleRole(member, 1187426128948899882) ? "Added" : "Removed";
        await RespondAsync(response + " spinnyfy role to user " + member.Mention, ephemeral: true);
    }
    
    private async Task<bool> ToggleRole(IUser member, ulong roleId)
    {
        if (member is not IGuildUser user)
            return false;
        
        IRole role = Context.Guild.GetRole(roleId);

        if (user.RoleIds.Contains(roleId))
        {
            await user.RemoveRoleAsync(role);
            return false;
        }
        
        await user.AddRoleAsync(role);
        return true;
    }
}