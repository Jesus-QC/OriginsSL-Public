namespace OriginsSL.Modules.LevelingSystem;

// TODO: mariadb integrated db in dedi
// PD: Nexus, do not use sqlite
public class LevelingSystemModule : OriginsModule
{
    public override void OnLoaded()
    {
        LevelingSystemEventsHandler.RegisterServerEvents();
        LevelingSystemEventsHandler.RegisterPlayerEvents();
    }
}
