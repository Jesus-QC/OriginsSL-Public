using System;
using System.Collections.Generic;
using System.Linq;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Round;
using PlayerStatsSystem;

namespace OriginsSL.Modules.EndScreen;

public class EndScreenModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Connected += OnPlayerConnected;
        CursedPlayerEventsHandler.Disconnecting += OnPlayerDisconnecting;
        CursedPlayerEventsHandler.Died += OnPlayerDied;
        CursedPlayerEventsHandler.ReceivingDamage += OnPlayerReceivingDamage;
        CursedPlayerEventsHandler.Escaping += OnPlayerEscaping;
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
    }
    
    private static readonly Dictionary<CursedPlayer, int> Kills = new();
    private static readonly Dictionary<CursedPlayer, float> Damage = new();
    private static string _escapeMessage = string.Empty;
    private static string _dieMessage = string.Empty;

    private static readonly string[] FinalMessages = new string[4];
    
    public static string GetContent(int n) => FinalMessages[n];

    public static void ClearCache()
    {
        Kills.Clear();
        Damage.Clear();
        _escapeMessage = string.Empty;
        _dieMessage = string.Empty;
    }
    
    private static void OnPlayerConnected(PlayerConnectedEventArgs args)
    {
        Kills.Add(args.Player, 0);
        Damage.Add(args.Player, 0);
    }

    private static void OnPlayerDisconnecting(PlayerDisconnectingEventArgs args)
    {
        Kills.Remove(args.Player);
        Damage.Remove(args.Player);
    }
    
    private static void OnPlayerEscaping(PlayerEscapingEventArgs args)
    {
        if (string.IsNullOrEmpty(_escapeMessage))
        {
            _escapeMessage = $"<color=red>Jesus-QC</color> <lowercase>was the first to escape the facility in </lowercase> <color=red>{FormatTimer(CursedRound.RoundTime)}</color>";
        }
        
        Kills[args.Player]++;
    }
    
    private static void OnPlayerReceivingDamage(PlayerReceivingDamageEventArgs args)
    {
        if (args.DamageHandlerBase is not AttackerDamageHandler attackerDamageHandler)
            return;
        
        if (!CursedPlayer.TryGet(attackerDamageHandler.Attacker.Hub, out CursedPlayer attacker))
            return;
        
        if (attacker == args.Player)
            return;
        
        Damage[attacker] += args.DamageAmount;
    }
    
    private static void OnPlayerDied(PlayerDiedEventArgs args)
    {
        if (string.IsNullOrEmpty(_dieMessage))
        {
            _dieMessage = $"<color=red>Jesus-QC</color> <lowercase>was the first to die</lowercase> <color=yellow>{FormatTimer(CursedRound.RoundTime)}</color> <lowercase>after the round started</lowercase>";
        }
        
        if (args.DamageHandlerBase is not AttackerDamageHandler attackerDamageHandler)
            return;
        
        if (!CursedPlayer.TryGet(attackerDamageHandler.Attacker.Hub, out CursedPlayer attacker))
            return;
        
        if (attacker == args.Player)
            return;
        
        Kills[attacker]++;
    }

    private static void OnRoundEnded()
    {
        string mostKillsMsg = "<color=red>nobody</color> <lowercase>had any kill</lowercase> <color=red>amazing</color>";
        if (Kills.Count > 0)
        {
            CursedPlayer mostKills = Kills.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            mostKillsMsg = $"<color=red>{mostKills.DisplayNickname}</color> <lowercase>had the most kills with</lowercase> <color=red>{Kills[mostKills]}</color>";
        }
        
        string mostDamageMsg = "<color=red>nobody</color> <lowercase>had any damage</lowercase> <color=red>amazing</color>";
        if (Damage.Count > 0)
        {
            CursedPlayer mostDamage = Damage.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            mostDamageMsg = $"<color=red>{mostDamage.DisplayNickname}</color> <lowercase>had the most damage with</lowercase> <color=red>{Damage[mostDamage]}</color>";
        }

        FinalMessages[0] = mostKillsMsg;
        FinalMessages[1] = mostDamageMsg;
        FinalMessages[2] = _dieMessage;
        FinalMessages[3] = _escapeMessage;
        
        ClearCache();
    }

    private static string FormatTimer(TimeSpan timeSpan)
    {
        if (timeSpan.Minutes != 0)
            return timeSpan.Minutes + "m " + timeSpan.Seconds + "s"; 
        
        return timeSpan.Seconds + "s"; 
    }
}