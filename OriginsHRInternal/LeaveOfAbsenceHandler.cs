using System.Globalization;
using Discord;
using Discord.WebSocket;

namespace OriginsHRInternal;

public static class LeaveOfAbsenceHandler
{
    private static SocketGuild _guildId = null!;
    private static DiscordSocketClient _client = null!;
    private static SocketTextChannel _internalChannelId = null!;
    private static SocketTextChannel _publicChannelId = null!;
    private static SocketTextChannel _notificationChannelId = null!;
    private static ulong _roleId;
    
    public static async Task InitializeAsync(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        
        _guildId = _client.GetGuild(configuration.GetValue<ulong>("guild"));
        
        await _guildId.DownloadUsersAsync();
        
        _internalChannelId = _guildId.GetTextChannel(configuration.GetValue<ulong>("loa_internal"));
        _publicChannelId = _guildId.GetTextChannel(configuration.GetValue<ulong>("loa_public"));
        _notificationChannelId = _guildId.GetTextChannel(configuration.GetValue<ulong>("loa_notification"));
        _roleId = configuration.GetValue<ulong>("loa_role");
        
        Task.Run(RunTimer);
    }
    
    public static async Task SendLeaveOfAbsenceAsync(SocketUser user, string reason, string startDate, string endDate)
    {
        EmbedBuilder embed = new EmbedBuilder
            {
                Title = "Origins Leave of Absence",
                Color = Color.DarkRed
            }
            .AddField("Staff Name", user.Mention, false)
            .AddField("Reason", reason, true)
            .AddField("Start Date", startDate, true)
            .AddField("End Date", endDate, true)
            .WithThumbnailUrl(user.GetAvatarUrl(ImageFormat.Png));
        
        await _internalChannelId.SendMessageAsync(embed: embed.Build());
        await RefreshAsync();
    }

    public static bool HasRole(SocketGuildUser user) => user.Roles.Any(x => x.Id == _roleId);
    
    public static async Task CancelLeaveOfAbsenceAsync(SocketGuildUser user)
    {
        await user.RemoveRoleAsync(_roleId);
        
        foreach (IMessage message in await _internalChannelId.GetMessagesAsync().FlattenAsync())
            await RemoveLoAWithMentionAsync(message, user.Id.ToString());
        foreach (IMessage message in await _publicChannelId.GetMessagesAsync().FlattenAsync())
            await RemoveLoAWithMentionAsync(message, user.Id.ToString());

        return;

        async Task RemoveLoAWithMentionAsync(IMessage message, string id)
        {
            if (message.Author.Id != _client.CurrentUser.Id)
                return;
            
            if (message.Embeds.Count == 0)
                return;
            
            IEmbed embed = message.Embeds.First();
            
            string userId = embed.Fields.First(x => x.Name == "Staff Name").Value[2..^1];
            
            if (userId != id)
                return;
            
            await message.DeleteAsync();
            await _notificationChannelId.SendMessageAsync($"{user.Mention} your loa has ended!", embed: embed.ToEmbedBuilder().WithTitle("LoA Canceled").Build());
        }
    }

    public static async Task RefreshAsync()
    {
        await CheckInternalLeaveOfAbsence();
        await CheckPublicLeaveOfAbsence();
    }
    
    private static async Task RunTimer()
    {
        await Task.Delay(10000);
        Console.WriteLine("Running Leave of Absence Handler.");
        while (true)
        {
            await RefreshAsync();
            await Task.Delay(300000);
        }
    }
    
    private static async Task CheckInternalLeaveOfAbsence()
    {
        // Check for leave of absence
        foreach (IMessage message in await _internalChannelId.GetMessagesAsync().FlattenAsync())
        {
            if (message.Author.Id != _client.CurrentUser.Id)
                continue;
            
            if (message.Embeds.Count == 0)
                continue;
            
            IEmbed embed = message.Embeds.First();

            string starts = embed.Fields.First(x => x.Name == "Start Date").Value;
            
            if (!DateTime.TryParseExact(starts, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime startDate))
                continue;
            
            if (DateTime.Now < startDate)
                continue;

            string userId = embed.Fields.First(x => x.Name == "Staff Name").Value[2..^1];
            SocketGuildUser user = _guildId.GetUser(ulong.Parse(userId));
            await user.AddRoleAsync(_roleId);

            await _publicChannelId.SendMessageAsync(embed: embed.ToEmbedBuilder().Build());
            await message.DeleteAsync();
        }
        
        Console.WriteLine("Checked internal leaves of absence at " + DateTime.Now.ToString("G"));
    }

    private static async Task CheckPublicLeaveOfAbsence()
    {
        try
        {
            // Check for leave of absence
            foreach (IMessage message in await _publicChannelId.GetMessagesAsync().FlattenAsync())
            {
                if (message.Author.Id != _client.CurrentUser.Id)
                    continue;

                if (message.Embeds.Count == 0)
                    continue;

                IEmbed embed = message.Embeds.First();

                string ends = embed.Fields.First(x => x.Name == "End Date").Value;

                if (!DateTime.TryParseExact(ends, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal, out DateTime endDate))
                    continue;

                if (DateTime.Now < endDate)
                    continue;

                string userId = embed.Fields.First(x => x.Name == "Staff Name").Value[2..^1];
                
                Console.WriteLine("Checking user id: " + ulong.Parse(userId) + " and user:");
                
                SocketGuildUser user = _guildId.GetUser(ulong.Parse(userId));
                
                Console.WriteLine(userId + "  " + _guildId + " " + (user is null));
                await user.RemoveRoleAsync(_roleId);

                await message.DeleteAsync();
                await _notificationChannelId.SendMessageAsync($"{user.Mention} your loa has ended!",
                    embed: embed.ToEmbedBuilder().WithTitle("LoA Ended").Build());
            }

            Console.WriteLine("Checked public leaves of absence at " + DateTime.Now.ToString("G"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}