using OriginsSL.Loader;

namespace OriginsSL.Modules.GameModes;

public class GameModesModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedGameModeLoader.InitGameModes();
    }
}