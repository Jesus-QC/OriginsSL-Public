using System.IO;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Dummies;
using OriginsSL.Modules.AudioPlayer;
using PlayerRoles;

namespace OriginsSL.Modules.GameModes.Misc.GameModeComponents;

public class GameModeMusicComponent(string songName) : GameModeComponent
{
    private AudioPlayerBase _audioPlayerBase;
    private CursedPlayer _playerDummy;

    public override void OnStarting(CursedGameModeBase gameModeBase)
    {
        _playerDummy = CursedDummy.Create("Origins Games");
        _playerDummy.SetRole(RoleTypeId.Overwatch);
        _audioPlayerBase = AudioPlayerBase.Get(_playerDummy.ReferenceHub);
        _audioPlayerBase.Enqueue(Path.Combine(EntryPoint.Instance.ModuleDirectory.FullName, "GameModes", "Music", songName), 0);
        _audioPlayerBase.Volume = 10f;
        _audioPlayerBase.Play(0);
        _audioPlayerBase.Loop = true;
        base.OnStarting(gameModeBase);
    }

    public override void OnStopping()
    {
        _audioPlayerBase.Stoptrack(true);
        _playerDummy.DestroyDummy();
        base.OnStopping();
    }
}