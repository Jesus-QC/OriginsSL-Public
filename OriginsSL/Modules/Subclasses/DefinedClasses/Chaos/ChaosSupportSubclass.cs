using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Chaos;

public class ChaosSupportSubclass : SubclassBase
{
    public override string CodeName => "doctor";
    public override string Name => "<color=#d51143>C<lowercase>haos</lowercase> S<lowercase>upport</lowercase></color>";
    public override string Description => "you carry some medical supplies in your pockets";
    public override float SpawnChance => 0.25f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Medkit, ItemType.Medkit, ItemType.Painkillers, ItemType.Painkillers];
}