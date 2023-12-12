﻿using System.Collections.Generic;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Round;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace OriginsSL.Modules.BetterVoiceChat;

public class ScpProximityChatController : OriginsModule
{
    public override void OnLoaded()
    {
        CursedRoundEventsHandler.RestartingRound += ClearData;
        CursedPlayerEventsHandler.TogglingNoClip += OnPlayerTogglingNoClip;
        CursedPlayerEventsHandler.UsingVoiceChat += OnPlayerUsingVoiceChat;
    }
    
    private static readonly HashSet<CursedPlayer> ToggledPlayers = [];

    private static void ClearData()
    {
        ToggledPlayers.Clear();
    }

    private static void OnPlayerTogglingNoClip(PlayerTogglingNoClipEventArgs args)
    {
        if (args.Player.HasNoClipPermitted)
            return;
        
        if (args.Player.Role is not (RoleTypeId.Scp049 or RoleTypeId.Scp096 or RoleTypeId.Scp106 or RoleTypeId.Scp173 or RoleTypeId.Scp0492 or RoleTypeId.Scp939))
            return;

        args.IsAllowed = false;

        if (!ToggledPlayers.Add(args.Player))
        {
            ToggledPlayers.Remove(args.Player);
            args.Player.SendOriginsHint("<b>P<lowercase>roximity chat <color=red>disabled</color></lowercase></b>", ScreenZone.Important, 3f);
            return;
        }

        args.Player.SendOriginsHint("<b>P<lowercase>roximity chat <color=#42f57b>enabled</color></lowercase></b>", ScreenZone.Important, 3f);
    }

    private static void OnPlayerUsingVoiceChat(PlayerUsingVoiceChatEventArgs args)
    {
        if (CursedRound.IsInLobby)
        {
            args.IsAllowed = false;
            SendGlobal(args.VoiceMessage);

            return;

            void SendGlobal(VoiceMessage msg)
            {
                msg.Channel = VoiceChatChannel.RoundSummary;
                foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
                {
                    if (referenceHub.connectionToClient == null || referenceHub.roleManager.CurrentRole is not IVoiceRole voiceRole2)
                        continue;
            
                    if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) == VoiceChatChannel.None)
                        continue;
                    
                    referenceHub.connectionToClient.Send(msg);
                }
            }
        }
        
        if (args.VoiceMessage.Channel != VoiceChatChannel.ScpChat)
            return;
        
        if (args.Player.Role is not (RoleTypeId.Scp049 or RoleTypeId.Scp096 or RoleTypeId.Scp106 or RoleTypeId.Scp173 or RoleTypeId.Scp0492 or RoleTypeId.Scp939) || !ToggledPlayers.Contains(args.Player))
            return;

        args.IsAllowed = false;
        SendProximityMessage(args.VoiceMessage);
    }

    private static void SendProximityMessage(VoiceMessage msg)
    {
        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
        {
            if (referenceHub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(referenceHub))
                continue;
                
            if (referenceHub.roleManager.CurrentRole is not IVoiceRole voiceRole2)
                continue;
            
            if (Vector3.Distance(msg.Speaker.transform.position, referenceHub.transform.position) >= 7)
                continue;

            if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) == VoiceChatChannel.None)
                continue;
            
            msg.Channel = VoiceChatChannel.Proximity;
            referenceHub.connectionToClient.Send(msg);
        }
    }
}