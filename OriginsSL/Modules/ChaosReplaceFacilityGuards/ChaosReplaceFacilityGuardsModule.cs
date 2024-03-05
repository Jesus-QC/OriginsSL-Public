using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using OriginsSL.Loader;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.ChaosReplaceFacilityGuards;

public class ChaosReplaceFacilityGuardsModule : OriginsModule
{
    private static bool _chaosSpawn;
    
    public override void OnLoaded()
    {
        CursedRoundEventsHandler.WaitingForPlayers += OnWaitingForPlayers;
        CursedPlayerEventsHandler.ChangingRole += OnChangingRole;
    }
    
    private static void OnWaitingForPlayers()
    {
        _chaosSpawn = Random.value < 0.2f; // 20% chance
    }
    
    private static void OnChangingRole(PlayerChangingRoleEventArgs args)
    {
        if (!_chaosSpawn || args.ChangeReason is not (RoleChangeReason.RoundStart or RoleChangeReason.LateJoin))
            return;
        
        if (args.NewRole != RoleTypeId.FacilityGuard)
            return;
        
        args.NewRole = RoleTypeId.ChaosConscript;
    }
}