using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using PlayerRoles;
using PlayerStatsSystem;
using Utils;

namespace OriginsSL.Modules.PeanutExplode;

public class PeanutExplodeModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Dying += OnDying;
        CursedPlayerEventsHandler.ReceivingDamage += OnReceivingDamage;
    }
    
    private static void OnDying(PlayerDyingEventArgs args)
    {
        if (args.Player.Role is not RoleTypeId.Scp173) 
            return;
        
        ExplosionUtils.ServerExplode(args.Player.ReferenceHub);
    }

    private static void OnReceivingDamage(PlayerReceivingDamageEventArgs args)
    {
        if (args.DamageHandlerBase is ExplosionDamageHandler ex 
            && ex.Attacker.NetId == args.Player.NetId
            && args.Player.Role is RoleTypeId.Scp173)
            args.IsAllowed = false;
    }
}