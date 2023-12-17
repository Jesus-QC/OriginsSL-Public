namespace OriginsSL.Modules.LevelingSystem;

public class LevelingSystemModule : OriginsModule
{
    public static LevelingConfig Config { get; internal set; }
    
    public override void OnLoaded()
    {
        LevelingSystemEventsHandler.InitDatabase();
        LevelingSystemEventsHandler.RegisterPlayerEvents();
    }
}
