using OriginsSL.Modules.Subclasses.Misc;
using PlayerRoles;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.FoundationForces;

public class NtfSpy : SpySubclass
{
    public override string CodeName => "ntfspy";
    public override string Name => "<color=#FF8E00>N<lowercase>tf</lowercase>S<lowercase>py</lowercase></color>";
    public override string Description => "disguised as a class d";
    public override float SpawnChance => 1000f;
    public override RoleTypeId SpawnRole => RoleTypeId.NtfPrivate;
    protected override RoleTypeId DisguisedAs => RoleTypeId.ClassD;

    // public override bool FilterSubclass(CursedPlayer player) => CursedPlayer.Count > 15;
}