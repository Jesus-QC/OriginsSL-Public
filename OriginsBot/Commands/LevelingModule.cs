using System.Data.Common;
using System.Web;
using Discord;
using Discord.Interactions;
using MySql.Data.MySqlClient;
using OriginsBot.Security;

namespace OriginsBot.Commands;

public class LevelingModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("level", "View yours or another member's level and xp progress.")]
    public async Task LevelingCommand([Summary(description:"Member which you'd like to view progress")]IUser? member = null)
    {
        member ??= Context.User;

        (int level, int exp, int rank) = await GetLevelExpAndRank(member.Id);
        if (level == 0 && exp == 0 && rank == 0)
        {
            await RespondAsync("There has been an error while fetching the user.\n**Make sure the account is synced with the `/sync` command.**");
            return;
        }

        (int actualExp, int neededExp) = GetExpProgress(exp);
        
        LevelingImageBuilder levelingImageBuilder = new (member.Username, level, actualExp, neededExp, rank);
        await RespondWithFileAsync(await levelingImageBuilder.BuildAsync(), "level.png");
    }
    
    [SlashCommand("leaderboard", "View the leveling leaderboard.")]
    public async Task LeaderboardCommand()
    {
        await RespondAsync(embed: await GetLeaderboard(0), components: new ComponentBuilder().AddRow(
            new ActionRowBuilder()
                .WithButton(ButtonHandler.LeaderboardPrevButton)
                .WithButton(ButtonHandler.LeaderboardNextButton)
            ).Build());
    }

    public static async Task<Embed> GetLeaderboard(int offset)
    {
        const int pageSize = 10;
        
        EmbedBuilder builder = new()
        {
            Title = "Origins SL Leaderboard",
        };

        string usernames = string.Empty, experiences = string.Empty, levels = string.Empty; 
        
        await using MySqlConnection connection = (MySqlConnection)DatabaseHandler.Connection.Clone();
        await connection.OpenAsync();
        
        await using MySqlCommand cmd = new($"SELECT Username, Level, Experience FROM LevelingSystem ORDER BY Experience DESC LIMIT {pageSize} OFFSET @Offset", connection);
        cmd.Parameters.AddWithValue("@Offset", offset * pageSize);
        
        await using DbDataReader reader = await cmd.ExecuteReaderAsync();

        int rank = offset * pageSize + 1;
        while (await reader.ReadAsync())
        {
            string lbUsername = reader.GetString(0);
            int lbLevel = reader.GetInt32(1);
            int lbExp = reader.GetInt32(2);
            
            string rankEmoji = rank switch
            {
                1 => "<:trophy1:1186009506287722516>",
                2 => "<:trophy2:1186009504719056987>",
                3 => "<:trophy3:1186009502047285278>",
                _ => rank % 2 == 0 ? "<:player:1186012196552056913>" : "<:playerselected:1186012194098385116>",
            };
            
            usernames += $"`{rank.ToString().PadLeft(4, '0')}` | {rankEmoji} | {lbUsername}\n";
            levels += $"`{lbLevel}`\n";
            experiences += $"`{lbExp}`\n";
            rank++;
        }
        
        builder.AddField("Username", usernames, true);
        builder.AddField("Level", levels, true);
        builder.AddField("Experience", experiences, true);

        builder.Footer = new EmbedFooterBuilder().WithText("Page: " + ++offset);

        return builder.Build();
    }
    
    private static (int, int) GetExpProgress(int exp)
    {
        const int levelChunkTo = 10;
        const int startingExpPerLevel = 1000;
        
        int index = 1;
        while (true)
        {
            int neededExp = startingExpPerLevel * levelChunkTo * index;

            if (exp <= neededExp)
            {
                int chunk = index * 1000;
                return (exp % chunk, chunk);
            }
                
            exp -= neededExp;
            index++;
        }
    }
    
    private static async Task<(int, int, int)> GetLevelExpAndRank(ulong discordId)
    {
        await using MySqlConnection connection = (MySqlConnection)DatabaseHandler.Connection.Clone();
        await connection.OpenAsync();
        
        await using MySqlCommand cmd = new("IF (EXISTS(SELECT * FROM LevelingSystem WHERE DiscordId=@DiscordId)) THEN WITH cte AS ( SELECT Level, Experience, DiscordId, RANK() OVER (ORDER BY Experience DESC) AS rank FROM LevelingSystem) SELECT Level, Experience, rank FROM cte WHERE DiscordId=@DiscordId;ELSE SELECT 0,0,0;END IF;", connection);
        cmd.Parameters.AddWithValue("@DiscordId", discordId);
        
        await using DbDataReader reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows)
            return (0, 0, 0);

        await reader.ReadAsync();
        int level = reader.GetInt32(0);
        int exp = reader.GetInt32(1);
        int rank = reader.GetInt32(2);
        return (level, exp, rank);
    }

    private static readonly HashSet<ulong> AlreadySyncedIds = [];
    
    [SlashCommand("sync", "Sync your discord account with your steam account.")]
    public async Task SyncCommand()
    {
        if (await HasAlreadySyncedAccount(Context.User.Id))
        {
            await RespondAsync("You have already synced your accounts. To desync them use the /desync command.", ephemeral: true);
            return;
        }
        
        await RespondAsync("# Sync Steam Account\nPlease go to the link below to sync your steam account with your discord account.\nhttps://sync.origins.ink/signin?discordId=" + HttpUtility.UrlEncode(AesEncryptor.Encrypt(Context.User.Id.ToString())), ephemeral: true);
    }
    
    [SlashCommand("desync", "Desync your discord account with your steam account.")]
    public async Task DeSyncCommand()
    {
        if (!await HasAlreadySyncedAccount(Context.User.Id))
        {
            await RespondAsync("You haven't already synced your accounts. To sync them use the /sync command.", ephemeral: true);
            return;
        }

        try
        {
            await DeSyncAccount(Context.User.Id);
            await RespondAsync("Successfully desynced your discord and steam accounts.", ephemeral: true);
        }
        catch (Exception e)
        {
            await RespondAsync("<@430960270433845249>\n```cs\n" + e + "\n```", ephemeral: true);
        }
    }

    private static async Task<bool> HasAlreadySyncedAccount(ulong id)
    {
        if (AlreadySyncedIds.Contains(id))
            return true;

        await using MySqlConnection connection = (MySqlConnection)DatabaseHandler.Connection.Clone();
        await connection.OpenAsync();
        
        await using MySqlCommand cmd = new("SELECT EXISTS(SELECT * FROM LevelingSystem WHERE DiscordId=@DiscordId)", connection);
        cmd.Parameters.AddWithValue("@DiscordId", id);

        object? t = await cmd.ExecuteScalarAsync();

        if (t is not 1)
            return false;

        AlreadySyncedIds.Add(id);
        return true;
    }
    
    private static async Task DeSyncAccount(ulong id)
    {
        await using MySqlConnection connection = (MySqlConnection)DatabaseHandler.Connection.Clone();
        await connection.OpenAsync();
        
        await using MySqlCommand cmd = new("UPDATE LevelingSystem SET DiscordId=NULL WHERE DiscordId=@DiscordId", connection);
        cmd.Parameters.AddWithValue("@DiscordId", id);

        await cmd.ExecuteNonQueryAsync();
        AlreadySyncedIds.Remove(id);
    }
}