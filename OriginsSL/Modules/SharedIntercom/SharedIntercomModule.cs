using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using PlayerRoles;
using PlayerRoles.Voice;
using VoiceChat;
using VoiceChat.Networking;

namespace OriginsSL.Modules.SharedIntercom;

public class SharedIntercomHandler : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.UsingVoiceChat += OnUsingVoiceChat;
    }
    private static void OnUsingVoiceChat(PlayerUsingVoiceChatEventArgs args)
    {
        if (Intercom.State is not IntercomState.InUse 
            || args.Player.RoleBase is not HumanRole humanRole 
            || (humanRole.FpcModule.Position - Intercom._singleton._worldPos).sqrMagnitude >= Intercom._singleton._rangeSqr) 
            return;
        
        args.IsAllowed = false;
        SendGlobalMessage(args.VoiceMessage);
    }

    private static void SendGlobalMessage(VoiceMessage msg)
    {
        msg.Channel = VoiceChatChannel.Intercom;
        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
        {
            if (hub.connectionToClient == null || hub.roleManager.CurrentRole is not IVoiceRole voiceRole2)
                continue;
            
            if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) == VoiceChatChannel.None)
                continue;
            
            hub.connectionToClient.Send(msg);
        }
    }
}