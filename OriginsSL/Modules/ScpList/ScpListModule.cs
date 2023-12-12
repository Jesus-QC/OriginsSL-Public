using System.Linq;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles;

namespace OriginsSL.Modules.ScpList;

public class ScpListModule : OriginsModule
{
    private static readonly string[] SavedMessages = new string[8];
    
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.ChangingRole += OnPlayerChangingRole;
    }
    
    public static string GetContent(int n) => SavedMessages[n];
    
    private static void OnPlayerChangingRole(PlayerChangingRoleEventArgs args)
    {
        if (args.NewRole.GetTeam() == Team.SCPs)
        {
            Refresh(args.Player, args.NewRole);
            return;
        }
        
        if (!args.Player.IsScp)
            return;
        
        Refresh(args.Player);
    }

    private static void Refresh(CursedPlayer ply, RoleTypeId newRole = RoleTypeId.None)
    {
        IOrderedEnumerable<CursedPlayer> scpList = CursedPlayer.ScpList.OrderByDescending(x => x.Role);
        
        int zombieCount = 0;
        int i = 0;

        if (newRole != RoleTypeId.None)
        {
            i++;
            if (newRole == RoleTypeId.Scp0492)
                zombieCount++;
            else
                SavedMessages[0] = Format(ply, zombieCount);
        }
        
        foreach (CursedPlayer player in scpList)
        {
            if (i >= 8)
                break;
            
            if (player.Role == RoleTypeId.Scp0492)
            {
                zombieCount++;
                continue;
            }
            
            if (ply == player && newRole is RoleTypeId.None)
                continue;
            
            SavedMessages[i] = Format(player, zombieCount);
            i++;
        }
    }

    private static string Format(CursedPlayer ply, int zombieCount)
    {
        return ply.Role switch
        {
            RoleTypeId.Scp049 => zombieCount == 0 ? "SCP 049" : $"SCP 049 (Zombies: {zombieCount})",
            RoleTypeId.Scp079 => "SCP 079",
            RoleTypeId.Scp096 => "SCP 096",
            RoleTypeId.Scp106 => "SCP 106",
            RoleTypeId.Scp173 => "SCP 173",
            RoleTypeId.Scp939 => "SCP 939",
            RoleTypeId.Scp3114 => "SCP 3114",
            _ => string.Empty
        };
    }
}