using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Guard;

public class SeniorGuardSubclass : SubclassBase
{
    public override string CodeName => "seniorguard";
    public override string Name => "<color=#3b689c>S<lowercase>enior</lowercase> G<lowercase>uard</lowercase></color>";
    public override string Description => "veteran, upgraded equipment";
    public override float SpawnChance => 0.25f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.Flashlight];

    public override Dictionary<ItemType, ushort> AdditiveAmmo { get; } = new (){ [ItemType.Ammo556x45] = 50 };

    public override void OnSpawn(CursedPlayer player)
    {
        player.AddFirearm(ItemType.GunE11SR);
        base.OnSpawn(player);
    }
}