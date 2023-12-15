namespace OriginsSL.Modules.LevelingSystem;

// TODO: mariadb integrated db in dedi
// PD: Nexus, do not use sqlite
public class LevelingSystemModule : OriginsModule
{
    public static LevelingConfig Config { get; internal set; }
    
    public override void OnLoaded()
    {
        LevelingSystemEventsHandler.InitDatabase();
        LevelingSystemEventsHandler.RegisterServerEvents();
        LevelingSystemEventsHandler.RegisterPlayerEvents();
    }
}
