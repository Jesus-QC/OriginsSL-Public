using CursedMod.Features.Wrappers.Player;
using OriginsSL.Modules.Subclasses.Misc;
using PlayerRoles;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Chaos;

public class ChaosSpySubclass : SpySubclass
{
    public override string CodeName => "chaosspy";
    public override string Name => "<color=#FFFF7C>C<lowercase>haos</lowercase>S<lowercase>py</lowercase></color>";
    public override string Description => "disguised as a scientist";
    public override float SpawnChance => 0.1f;
    public override RoleTypeId SpawnRole => RoleTypeId.ChaosConscript;
    protected override RoleTypeId DisguisedAs => RoleTypeId.Scientist;
    
    public override bool FilterSubclass(CursedPlayer player) => CursedPlayer.Count > 15;
}