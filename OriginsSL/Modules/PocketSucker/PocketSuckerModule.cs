using System.Collections.Generic;
using CursedMod.Events.Arguments.Facility.Hazards;
using CursedMod.Events.Handlers;
using CursedMod.Features.Enums;
using CursedMod.Features.Wrappers.Player;
using CustomPlayerEffects;
using MEC;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerStatsSystem;
using PluginAPI.Core;
using UnityEngine;

namespace OriginsSL.Modules.PocketSucker;

public class PocketSuckerModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedHazardsEventHandler.StayingOnHazard += OnStayingHazard;
    }
    
    private static void OnStayingHazard(PlayerStayingOnHazardEventArgs args)
    {
        if (args.Hazard.HazardType != EnvironmentalHazardType.Sinkhole)
            return;
        
        if (Vector3.Distance(args.Player.Position, args.Hazard.SourcePosition) > 3)
            return;

        Timing.RunCoroutine(PortalAnimation(args.Player));
    }

    private static readonly HashSet<CursedPlayer> _suckingPlayers = [];
    
    private static IEnumerator<float> PortalAnimation(CursedPlayer player)
    {
        if (!_suckingPlayers.Add(player))
            yield break;
        
        bool inGodMode = player.HasGodMode;
        player.HasGodMode = true;
        
        Vector3 startPosition = player.Position, endPosition = player.Position -= Vector3.up * 1.23f * player.GameObject.transform.localScale.y;
        for (int i = 0; i < 28; i++)
        {
            player.Position = Vector3.Lerp(startPosition, endPosition, i / 28f);
            yield return Timing.WaitForOneFrame;
        }
        
        player.Position = Vector3.down * 1997f;
        player.HasGodMode = inGodMode;
        
        if (Warhead.IsDetonated)
        {
            player.Kill(DeathTranslations.PocketDecay.LogLabel);
            yield break;
        }
        
        player.Damage(10, "Pocket Suck");
        player.EnableEffect<PocketCorroding>();
        player.EnableEffect<Sinkhole>();
        
        player.SendOriginsHint("<b>Y<lowercase>ou have been sucked by a sinkhole</lowercase></b>", ScreenZone.Environment);
        _suckingPlayers.Remove(player);
    }
}