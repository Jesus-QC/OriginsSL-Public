using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Chaos;

public class ChaosJuggernautSubclass : SubclassBase
{
    public override string CodeName => "juggernaut";
    public override string Name => "<color=#528f1d>J<lowercase>uggernaut</lowercase></color>";
    public override string Description => "pretty much a walking tank";
    public override float SpawnChance => 0.2f;

    public override Vector3 FakeSize { get; } = new (1.5f, 1, 1.5f);
    
    public override Dictionary<ItemType, ushort> AdditiveAmmo { get; } = new (){ [ItemType.Ammo556x45] = 20 };
    
    public override void OnSpawn(CursedPlayer player)
    {
        player.AddFirearm(ItemType.GunFRMG0);
        base.OnSpawn(player);
    }
}