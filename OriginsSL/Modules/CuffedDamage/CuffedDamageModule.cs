using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using PlayerStatsSystem;

namespace OriginsSL.Modules.CuffedDamage;

public class CuffedDamageModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.ReceivingDamage += OnPlayerReceivingDamage;
    }

    private static void OnPlayerReceivingDamage(PlayerReceivingDamageEventArgs args)
    {
        if (!args.Player.IsCuffed || args.DamageHandlerBase is not AttackerDamageHandler)
            return;

        args.IsAllowed = false;
        args.DamageAmount = 0;
    }
}