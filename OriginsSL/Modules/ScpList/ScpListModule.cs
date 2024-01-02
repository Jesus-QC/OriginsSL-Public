using System.Collections.Generic;
using System.Linq;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Loader;
using PlayerRoles;

namespace OriginsSL.Modules.ScpList;

public class ScpListModule : OriginsModule
{
    private static readonly List<string> SavedMessages = [];
    
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.ChangingRole += OnPlayerChangingRole;
    }

    public static string GetContent(int n) 
        => SavedMessages.Count > n ? SavedMessages[n] : string.Empty;

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

        SavedMessages.Clear();

        bool doctorFound = newRole == RoleTypeId.Scp049;
        
        if (newRole != RoleTypeId.None)
        {
            if (newRole is not (RoleTypeId.Scp0492 or RoleTypeId.ZombieFlamingo))
                SavedMessages.Add(Format(newRole, zombieCount));
            else
                zombieCount++;
            
            i++;
        }
        
        foreach (CursedPlayer player in scpList)
        {
            if (i >= 8)
                break;
            
            if (ply == player)
                continue;

            switch (player.Role)
            {
                case RoleTypeId.Scp0492 or RoleTypeId.ZombieFlamingo:
                    zombieCount++;
                    continue;
                case RoleTypeId.Scp049:
                    doctorFound = true;
                    break;
            }

            SavedMessages.Add(Format(player.Role, zombieCount));
            i++;
        }

        if (!doctorFound && zombieCount > 0)
        {
            SavedMessages.Add("Zombies: " + zombieCount);
        }
    }

    private static string Format(RoleTypeId role, int zombieCount)
    {
        return role switch
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