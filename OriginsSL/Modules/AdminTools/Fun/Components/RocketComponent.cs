using CursedMod.Features.Wrappers.Player;
using UnityEngine;
using Utils;

namespace OriginsSL.Modules.AdminTools.Fun.Components;

public class RocketComponent : MonoBehaviour
{
    private float _startPos;
    private float _startTime;
    public CursedPlayer Player;
    public bool IsInversed;

    private void Start() => _startPos = Player.Position.y;
    
    private void Update()
    {
        if (!IsInversed)
            Player.Position += new Vector3(0, 20 * Time.deltaTime);
        else
            Player.Position -= new Vector3(0, 20 * Time.deltaTime);
        
        _startTime += Time.deltaTime;

        if (_startTime > 1)
        {
            ExplosionUtils.ServerSpawnEffect(Player.Position, ItemType.GrenadeHE);
            Player.Kill("I believe I can fly! I believe I can touch the sky!");
            Destroy(this);
            return;
        }

        if (Player.Position.y < _startPos + 10) 
            return;
            
        ExplosionUtils.ServerSpawnEffect(Player.Position, ItemType.GrenadeHE);
        Player.Kill("I believe I can fly! I believe I can touch the sky!");
        Destroy(this);
    }
}