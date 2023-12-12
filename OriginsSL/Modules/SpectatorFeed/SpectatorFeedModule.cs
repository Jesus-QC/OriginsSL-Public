using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
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
            Notifications[i + 1] = Notifications[i];
        
        Notifications[0] = new Notification(content, duration);
    }
    
    public static string GetContent(int n) => Notifications[n].Content;
    
    public override void OnLoaded()
    {
        StaticUnityMethods.OnUpdate += OnUpdate;
        CursedPlayerEventsHandler.Dying += OnPlayerDying;
        // TODO: Add generator engaged
        // TODO: Add warhead started
        // TODO: Add escaped
        // TODO: Add detained (handcuffed)
    }

    private static void OnPlayerDying(PlayerDyingEventArgs args)
    {
        CursedPlayer attacker = args.Attacker;
        
        if (attacker == null)
            return;
        
        AddNotification("<color=" + args.Player.CurrentRole.RoleColor.ToHex() + ">" + args.Player.DisplayNickname + "</color> <lowercase>was killed by</lowercase> <color=" + attacker.CurrentRole.RoleColor.ToHex() + ">" + attacker.DisplayNickname + "</color>");
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