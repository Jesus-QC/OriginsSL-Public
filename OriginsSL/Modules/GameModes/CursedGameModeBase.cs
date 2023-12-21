namespace OriginsSL.Modules.GameModes;

public abstract class CursedGameModeBase : ICursedGameMode
{
    public virtual string GameModeName => string.Empty;
    
    public virtual string GameModeDescription => string.Empty;

    public virtual bool IsCustomLobbyEnabled => true;
    
    public virtual void PrepareGameMode() { }

    public virtual void StopGameMode() { }
}