using OriginsSL.Modules.Subclasses.Misc;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class KidSubclass : PitchChangerSubclass
{ 
    public override string CodeName => "kid";
    public override string Name => "<color=#c74cb0>K<lowercase>id</lowercase></color>";
    public override string Description => "you are small and your voice sounds high";
    public override float SpawnChance => 0.6f;
    public override Vector3 PlayerSize { get; } = new (0.6f, 0.6f, 0.6f);

    public override float Pitch { get; } = 1.3f;

    public override bool KeepAfterEscaping => true;

    public override float Health { get; } = 50f;

    public override float MaxHealth { get; } = 50f;
}