using CursedMod.Features.Wrappers.Player;
using PlayerRoles;

namespace OriginsSL.Modules.Subclasses.Misc;

public abstract class SpySubclass : SubclassBase
{
    protected virtual RoleTypeId DisguisedAs => RoleTypeId.None;

    public override RoleTypeId SpawnLocation => DisguisedAs;

    public override void OnSpawn(CursedPlayer player)
    {
        player.FakeAliveRole = DisguisedAs;
        base.OnSpawn(player);
    }
}