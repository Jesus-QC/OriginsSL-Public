using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;
using OriginsSL.Features.Commands;
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

        SubclassBase subclass = null;
        string args = arguments.At(1).ToLower();
        
        foreach (KeyValuePair<RoleTypeId, SubclassBase[]> roleSubclass in SubclassManager.AvailableSubclasses)
        {
            foreach (SubclassBase availableSubclass in roleSubclass.Value)
            {
                if (availableSubclass.CodeName.ToLower() != args) 
                    continue;
                
                subclass = availableSubclass;
                break;
            }
            
            if (subclass != null)
                break;
        }

        if (subclass == null)
        {
            response = "Subclass not found. Available subclasses:";
            
            foreach (KeyValuePair<RoleTypeId, SubclassBase[]> roleSubclass in SubclassManager.AvailableSubclasses)
            {
                response += $"\n{roleSubclass.Key}:";
                response = roleSubclass.Value.Aggregate(response, (current, availableSubclass) => current + $"\n\t{availableSubclass.CodeName}");
            }
            return false;
        }
        
        foreach (CursedPlayer player in players)
        {
            if (player.TryGetSubclass(out SubclassBase oldSubclass) && subclass.GetType() == oldSubclass.GetType())
                continue;
            
            player.ForceSubclass(Activator.CreateInstance(subclass.GetType()) as SubclassBase);
        }

        response = $"Done for {players.Count} players";
        return true;
    }

    public string Command { get; } = "setsubclass";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Sets a player certain subclass.";
    public string[] Usage { get; } = ["%player%", "subclass"];
}