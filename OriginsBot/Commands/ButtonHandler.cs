using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace OriginsBot.Commands;

public class ButtonHandler : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("lb_prev")]
    public async Task UpdateMessage()
    {
        try
        {
            string? page = Context.Interaction.Message.Embeds.First().Footer?.Text.Replace("Page: ", string.Empty);

            if (string.IsNullOrEmpty(page) || !int.TryParse(page, out int pageNumber))
                return;

            pageNumber--;

            if (pageNumber == 0)
            {
                await RespondAsync("You are already on the first page.", ephemeral: true);
                return;
            }

            Embed embed = await LevelingModule.GetLeaderboard(pageNumber, string.Empty, 0, 0, 0);

            await Context.Interaction.UpdateAsync(msg => { msg.Embed = embed; });
        }
        catch (Exception e)
        {
        }
    }
    
    [ComponentInteraction("lb_next")]
    public async Task UpdateMessageNext()
    {
        try
        {
            string? page = Context.Interaction.Message.Embeds.First().Footer?.Text.Replace("Page: ", string.Empty);

            if (string.IsNullOrEmpty(page) || !int.TryParse(page, out int pageNumber))
                return;

            pageNumber++;

            Embed embed = await LevelingModule.GetLeaderboard(pageNumber, string.Empty, 0, 0, 0);

            await Context.Interaction.UpdateAsync(msg => { msg.Embed = embed; });
        }
        catch (Exception e)
        {
        }
    }
}