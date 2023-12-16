using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Enums;
using CursedMod.Features.Wrappers.Player;
using MySql.Data.MySqlClient;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PluginAPI.Core;

namespace OriginsSL.Modules.LevelingSystem;

public static partial class LevelingSystemEventsHandler
{
    private static MySqlConnection Connection { get; set; }
    
    public static void InitDatabase()
    {
        Connection = new MySqlConnection($"Server={LevelingSystemModule.Config.DatabaseAddress}; Port={LevelingSystemModule.Config.DatabasePort}; Database={LevelingSystemModule.Config.DatabaseName}; Uid={LevelingSystemModule.Config.UserId}; Pwd={LevelingSystemModule.Config.Password};");
        CreateDatabase();
        CursedRoundEventsHandler.RestartingRound += OnRestartingRoundDatabase;
        CursedPlayerEventsHandler.Connected += OnPlayerConnected;
        CursedPlayerEventsHandler.Disconnected += OnPlayerDisconnected;
    }
    
    private static readonly Dictionary<CursedPlayer, int> PlayerIds = new();
    private static readonly Dictionary<CursedPlayer, int> PlayerExp = new();
    private static readonly Dictionary<CursedPlayer, int> PlayerLevel = new();

    private static void OnRestartingRoundDatabase()
    {
        PlayerIds.Clear();
        PlayerExp.Clear();
        PlayerLevel.Clear();
        ClearPlayerCache();
    }

    private static void OnPlayerConnected(PlayerConnectedEventArgs args)
    {
        if (args.Player.DoNotTrack)
            return;
        
        Authorize(args.Player);
    }

    private static void OnPlayerDisconnected(PlayerDisconnectedEventArgs args)
    {
        PlayerIds.Remove(args.Player);
        PlayerExp.Remove(args.Player);
        PlayerLevel.Remove(args.Player);
    }

    private static async void CreateDatabase()
    {
        try
        {
            MySqlConnection con = (MySqlConnection)Connection.Clone();
            await con.OpenAsync();

            MySqlCommand cmd = new("CREATE TABLE IF NOT EXISTS LevelingSystem(" +
                                   "Id INT AUTO_INCREMENT PRIMARY KEY," +
                                   "Username varchar(255) NOT NULL DEFAULT 'Unknown'," +
                                   "SteamId bigint DEFAULT NULL," +
                                   "DiscordId bigint DEFAULT NULL," +
                                   "NorthWoodId varchar(255) DEFAULT NULL," +
                                   "Experience INT NOT NULL DEFAULT 0," +
                                   "Level INT NOT NULL DEFAULT 0);", con);


            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
    
    private static async Task<(int, int, int)> CreatePlayer(CursedPlayer player)
    {
        MySqlConnection con = (MySqlConnection)Connection.Clone();
        await con.OpenAsync();

        MySqlCommand cmd =
            new(
                "IF (EXISTS(SELECT * FROM LevelingSystem WHERE SteamId = @SteamId or DiscordId = @DiscordId or NorthWoodId = @NorthWoodId)) THEN SELECT Id, Experience FROM LevelingSystem WHERE SteamId = @SteamId or DiscordId = @DiscordId or NorthWoodId = @NorthWoodId; ELSE INSERT INTO LevelingSystem(Username, SteamId, DiscordId, NorthWoodId) VALUES (@Username, @SteamId, @DiscordId, @NorthWoodId); SELECT LAST_INSERT_ID(), 0;END IF;",
                con);
        cmd.Parameters.AddWithValue("@Username", player.RealNickname);
        cmd.Parameters.AddWithValue("@SteamId",
            player.AuthenticationType == AuthenticationType.Steam ? player.RawUserId : null);
        cmd.Parameters.AddWithValue("@DiscordId",
            player.AuthenticationType == AuthenticationType.Discord ? player.RawUserId : null);
        cmd.Parameters.AddWithValue("@NorthWoodId",
            player.AuthenticationType is AuthenticationType.NorthWood or AuthenticationType.Other
                ? player.UserId
                : null);

        using DbDataReader reader = await cmd.ExecuteReaderAsync();

        if (!reader.HasRows)
            return (0, 0, 0);

        await reader.ReadAsync();
        int id = reader.GetInt32(0);
        int exp = reader.GetInt32(1);
        int level = ConvertExpToLevel(exp);
        return (id, exp, level);
    }

    private static async void Authorize(CursedPlayer player)
    {
         (int id, int exp, int level) = await CreatePlayer(player);
         
         if (id == 0)
             return;
         
         PlayerIds.Add(player, id);
         PlayerExp.Add(player, exp);
         PlayerLevel.Add(player, level);
         
         OnAuthenticated(player);
    }

    public static async void AddExp(this CursedPlayer player, int exp)
    {
        if (!PlayerIds.TryGetValue(player, out int plyId))
            return;
        
        PlayerExp[player] += exp;
        int level = ConvertExpToLevel(PlayerExp[player]);
        PlayerLevel[player] = level;
        
        MySqlConnection con = (MySqlConnection)Connection.Clone();
        await con.OpenAsync();

        MySqlCommand cmd = new("UPDATE LevelingSystem SET Experience = Experience + @Exp, Level = @Level WHERE Id=@Id", con);
        cmd.Parameters.AddWithValue("@Id", plyId);
        cmd.Parameters.AddWithValue("@Exp", exp);
        cmd.Parameters.AddWithValue("@Level", level);
   
        await cmd.ExecuteNonQueryAsync();
        
        if (!DisplayRendererModule.TryGetDisplayBuilder(player, out CursedDisplayBuilder builder))
            return;
        
        builder.AddNotification($"<color=#E5DEA2>⌈ + {exp}</color> <color=#AFC0B8>E</color><lowercase><color=#94B1C3>x</color><color=#79A2CE>p</color></lowercase> <color=#4384E4>⌋</color>");
    }
    
    public static bool TryGetId(this CursedPlayer player, out int id) => PlayerIds.TryGetValue(player, out id);

    // Converts Exp to Level:
    // 1000 Exp = 1 Level
    // When reached 10 Levels, it will be 2000 Exp = 1 Level
    // When reached 20 Levels, it will be 3000 Exp = 1 Level
    // And so on...
    private static int ConvertExpToLevel(int exp)
    {
        const int levelChunkTo = 10;
        const int startingExpPerLevel = 1000;
        
        int index = 1;
        while (true)
        {
            int neededExp = startingExpPerLevel * levelChunkTo * index;
			
            if (exp <= neededExp)
                return levelChunkTo * (index - 1) + exp / (startingExpPerLevel * index);
                
            exp -= neededExp;
            index++;
        }
    }
}