using CursedMod.Features.Wrappers.Player;
using CustomPlayerEffects;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class TestSubjectSubclass : SubclassBase
{
    public override string CodeName => "testsubject";
    public override string Name => "<color=#3aeb34>T<lowercase>est</lowercase> S<lowercase>ubject</lowercase></color>";
    public override string Description => "you run really fast";
    public override float SpawnChance => 0.3f;
    public override bool KeepAfterEscaping => true;

    public override float MaxHealth { get; } = 75f;

    public override float Health { get; } = 75f;

    public override void OnSpawn(CursedPlayer player)
    {
        player.EnableEffect<MovementBoost>().Intensity = 10;
        player.EnableEffect<Scp1853>();
        base.OnSpawn(player);
    }
}