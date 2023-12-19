using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Scientist;

public class VigilantSubclass : SubclassBase
{
    public override string CodeName => "vigilant";
    public override string Name => "<color=#a4a4a4>V<lowercase>igilant</lowercase></color>";
    public override string Description => "spawns with a radio to call for help";
    public override float SpawnChance => 0.5f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Radio, ItemType.Painkillers];
}