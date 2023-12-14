using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Server;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;

namespace OriginsSL.Modules.Subclasses.Misc;

public abstract class SpySubclass : SubclassBase
{
    protected virtual RoleTypeId DisguisedAs => RoleTypeId.None;

    public override RoleTypeId SpawnLocation => DisguisedAs;

    public override bool Spoofed => true;

    private bool _disguised = true;

    private void UnDisguise(CursedPlayer player)
    {
        _disguised = false;
        player.FakeAliveRole = RoleTypeId.None;
        player.SendOriginsHint("Y<lowercase>ou have been un-disguised</lowercase>!", ScreenZone.Important, 5f);
    }
    
    public override void OnSpawn(CursedPlayer player)
    {
        player.FakeAliveRole = DisguisedAs;
        base.OnSpawn(player);
    }

    public class SpySubclassHandler : ISubclassEventsHandler
    {
        public void OnLoaded()
        {
            CursedPlayerEventsHandler.ReceivingDamage += OnPlayerReceivingDamage;
        }

        private static void OnPlayerReceivingDamage(PlayerReceivingDamageEventArgs args)
        {
            if (args.Attacker is null)
                return;
            
            if (CursedServer.IsFriendlyFireEnabled && args.Attacker.RoleBase.Team == args.Player.RoleBase.Team)
                return;

            if (args.Player.TryGetSubclass(out ISubclass playerSubclass) && playerSubclass is SpySubclass { _disguised: true} playerSpySubclass)
            {
                playerSpySubclass.UnDisguise(args.Player);
            }
            else
            {
                if (args.Attacker is null || !args.Attacker.TryGetSubclass(out ISubclass attackerSubclass) || attackerSubclass is not SpySubclass { _disguised: true } spySubclass)
                    return;
            
                spySubclass.UnDisguise(args.Attacker);
            }
        }
    }
}