using System.Collections.Generic;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Roles;
using CursedMod.Features.Wrappers.Round;
using Hints;
using OriginsSL.Features.Display;
using OriginsSL.Loader;
using OriginsSL.Modules.GameModes;
using PlayerRoles.RoleAssign;
using UnityEngine;

namespace OriginsSL.Modules.DisplayRenderer;

public class DisplayRendererModule : OriginsModule
{
    private static float _counter;
    private static readonly Dictionary<CursedPlayer, CursedDisplayBuilder> DisplayBuilders = new();
    
    public static bool TryGetDisplayBuilder(CursedPlayer player, out CursedDisplayBuilder displayBuilder) => DisplayBuilders.TryGetValue(player, out displayBuilder);
    
    public override void OnLoaded()
    {
        StaticUnityMethods.OnUpdate += OnUpdate;
        CursedPlayerEventsHandler.Connected += OnPlayerConnected;
        CursedPlayerEventsHandler.Disconnecting += OnPlayerDisconnecting;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
    }

    private static void OnRestartingRound()
    {
        foreach (CursedDisplayBuilder display in DisplayBuilders.Values)
            display.ClearData();
        
        DisplayBuilders.Clear();
    }

    private static void OnPlayerConnected(PlayerConnectedEventArgs args)
    {
        DisplayBuilders.Add(args.Player, new CursedDisplayBuilder(args.Player));
    }

    private static void OnPlayerDisconnecting(PlayerDisconnectingEventArgs args)
    {
        if (!DisplayBuilders.TryGetValue(args.Player, out CursedDisplayBuilder displayBuilder))
            return;
        
        displayBuilder.ClearData();
        DisplayBuilders.Remove(args.Player);
    }
    
    private static void OnUpdate()
    {
        _counter += Time.deltaTime;
        
        if (_counter < 0.5f)
            return;

        _counter = 0;
        
        if (CursedRound.IsInLobby || !RoleAssigner._spawned)
        {
            foreach (KeyValuePair<CursedPlayer, CursedDisplayBuilder> value in DisplayBuilders)
                RenderLobby(value.Key, value.Value);

            return;
        }

        if (CursedRound.HasEnded)
        {
            foreach (KeyValuePair<CursedPlayer, CursedDisplayBuilder> value in DisplayBuilders)
                RenderEndScreen(value.Key, value.Value);
            
            return;
        }

        if (CursedGameModeLoader.EventRunning)
        {
            foreach (KeyValuePair<CursedPlayer, CursedDisplayBuilder> value in DisplayBuilders)
                RenderEvent(value.Key, value.Value);
            
            return;
        }

        foreach (KeyValuePair<CursedPlayer, CursedDisplayBuilder> value in DisplayBuilders)
        {
            Render(value.Key, value.Value);
        }
    }

    private static void RenderLobby(CursedPlayer player, CursedDisplayBuilder displayBuilder)
    {
        player.NetworkConnection?.Send(new HintMessage(new TextHint(displayBuilder.BuildForLobby(), new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4)));
    }
    
    private static void RenderEndScreen(CursedPlayer player, CursedDisplayBuilder displayBuilder)
    {
        player.NetworkConnection?.Send(new HintMessage(new TextHint(displayBuilder.BuildEndScreen(), new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4)));
    }
    
    private static void RenderEvent(CursedPlayer player, CursedDisplayBuilder displayBuilder)
    {
        player.NetworkConnection?.Send(new HintMessage(new TextHint(displayBuilder.BuildForEvent(), new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4)));
    }
    
    private static void Render(CursedPlayer player, CursedDisplayBuilder displayBuilder)
    {
        if (player.IsScp)
        {
            player.NetworkConnection?.Send(new HintMessage(new TextHint(displayBuilder.BuildForScp(), new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4)));
            return;
        }
                
        if (player.IsDead && player.CurrentRole is CursedSpectatorRole spectatorRole)
        {
            CursedPlayer spectatedPlayer = spectatorRole.SpectatedPlayer;
            player.NetworkConnection?.Send(new HintMessage(new TextHint(displayBuilder.BuildForSpectator(spectatedPlayer), new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4)));
            return;
        }
                
        player.NetworkConnection?.Send(new HintMessage(new TextHint(displayBuilder.BuildForHuman(), new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4)));
    }
}