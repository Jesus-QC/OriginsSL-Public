using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using OriginsSL.Loader;

namespace OriginsSL.Modules.WelcomeMessage;

public class WelcomeMessageModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Connected += OnPlayerConnected;
    }

    private static void OnPlayerConnected(PlayerConnectedEventArgs args)
    {
        args.Player.ShowBroadcast($"<size=200%><b><color=#7cb0f9>{args.Player.DisplayNickname}</color></b></size>\n<b>Welcome to <color=#E2E0A6>o</color><color=#D8D4AC>r</color><color=#CEC8B2>i</color><color=#C4BCB8>g</color><color=#BAB0BE>i</color><color=#B0A4C4>n</color><color=#A698CA>s</color> <color=#9280D6>s</color><color=#8874DC>l \ud83d\udcab</color>");
    }
}