using System;
using CommandSystem;
using CursedMod.Features.Wrappers.Inventory.Pickups;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class SpawnItemCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.fun.spawnitem") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "Not enough arguments.";
            return false;
        }

        if(!Enum.TryParse(arguments.At(0), out ItemType itemType))
        {
            response = "Item type not found. Available types:\n" + string.Join(", ", Enum.GetNames(typeof(ItemType)));
            return false;
        }

        if (arguments.Count == 2 && int.TryParse(arguments.At(1), out int amount))
        {
            for (; amount > 0; amount--)
            {
                CursedPickup.Create(itemType, ply.Position);
            }
                    
            response = "Spawned items.";
            return true;
        }
        
        CursedPickup.Create(itemType, ply.Position);
        response = "Spawned items.";
        return true;
    }

    public string Command { get; } = "spawnitem";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Spawns a desired item";
    public string[] Usage { get; } = { "Item Type", "<amount>" };
}