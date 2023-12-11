using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using PluginAPI.Core;

namespace OriginsSL.Modules.AbsenceChecker;

public class AbsenceCheckerModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Connected += OnPlayerConnected;
    }

    private void OnPlayerConnected(PlayerConnectedEventArgs args)
    {
        args.Player.AddComponent<AbsenceComponent>();
    }

    
}