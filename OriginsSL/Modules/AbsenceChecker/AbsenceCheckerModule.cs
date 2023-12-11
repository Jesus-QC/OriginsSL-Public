using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;

namespace OriginsSL.Modules.AbsenceChecker;

public class AbsenceCheckerModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Connected += OnPlayerConnected;
    }

    private static void OnPlayerConnected(PlayerConnectedEventArgs args)
    {
        AbsenceComponent.AddController(args.Player);
    }
}