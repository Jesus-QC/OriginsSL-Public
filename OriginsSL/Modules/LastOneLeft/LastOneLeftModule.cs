using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;

namespace OriginsSL.Modules.LastOneLeft;

public class LastOneLeftModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.ChangingRole += OnChangingRole;
    }
    
    private static void OnChangingRole(PlayerChangingRoleEventArgs args)
    {
        Team team = args.Player.CurrentRole.Team;
        
        if (team == Team.Dead)
            return;

        bool foundOne = false;
        CursedPlayer lastTeammate = null;
        
        foreach (CursedPlayer player in CursedPlayer.Collection)
        {
            if (player == args.Player || player.CurrentRole.Team != team)
                continue;
            
            if (foundOne)
                return;
                
            foundOne = true;
            lastTeammate = player;
        }

        lastTeammate?.SendOriginsHint("Y<lowercase>ou are the last one left on your team!</lowercase>", ScreenZone.Important, 5);
    }
}