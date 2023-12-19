using System.Collections.Generic;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Guard;

public class SeniorGuardSubclass : SubclassBase
{
    public override string CodeName => "seniorguard";
    public override string Name => "<color=#3b689c>S<lowercase>enior</lowercase> G<lowercase>uard</lowercase></color>";
    public override string Description => "has been working for a long time, upgraded equipment";
    public override float SpawnChance => 0.5f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.GunE11SR, ItemType.Flashlight];

    public override Dictionary<ItemType, ushort> AdditiveAmmo { get; } = new (){ [ItemType.GunE11SR] = 50 };
}