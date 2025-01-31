using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using OriginsSL.Loader;
using PlayerRoles;
using Utils;

namespace OriginsSL.Modules.PeanutExplode;

public class PeanutExplodeModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Dying += OnDying;
    }
    
    private static void OnDying(PlayerDyingEventArgs args)
    {
        if (args.Player.Role is not RoleTypeId.Scp173) 
            return;
        
        ExplosionUtils.ServerSpawnEffect(args.Player.Position, ItemType.GrenadeHE);
    }
}