using Discord;
using Discord.WebSocket;

namespace OriginsBot.Commands;

public class ButtonHandler(DiscordSocketClient discord)
{
    public static readonly ButtonBuilder LeaderboardPrevButton = ButtonBuilder.CreateSecondaryButton("Previous Page", "lb_prev", Emote.Parse("<:arrowpinkleft:1186017160410173500>"));
    public static readonly ButtonBuilder LeaderboardNextButton = ButtonBuilder.CreateSecondaryButton("Next Page", "lb_next", Emote.Parse("<:arrowpink:1186017163404902481>"));
    
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
        {
            await component.RespondAsync("There has been an error while fetching the leaderboard.");
            return;
        }

        page--;
        page += prev ? -1 : 1;

        page = Math.Max(0, page); // Clamp value to 0
        
        Embed embed = await LevelingModule.GetLeaderboard(page);
        
        await component.Message.ModifyAsync(x => x.Embed = embed);
        
        try
        {
            await component.RespondAsync();
        }
        catch
        {
            // It marks the message as handled and throws an exception
            // Which means that the message content can't be null
            // So we just ignore it
        }
    }

    private Task HandleModal(SocketModal modal)
    {
        return Task.CompletedTask;
    }
}