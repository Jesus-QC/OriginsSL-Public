using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class DrugDealerSubclass : SubclassBase
{
    public override string CodeName => "drugdealer";
    public override string Name => "<color=#059033>D<lowercase>rug</lowercase> D<lowercase>ealer</lowercase></color>";
    public override string Description => "you may keep some drugs in your pockets";
    public override float SpawnChance => 0.5f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Painkillers, ItemType.Painkillers, ItemType.Adrenaline];
}