using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Scientist;

public class DoctorSubclass : SubclassBase
{
    public override string CodeName => "doctor";
    public override string Name => "<color=#d51143>D<lowercase>octor</lowercase></color>";
    public override string Description => "you may keep some medical supplies in your pockets";
    public override float SpawnChance => 0.5f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Medkit, ItemType.Medkit, ItemType.Painkillers, ItemType.Painkillers];
}