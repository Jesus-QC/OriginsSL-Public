using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Round;
using OriginsSL.Features;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;

namespace OriginsSL.Modules.DisconnectReplace;

public class DisconnectReplaceModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Disconnecting += OnPlayerDisconnecting;
    }
    
    private static void OnPlayerDisconnecting(PlayerDisconnectingEventArgs args)
    {
        if (!CursedRound.HasStarted || CursedRound.HasEnded)
            return;
        
        if (args.Player.Role is RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.None or RoleTypeId.Tutorial)
            return;
        
        if (!OriginsPlayerReplacer.TryGetRandomSpectator(out CursedPlayer player))
            return;
        
        OriginsPlayerReplacer.ReplacePlayer(player, args.Player);
        player.SendOriginsHint("R<lowercase>eplaced a player that was disconnected</lowercase>", ScreenZone.Important, 5f);
    }
}