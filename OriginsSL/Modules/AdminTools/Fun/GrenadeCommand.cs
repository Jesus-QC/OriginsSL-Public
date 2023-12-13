using System;
using System.Collections.Generic;
using CommandSystem;
using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Inventory.Items.ThrowableProjectiles;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Features.Commands;
using NWAPIPermissionSystem;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class GrenadeCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.fun.grenade") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "Not enough arguments.";
            return false;
        }

        List<CursedPlayer> players = CursedCommandUtils.GetPlayers(arguments.At(0));

        if (players.Count is 0)
        {
            response = "Players not found.";
            return false;
        }

        float fuseTime = -1;

        if (arguments.Count > 2 && !float.TryParse(arguments.At(2), out fuseTime))
        {
            response = "Couldn't parse the fuse time.";
            return false;
        }

        switch (arguments.At(1))
        {
            case "0" or "Explosion":
                foreach (CursedPlayer player in players)
                {
                    if(player.IsDead) 
                        continue;
                    
                    ((CursedThrowableItem)CursedItem.Create(ItemType.GrenadeHE)).SpawnCharged(player.Position, fuseTime);
                }
                break;
            case "1" or "Flash":
                foreach (CursedPlayer player in players)
                {
                    if(player.IsDead) 
                        continue;
                    
                    ((CursedThrowableItem)CursedItem.Create(ItemType.GrenadeFlash)).SpawnCharged(player.Position, fuseTime);
                }
                break;
            case "2" or "SCP018" or "018":
                foreach (CursedPlayer player in players)
                {
                    if(player.IsDead) 
                        continue;
                    
                    ((CursedThrowableItem)CursedItem.Create(ItemType.SCP018)).SpawnCharged(player.Position, fuseTime);
                }
                break;
            
            default:
                response = "Grenade not found, use Explosion, Flash or SCP018";
                return false;
        }
        
        response = "Spawned grenade.";
        return true;
    }

    public string Command { get; } = "grenade";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Spawns a grenade";
    public string[] Usage { get; } = { "%player%", "Grenade Type", "Fuse Time" };
}