using CursedMod.Events.Arguments.Facility.Warhead;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

namespace OriginsSL.Modules.SpectatorFeed;

public class SpectatorFeedModule : OriginsModule
{
    private static readonly Notification[] Notifications = new Notification[5];
    
    private record struct Notification(string Content,float Duration);
    
    private static void AddNotification(string content, float duration = 4f)
    {
        for (int i = 0; i < 4; i++)
        {
            Notifications[i + 1] = Notifications[i];
        }
        
        Notifications[0] = new Notification(content, duration);
    }
    
    public static string GetContent(int n) => Notifications[n].Content;

    public static string GetContentWithAlpha(int n, string alpha)
    {
        string c = GetContent(n);
        return string.IsNullOrEmpty(c) ? c : c.Replace("<a>", alpha);
    }
    
    public override void OnLoaded()
    {
        StaticUnityMethods.OnUpdate += OnUpdate;
        CursedPlayerEventsHandler.Died += OnPlayerDied;
        CursedWarheadEventsHandler.PlayerStartingDetonation += OnPlayerStartingDetonation;
        CursedWarheadEventsHandler.PlayerCancelingDetonation += OnPlayerCancelingDetonation;
        CursedPlayerEventsHandler.Escaping += OnPlayerEscaping;
        CursedPlayerEventsHandler.ChangingRole += OnPlayerChangingRole;
        // TODO: Add detained (ply has detained other)
    }
    
    private static void OnPlayerChangingRole(PlayerChangingRoleEventArgs args)
    {
        if (args.ChangeReason is not RoleChangeReason.Revived || args.NewRole != RoleTypeId.Scp0492)
            return;
        
        if (args.Player.IsHost)
            return;
        
        AddNotification("<color=#EC2121><a>" + args.Player.DisplayNickname + "</color><a><lowercase> has been resurrected</lowercase>");
    }
    
    private static void OnPlayerEscaping(PlayerEscapingEventArgs args)
    {
        if (args.Player.IsHost)
            return;
        
        AddNotification("<color=" + args.Player.CurrentRole.RoleColor.ToHex() + "><a>" + args.Player.DisplayNickname + "</color><a><lowercase> has escaped the facility</lowercase>");
    }

    private static void OnPlayerStartingDetonation(PlayerStartingDetonationEventArgs args)
    {
        if (args.Player.IsHost)
            return;
        
        AddNotification("<color=" + args.Player.CurrentRole.RoleColor.ToHex() + "><a>" + args.Player.DisplayNickname + "</color><a><lowercase> started the warhead</lowercase>");
    }
    
    private static void OnPlayerCancelingDetonation(PlayerCancelingDetonationEventArgs args)
    {
        AddNotification("<color=" + args.Player.CurrentRole.RoleColor.ToHex() + "><a>" + args.Player.DisplayNickname + "</color><a><lowercase> stopped the warhead</lowercase>");
    }
    
    private static void OnPlayerDied(PlayerDiedEventArgs args)
    {
        if (args.DamageHandlerBase is not AttackerDamageHandler attackerDamageHandler 
            || !CursedPlayer.TryGet(attackerDamageHandler.Attacker.Hub, out CursedPlayer attacker) 
            || attacker == args.Player)
            return;
        
        AddNotification("<color=" + args.Player.CurrentRole.RoleColor.ToHex() + "><a>" + args.Player.DisplayNickname + "</color><a> <lowercase>was killed by</lowercase> <color=" + attacker.CurrentRole.RoleColor.ToHex() + "><a>" + attacker.DisplayNickname + "</color>");
    }

    private static void OnUpdate()
    {
        for (int i = 0; i < Notifications.Length; i++)
        {
            Notification notification = Notifications[i];
            float duration = notification.Duration - Time.deltaTime;
            
            if (duration <= 0)
                Notifications[i] = new Notification(string.Empty, 0);
            else
                Notifications[i] = notification with { Duration = duration };
        }
    }
}