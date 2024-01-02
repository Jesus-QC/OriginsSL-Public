using OriginsSL.Loader;

namespace OriginsSL.Modules.LevelingSystem;

public class LevelingSystemModule : OriginsModule
{
    public static LevelingConfig Config { get; internal set; }
    
    public override byte Priority { get; set; } = 240;
    
    public override void OnLoaded()
    {
        if (Config.DatabaseAddress == "0")
            return;
        
        LevelingSystemEventsHandler.InitDatabase();
        LevelingSystemEventsHandler.RegisterPlayerEvents();
    }
}
