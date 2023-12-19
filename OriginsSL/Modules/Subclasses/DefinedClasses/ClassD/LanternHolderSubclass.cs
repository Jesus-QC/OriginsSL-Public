using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class LanternHolderSubclass : SubclassBase
{
    public override string CodeName => "lanternholder";
    public override string Name => "<color=#e3bb76>L<lowercase>antern</lowercase> H<lowercase>older</lowercase></color>";
    public override string Description => "you hold peace in your hands";
    public override float SpawnChance => 0.5f;
    
    public override bool KeepAfterEscaping => true;

    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Lantern];
}