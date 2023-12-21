using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace OriginsBot.Commands;

public class ButtonHandler(DiscordSocketClient discord)
{
    public static readonly ButtonBuilder LeaderboardNextButton = ButtonBuilder.CreateSecondaryButton("Previous Page", "lb_prev", Emote.Parse("<:arrowpinkleft:1186017160410173500>"));
    public static readonly ButtonBuilder LeaderboardPrevButton = ButtonBuilder.CreateSecondaryButton("Next Page", "lb_next", Emote.Parse("<:arrowpink:1186017163404902481>"));
    
    public void Initialize()
    {
        discord.ButtonExecuted += HandleButton;
        discord.ModalSubmitted += HandleModal;
    }

    private async Task HandleButton(SocketMessageComponent component)
    {
        if (component.Data.CustomId == LeaderboardNextButton.CustomId)
            await HandleLeaderboardButton(component, false);
        else if (component.Data.CustomId == LeaderboardPrevButton.CustomId)
            await HandleLeaderboardButton(component, true);
    }
    
    private async Task HandleLeaderboardButton(SocketMessageComponent component, bool prev)
    {
        if (!int.TryParse(component.Message.Embeds.First().Footer?.Text.Replace("Page: ", "") ?? "0", out int page))
            return;

        page += prev ? -1 : 1;

        Embed embed = await LevelingModule.GetLeaderboard(page);

        await component.Message.ModifyAsync(x => x.Embed = embed);
    }

    private Task HandleModal(SocketModal modal)
    {
        return Task.CompletedTask;
    }
}