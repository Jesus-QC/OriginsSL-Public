using OriginsSL.Modules.Subclasses.Misc;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class OrcSubclass : PitchChangerSubclass
{ 
    public override string CodeName => "orc";
    public override string Name => "<color=#454339>O<lowercase>rc</lowercase></color>";
    public override string Description => "you are big and your voice sounds deep";
    public override float SpawnChance => 1f;

    public override Vector3 FakeSize { get; } = new (1.7f, 1, 1.5f);
    
    public override float Pitch { get; } = 0.7f;
}