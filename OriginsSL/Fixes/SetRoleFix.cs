using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;

namespace OriginsSL.Fixes;

public class SetRoleFix : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.ChangingRole += OnPlayerChangingRole;
    }
    
    private static void OnPlayerChangingRole(PlayerChangingRoleEventArgs args)
    {
        if (args.NewRole != args.Player.Role)
            return;

        args.IsAllowed = false;
    }
}