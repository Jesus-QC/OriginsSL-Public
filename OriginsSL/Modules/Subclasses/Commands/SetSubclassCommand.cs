using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;
using OriginsSL.Features.Commands;
using OriginsSL.Modules.AdminTools.Fun.Components;
using PlayerRoles;

namespace OriginsSL.Modules.Subclasses.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class SetSubclassCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.subclasses.set") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        if (arguments.Count == 0)
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

        ISubclass subclass = null;

        foreach (KeyValuePair<RoleTypeId, ISubclass[]> roleSubclass in SubclassManager.AvailableSubclasses)
        {
            foreach (ISubclass availableSubclass in roleSubclass.Value)
            {
                if (availableSubclass.CodeName == arguments.At(1).ToLower())
                    subclass = availableSubclass;
                break;
            }
            
            if (subclass != null)
                break;
        }

        if (subclass == null)
        {
            response = "Subclass not found. Available subclasses:";
            
            foreach (KeyValuePair<RoleTypeId, ISubclass[]> roleSubclass in SubclassManager.AvailableSubclasses)
            {
                response += $"\n{roleSubclass.Key}:";
                response = roleSubclass.Value.Aggregate(response, (current, availableSubclass) => current + $"\n\t{availableSubclass.CodeName}");
            }
            return false;
        }
        
        foreach (CursedPlayer player in players)
        {
            player.SetSubclass(subclass);
        }

        response = $"Done for {players.Count} players";
        return true;
    }

    public string Command { get; } = "setsubclass";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Sets a player certain subclass.";
    public string[] Usage { get; } = {"%player%"};
}