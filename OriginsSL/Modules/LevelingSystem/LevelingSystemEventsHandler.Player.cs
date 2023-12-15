using System.Collections.Generic;
using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Round;
using PlayerRoles;

namespace OriginsSL.Modules.LevelingSystem;

public static partial class LevelingSystemEventsHandler
{
    public static void RegisterPlayerEvents()
    {
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
        CursedPlayerEventsHandler.ChangingRole += OnChangingRole;
        CursedItemsEventsHandler.PlayerCancellingThrow += OnPlayerCancellingThrow;
    }
    
    private static readonly LevelingRateLimiter JoinRoundRateLimiter = new (1);
    
    private static void OnAuthenticated(CursedPlayer player)
    {
        if (CursedRound.HasEnded)
            return;

        JoinRoundRateLimiter.AddExpWithCheck(player, 50);
    }
    
    private static void OnRoundEnded()
    {
        foreach (CursedPlayer player in CursedPlayer.Collection)
            player.AddExp(50);
    }
    
    private static void OnChangingRole(PlayerChangingRoleEventArgs args)
    {
        OnChangingRole_Respawning(args);
        OnChangingRole_SCP(args);
    }

    private static void OnChangingRole_Respawning(PlayerChangingRoleEventArgs args)
    {
        if (args.ChangeReason != RoleChangeReason.Respawn)
            return;
        
        args.Player.AddExp(20);
    }

    private static void OnChangingRole_SCP(PlayerChangingRoleEventArgs args)
    {
        if (args.ChangeReason != RoleChangeReason.RoundStart || args.NewRole.GetTeam() != Team.SCPs)
            return;
        
        args.Player.AddExp(20);
    }
    
    private static readonly LevelingRateLimiter CancelThrowRateLimiter = new (3);
    
    private static void OnPlayerCancellingThrow(PlayerCancellingThrowEventArgs args)
    {
        CancelThrowRateLimiter.AddExpWithCheck(args.Player, 10);
    }
}