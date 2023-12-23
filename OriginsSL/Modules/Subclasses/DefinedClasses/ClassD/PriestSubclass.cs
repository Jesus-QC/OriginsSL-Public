using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class PriestSubclass : SubclassBase
{
    public override string CodeName => "priest";
    public override string Name => "<color=#e3bb76>P<lowercase>riest</lowercase></color>";
    public override string Description => "you hold peace in your hands";
    public override float SpawnChance => 0.4f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Lantern];
}