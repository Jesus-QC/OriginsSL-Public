using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class SignalIntruderSubclass : SubclassBase
{
    public override string CodeName => "signalintruder";
    public override string Name => "<color=#949494>S<lowercase>ignal</lowercase> I<lowercase>ntruder</lowercase></color>";
    public override string Description => "spawns with a radio to distract foundation personnel";
    public override float SpawnChance => 0.5f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Radio];
    
}