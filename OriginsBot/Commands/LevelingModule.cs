using Discord;
using Discord.Interactions;

namespace OriginsBot.Commands;

public class LevelingModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("level", "View yours or another member's level and xp progress.")]
    public async Task LevelingCommand([Summary(description:"Member which you'd like to view progress")]IUser? member = null)
    {
        member ??= Context.User;
        LevelingImageBuilder levelingImageBuilder = new (member.Username, Random.Shared.Next(0, 9999), Random.Shared.Next(0, 9999), Random.Shared.Next(0, 9999), Random.Shared.Next(0, 9999));
        await RespondWithFileAsync(await levelingImageBuilder.BuildAsync(), "level.png");
    }
}