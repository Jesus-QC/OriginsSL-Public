using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;

namespace OriginsSL.Modules.WelcomeMessage;

public class WelcomeMessageModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Connected += OnPlayerConnected;
    }

    private static void OnPlayerConnected(PlayerConnectedEventArgs args)
    {
        args.Player.ShowBroadcast("Welcome to Origins SL! ");
        // TODO: Enhance message
    }
}