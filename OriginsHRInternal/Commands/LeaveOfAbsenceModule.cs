using System.Globalization;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace OriginsHRInternal.Commands;

[Group("loa", "Leave of Absence commands.")]
public class LeaveOfAbsenceModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("submit", "Submits a loa.")]
    public async Task SubmitLoa([Discord.Interactions.Summary("startdate", "The LoA start date. Use a date in the ISO format (30-05-2024).")]string startDate, [Discord.Interactions.Summary("enddate", "The LoA end date. Use a date in the ISO format (30-05-2024).")]string endDate, [Discord.Interactions.Summary(description:"The reason of the LoA.")]string reason)
    {
        if (!DateTime.TryParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime start))
        {
            await RespondAsync("Couldn't parse the start date. Please use the format dd-MM-yyyy.", ephemeral: true);
            return;
        }
        
        if (!DateTime.TryParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime end))
        {
            await RespondAsync("Couldn't parse the end date. Please use the format dd-MM-yyyy.", ephemeral: true);
            return;
        }
        
        if (start > end)
        {
            await RespondAsync("The start date cannot be after the end date.", ephemeral: true);
            return;
        }
        
        if (start < DateTime.Today)
        {
            await RespondAsync("The start date cannot be in the past.", ephemeral: true);
            return;
        }
        
        if (Context.User is not SocketGuildUser user)
        {
            await RespondAsync("You are not a member of the server.", ephemeral: true);
            return;
        }

        if (LeaveOfAbsenceHandler.HasRole(user))
        {
            await RespondAsync("You already have a leave of absence.", ephemeral: true);
            return;
        }
        
        await LeaveOfAbsenceHandler.SendLeaveOfAbsenceAsync(Context.User, reason, startDate, endDate);
        await RespondAsync("You have submitted a leave of absence.", ephemeral: true);
    }
    
    [SlashCommand("cancel", "Cancels a loa.")]
    public async Task CancelLoa()
    {
        if (Context.User is not SocketGuildUser user)
        {
            await RespondAsync("You are not a member of the server.", ephemeral: true);
            return;
        }
        
        if (!LeaveOfAbsenceHandler.HasRole(user))
        {
            await RespondAsync("You don't have a leave of absence.", ephemeral: true);
            return;
        }
        
        await LeaveOfAbsenceHandler.CancelLeaveOfAbsenceAsync(user);
        await RespondAsync("You have cancelled a leave of absence.", ephemeral: true);
    }
    
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("refresh", "refreshes loas.")]
    public async Task RefreshLoa()
    {
        await LeaveOfAbsenceHandler.RefreshAsync();
        await RespondAsync("Refreshed leave of absences.", ephemeral: true);
    }
}