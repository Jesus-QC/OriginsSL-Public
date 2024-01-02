using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using OriginsSL.Loader;
using PlayerRoles;
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
        if (!args.Player.IsCuffed || !args.Player.IsHuman || args.DamageHandlerBase is not AttackerDamageHandler attacker || !attacker.Attacker.Role.IsHuman())
            return;

        args.IsAllowed = false;
        args.DamageAmount = 0;
    }
}