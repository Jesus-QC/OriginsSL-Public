using OriginsSL.Modules.Subclasses.Misc;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Scientist;

public class MidgetSubclass : PitchChangerSubclass
{ 
    public override string CodeName => "midget";
    public override string Name => "<color=#c74cb0>M<lowercase>idget</lowercase></color>";
    public override string Description => "you are known by ghost, a female midget scientist";
    public override float SpawnChance => 0.5f;
    public override Vector3 PlayerSize { get; } = new (0.6f, 0.6f, 0.6f);

    public override float Pitch { get; } = 1.4f;

    public override bool KeepAfterEscaping => true;

    public override float Health { get; } = 50f;
}