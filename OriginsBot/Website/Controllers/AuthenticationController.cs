using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using OriginsBot.Security;

namespace OriginsBot.Website.Controllers;

public class AuthenticationController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("/signin")]
    public async Task<IActionResult> LogIn(string discordId)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Challenge("Steam");
        
        Console.WriteLine(User.Identity.Name + " got authenticated via discord.");
        await SyncUser(discordId, User.Claims.First(x => x.Type is ClaimTypes.NameIdentifier).Value);
        return Content("You have synced your accounts. You can close this tab.");
    }

    private static async Task SyncUser(string discordId, string steamId)
    {
        try
        {
            string id = AesEncryptor.Decrypt(discordId);
            Console.WriteLine(id + " syncing accounts.");
            
            MySqlConnection connection = (MySqlConnection)DatabaseHandler.Connection.Clone();
            await connection.OpenAsync();
        
            MySqlCommand cmd = new("UPDATE LevelingSystem SET DiscordId=@DiscordId WHERE SteamId=@SteamId", connection);
            cmd.Parameters.AddWithValue("@DiscordId", id);
            cmd.Parameters.AddWithValue("@SteamId", steamId[37..]);
            
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    } 
}