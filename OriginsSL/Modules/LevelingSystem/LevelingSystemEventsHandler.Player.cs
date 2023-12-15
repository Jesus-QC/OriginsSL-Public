using System.Collections.Generic;
using CursedMod.Events.Arguments.Facility.Doors;
using CursedMod.Events.Arguments.Facility.Warhead;
using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Arguments.SCPs.Scp330;
using CursedMod.Events.Arguments.SCPs.Scp914;
using CursedMod.Events.Handlers;
using CursedMod.Features.Extensions;
using CursedMod.Features.Wrappers.Facility.Doors;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Round;
using PlayerRoles;
using PlayerStatsSystem;

namespace OriginsSL.Modules.LevelingSystem;

public static partial class LevelingSystemEventsHandler
{
    public static void RegisterPlayerEvents()
    {
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
        CursedPlayerEventsHandler.ChangingRole += OnChangingRole;
        CursedItemsEventsHandler.PlayerCancellingThrow += OnPlayerCancellingThrow;
        CursedItemsEventsHandler.PlayerThrowingItem += OnPlayerThrowingItem;
        CursedItemsEventsHandler.PlayerPickedUpItem += OnPlayerPickedUpItem;
        CursedItemsEventsHandler.PlayerUsedItem += OnPlayerUsedItem;
        CursedScp330EventsHandler.PlayerInteractingScp330 += OnPlayerInteractingScp330;
        CursedDoorsEventsHandler.PlayerInteractingDoor += OnPlayerInteractingDoor;
        CursedPlayerEventsHandler.Escaping += OnPlayerEscaping;
        CursedPlayerEventsHandler.EscapingPocketDimension += OnPlayerEscapingPocketDimension;
        CursedPlayerEventsHandler.Disarming += OnPlayerDisarming;
        CursedPlayerEventsHandler.RemovingHandcuff += OnPlayerRemovingHandcuff;
        CursedScp914EventsHandler.PlayerChangingScp914KnobSetting += OnPlayerChangingScp914KnobSetting;
        CursedWarheadEventsHandler.PlayerStartingDetonation += OnPlayerStartingDetonation;
        CursedWarheadEventsHandler.PlayerCancelingDetonation += OnPlayerCancellingDetonation;
        CursedPlayerEventsHandler.Died += OnPlayerDied;
    }

    private static void ClearPlayerCache()
    {
        DoorInteractions.Clear();
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
        OnChangingRole_Dead(args);
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

    private static void OnChangingRole_Dead(PlayerChangingRoleEventArgs args)
    {
        if (args.ChangeReason != RoleChangeReason.Died)
            return;

        args.Player.AddExp(5);
    }
    
    private static readonly LevelingRateLimiter CancelThrowRateLimiter = new (2);
    
    private static void OnPlayerCancellingThrow(PlayerCancellingThrowEventArgs args)
    {
        CancelThrowRateLimiter.AddExpWithCheck(args.Player, 10);
    }
    
    private static readonly LevelingRateLimiter CandyLimiter = new (3);
    
    private static void OnPlayerInteractingScp330(PlayerInteractingScp330EventArgs args)
    {
        CandyLimiter.AddExpWithCheck(args.Player, 15);
    }
    
    private static readonly Dictionary<CursedDoor, HashSet<CursedPlayer>> DoorInteractions = new();
    
    private static readonly LevelingRateLimiter GateLimiter = new (3);
    
    private static void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs args)
    {
        if (!DoorInteractions.ContainsKey(args.Door))
            DoorInteractions.Add(args.Door, []);
        
        if (DoorInteractions[args.Door].Add(args.Player))
            return;
        
        args.Player.AddExp(10);
        GateLimiter.AddExpWithCheck(args.Player, 35);
    }
    
    private static readonly LevelingRateLimiter EscapeLimiter = new (1);

    private static void OnPlayerEscaping(PlayerEscapingEventArgs args)
    {
        EscapeLimiter.AddExpWithCheck(args.Player, 50);
    }
    
    private static void OnPlayerEscapingPocketDimension(PlayerEscapingPocketDimensionEventArgs args)
    {
        args.Player.AddExp(50);
    }
    
    private static readonly LevelingRateLimiter DisarmingLimiter = new (3);
    private static readonly LevelingRateLimiter RemovingHandcuffsLimiter = new (3);

    private static void OnPlayerDisarming(PlayerDisarmingEventArgs args)
    {
        DisarmingLimiter.AddExpWithCheck(args.Player, 50);
    }

    private static void OnPlayerRemovingHandcuff(PlayerRemovingHandcuffEventArgs args)
    {
        RemovingHandcuffsLimiter.AddExpWithCheck(args.Player, 50);
    }
    
    private static readonly LevelingRateLimiter ChangingKnowSettingLimiter = new (3);
    
    private static void OnPlayerChangingScp914KnobSetting(PlayerChangingScp914KnobSettingEventArgs args)
    {
        ChangingKnowSettingLimiter.AddExpWithCheck(args.Player, 10);
    }

    private static readonly LevelingRateLimiter ThrowingItemLimiter = new (3);
    
    private static void OnPlayerThrowingItem(PlayerThrowingItemEventArgs args)
    {
        ThrowingItemLimiter.AddExpWithCheck(args.Player, 25);
    }
    
    private static readonly LevelingRateLimiter PickedUpItemLimiter = new (3);
    
    private static void OnPlayerPickedUpItem(PlayerPickedUpItemEventArgs args)
    {
        PickedUpItemLimiter.AddExpWithCheck(args.Player, 10);
    }

    private static readonly LevelingRateLimiter UsedMedicalItemLimiter = new (3);
    private static readonly LevelingRateLimiter UsedScpItemLimiter = new (3);
    
    private static void OnPlayerUsedItem(PlayerUsedItemEventArgs args)
    {
        if (args.Item.ItemType is ItemType.Medkit or ItemType.Adrenaline or ItemType.Painkillers)
            UsedMedicalItemLimiter.AddExpWithCheck(args.Player, 20);
        if (args.Item.ItemType.IsScpItem())
            UsedScpItemLimiter.AddExpWithCheck(args.Player, 30);
    }

    private static readonly LevelingRateLimiter StartingDetonationLimiter = new (1);
    private static readonly LevelingRateLimiter StoppingDetonationLimiter = new (1);
    
    private static void OnPlayerStartingDetonation(PlayerStartingDetonationEventArgs args)
    {
        StartingDetonationLimiter.AddExpWithCheck(args.Player, 50);
    }

    private static void OnPlayerCancellingDetonation(PlayerCancelingDetonationEventArgs args)
    {
        StoppingDetonationLimiter.AddExpWithCheck(args.Player, 75);
    }

    private static void OnPlayerDied(PlayerDiedEventArgs args)
    {
        if (args.DamageHandlerBase is not AttackerDamageHandler attackerDamageHandler)
            return;
        
        if (attackerDamageHandler.IsSuicide)
            return;
        
        if (attackerDamageHandler.Attacker.Hub.IsHost)
            return;
        
        if (!CursedPlayer.TryGet(attackerDamageHandler.Attacker.Hub, out CursedPlayer attacker))
            return;
        
        attacker.AddExp(5);
    }
}