using CursedMod.Features.Wrappers.Player;
using UnityEngine;
using Utils;

namespace OriginsSL.Modules.AdminTools.Fun.Components;

public class AliveGrenadeComponent : MonoBehaviour
{
    private float _startTime;
    public CursedPlayer Player;
    public ItemType ItemType = ItemType.GrenadeHE;

    private void Update()
    {
        _startTime += Time.deltaTime;

        if (_startTime > 5)
        {
            Destroy(this);
            return;
        }
        
        ExplosionUtils.ServerSpawnEffect(Player.Position, ItemType);
    }
}