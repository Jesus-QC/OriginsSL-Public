using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CustomPlayerEffects;
using OriginsSL.Loader;
using PlayerRoles;

namespace OriginsSL.Modules.BetterOneHit;

public class BetterOneHitModule : OriginsModule
{ 
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.ReceivingDamage += OnReceivingDamage;
    }

    private static void OnReceivingDamage(PlayerReceivingDamageEventArgs args)
    {
        if (args.Attacker?.Role is not RoleTypeId.Scp106)
            return;

        args.Player.EnableEffect<Corroding>(20);
    }
}