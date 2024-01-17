using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using CustomPlayerEffects;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Chaos;

public class ChaosJuggernautSubclass : SubclassBase
{
    public override string CodeName => "juggernaut";
    public override string Name => "<color=#528f1d>J<lowercase>uggernaut</lowercase></color>";
    public override string Description => "pretty much a walking tank";
    public override float SpawnChance => 0.2f;

    public override float Health { get; } = 150f;

    public override float MaxHealth { get; } = 150f;

    public override Vector3 FakeSize { get; } = new (1.5f, 1, 1.5f);
    
    public override Dictionary<ItemType, ushort> AdditiveAmmo { get; } = new (){ [ItemType.Ammo556x45] = 50 };
    
    public override void OnSpawn(CursedPlayer player)
    {
        player.AddFirearm(ItemType.GunFRMG0);
        player.EnableEffect<Stained>();
        base.OnSpawn(player);
    }
}
