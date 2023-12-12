using CursedMod.Features.Wrappers.Player;
using OriginsSL.Features.Display;

namespace OriginsSL.Modules.DisplayRenderer;

public static class DisplayRendererExtensions
{
    public static void SendOriginsHint(this CursedPlayer player, string content, ScreenZone zone = ScreenZone.Center, float duration = 4f)
    {
        if (!DisplayRendererModule.TryGetDisplayBuilder(player, out CursedDisplayBuilder displayBuilder))
            return;
        
        displayBuilder.WithContent(zone, content, duration);
    }
}