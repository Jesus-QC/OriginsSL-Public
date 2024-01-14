using System.IO;
using OriginsSL.Loader;

namespace OriginsSL.Modules.GameModes;

public class GameModesModule : OriginsModule
{
    public override void OnLoaded()
    {
        Directory.CreateDirectory(Path.Combine(EntryPoint.Instance.ModuleDirectory.FullName, "GameModes", "Music"));
        Directory.CreateDirectory(Path.Combine(EntryPoint.Instance.ModuleDirectory.FullName, "GameModes", "Schematics"));
        CursedGameModeLoader.InitGameModes();
    }
}