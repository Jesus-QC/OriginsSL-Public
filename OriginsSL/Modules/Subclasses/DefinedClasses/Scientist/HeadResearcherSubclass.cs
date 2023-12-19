using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Scientist;

public class HeadResearcherSubclass : SubclassBase
{
    public override string CodeName => "headresearcher";
    public override string Name => "<color=#FFFF7C>H<lowercase>ead</lowercase> R<lowercase>esearcher</lowercase></color>";
    public override string Description => "well-known scientist, higher level clearance";
    public override float SpawnChance => 0.25f;
    public override List<ItemType> OverrideInventory { get; } = [ItemType.KeycardResearchCoordinator, ItemType.Medkit, ItemType.Flashlight];

}