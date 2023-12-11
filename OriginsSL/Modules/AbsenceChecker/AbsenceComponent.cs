using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Round;
using OriginsSL.Features;
using PlayerRoles;
using PluginAPI.Core;
using UnityEngine;

namespace OriginsSL.Modules.AbsenceChecker;

public class AbsenceComponent : MonoBehaviour
{
    private const int AfkTime = 90;
    
    private float _counter;
    private int _afkTime;
    private Vector3 _lastPos;
    private Vector2 _lastRot;

    private CursedPlayer _player;

    public static void AddController(CursedPlayer player)
    {
        player.AddComponent<AbsenceComponent>().Init(player);
    }

    public void Init(CursedPlayer player)
    {
        _player = player;
    }
    
    private void Update()
    {
        _counter += Time.deltaTime;

        if (_counter < 1)
            return;
        
        _counter = 0;
        
        Vector3 pos = _player.Position; 
        Vector2 rot = _player.Rotation;
            
        if (_player.CurrentRole.Team != Team.Dead && _player.Role != RoleTypeId.Scp079 && !CursedRound.IsInLobby && _lastPos == pos && _lastRot == rot)
        {
            _afkTime++;

            if (_afkTime < AfkTime - 10) 
                return;
            
            _player.ShowBroadcast($"<b><color=#ff4940>You were detected as afk.</color>\nMove in less than {AfkTime - _afkTime} seconds or you will be kicked.</b>", 1);

            if (_afkTime < AfkTime)
                return;
            
            Log.Info($"{_player.RealNickname} has been detected as AFK!");

            if (_player.Role != RoleTypeId.Tutorial && OriginsPlayerReplacer.TryGetRandomSpectator(out CursedPlayer target))
            {
                OriginsPlayerReplacer.ReplacePlayer(target, _player);
                // TODO: HUD.SEND HINT
                // target.SendHint(ScreenZone.Environment, "<color=red><i>Replaced afk player</i></color>", 5f);
            }

            if (CursedPlayer.Count > 25)
                _player.Disconnect("Kicked for being AFK.\n[KICKED BY A SERVER MODIFICATION]");

            _player.SetRole(RoleTypeId.Spectator);
            _lastPos = pos;
            _lastRot = rot;
            _afkTime = -5;
        }
        else
        {
            _lastPos = pos;
            _lastRot = rot;
            _afkTime = 0;
        }
    }
}
