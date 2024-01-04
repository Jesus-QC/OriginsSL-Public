using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;
using OriginsSL.Features.Commands;

namespace OriginsSL.Modules.CustomItems.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class SetCustomItemCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.customitems.set") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        if (arguments.Count < 2)
        {
            response = "Not enough arguments";
            return false;
        }
        
        List<CursedPlayer> players = CursedCommandUtils.GetPlayers(arguments.At(0));

        if (players.Count is 0)
        {
            response = "Players not found.";
            return false;
        }

        ItemType itemType = ItemType.None;
        ICustomItem item = null;
        string args = arguments.At(1).ToLower();
        
        foreach (KeyValuePair<ItemType, ICustomItem[]> customItem in CustomItemManager.NaturallySpawnedItems)
        {
            foreach (ICustomItem availableCustomItem in customItem.Value)
            {
                if (availableCustomItem.CodeName.ToLower() != args) 
                    continue;

                itemType = customItem.Key;
                item = availableCustomItem;
                break;
            }
            
            if (item != null)
                break;
        }

        if (itemType is ItemType.None || item == null)
        {
            response = "CustomItem not found. Available custom items:";
            
            foreach (KeyValuePair<ItemType, ICustomItem[]> customItems in CustomItemManager.NaturallySpawnedItems)
            {
                response += $"\n{customItems.Key}:";
                response = customItems.Value.Aggregate(response, (current, availableSubclass) => current + $"\n\t{availableSubclass.CodeName}");
            }
            return false;
        }
        
        foreach (CursedPlayer player in players)
        {
            ushort serial = player.AddItem(itemType).Serial;
            CustomItemManager.AlreadyRegisteredSerials.Add(serial);
            CustomItemManager.RegisterCustomItem(serial, Activator.CreateInstance(item.GetType()) as ICustomItem);
        }

        response = $"Done for {players.Count} players";
        return true;
    }

    public string Command { get; } = "addcustomitem";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Adds a player certain customitem.";
    public string[] Usage { get; } = ["%player%", "custom item"];
}