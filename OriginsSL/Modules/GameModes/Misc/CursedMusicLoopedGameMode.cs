using System.IO;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Dummies;
using OriginsSL.Modules.AudioPlayer;
using PlayerRoles;

namespace OriginsSL.Modules.GameModes.Misc;

public class CursedMusicLoopedGameMode : CursedLoopedGameMode
{
    public virtual string SongName { get; }

    private CursedPlayer _playerDummy;
    
    public override void PrepareGameMode()
    {
        _playerDummy = CursedDummy.Create("Origins Games");
        _playerDummy.SetRole(RoleTypeId.Overwatch);
        AudioPlayerBase playerBase = AudioPlayerBase.Get(_playerDummy.ReferenceHub);
        playerBase.Enqueue(Path.Combine(EntryPoint.Instance.ModuleDirectory.FullName, "GameModes", "Music", SongName), 0);
        playerBase.Play(0);
        playerBase.Loop = true;
        base.PrepareGameMode();
    }
    
    public override void StopGameMode()
    {
        _playerDummy.DestroyDummy();
        base.StopGameMode();
    }
}