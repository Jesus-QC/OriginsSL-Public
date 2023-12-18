using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Dummies;
using CursedMod.Features.Wrappers.Player.Ragdolls;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Emote.Components;

public class EmoteDummyOwner : MonoBehaviour
{
    public static readonly HashSet<CursedPlayer> PlayersEmoting = [];
    
    private Vector3 _initialPos = Vector3.zero;
    private RoleTypeId _role = RoleTypeId.None;
    private Vector3 _scale = Vector3.zero;
    private Vector3 _fakeScale = Vector3.zero;
    
    private CursedPlayer _owner;
    private CursedPlayer _dummy;
    private CursedRagdoll _ragdoll;

    public void Init(CursedPlayer player, CursedPlayer dummy, CursedRagdoll ragdoll)
    {
        _owner = player;
        _dummy = dummy;
        _ragdoll = ragdoll;
        _initialPos = player.Position;
        _role = player.Role;
        _scale = player.Scale;
        _fakeScale = player.FakeScale;
        player.FakeScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    private void Update()
    {
        if (_initialPos == _owner.Position || _role != _owner.Role)
            return;
        
        _owner.FakeScale = _fakeScale;
        
        if (_scale != Vector3.one)
            _owner.Scale = _scale;
        
        _dummy.DestroyDummy();
        _ragdoll.Destroy();
        Destroy(this);
        PlayersEmoting.Remove(_owner);
    }
}