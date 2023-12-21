using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Dummies;
using CursedMod.Features.Wrappers.Player.Ragdolls;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Emote.Components;

public class EmoteDummyOwner : MonoBehaviour
{
    public static readonly Dictionary<CursedPlayer, EmoteDummyOwner> PlayersEmoting = [];
    
    private Vector3 _initialPos = Vector3.zero;
    private RoleTypeId _role = RoleTypeId.None;
    private Vector3 _scale = Vector3.zero;
    private Vector3 _fakeScale = Vector3.zero;
    
    public CursedPlayer Owner;
    public CursedPlayer Dummy;
    public CursedRagdoll Ragdoll;

    public void Init(CursedPlayer player, CursedPlayer dummy, CursedRagdoll ragdoll)
    {
        PlayersEmoting.Add(player, this);
        
        Owner = player;
        Dummy = dummy;
        Ragdoll = ragdoll;
        _initialPos = player.Position;
        _role = player.Role;
        _scale = player.Scale;
        _fakeScale = player.FakeScale;
        player.FakeScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    private void Update()
    {
        if (_initialPos == Owner.Position && _role == Owner.Role)
            return;
        
        StopEmoting();
    }

    public void StopEmoting()
    {
        Owner.FakeScale = _fakeScale;
        
        if (_scale != Vector3.one)
            Owner.Scale = _scale;
        
        Destroy(this);
        Dummy.DestroyDummy();
        Ragdoll.Destroy();
        PlayersEmoting.Remove(Owner);
    }
}