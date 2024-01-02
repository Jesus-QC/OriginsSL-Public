using System.Collections.Generic;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Loader;

namespace OriginsSL.Modules.BulletHoleCap;

public class BulletHoleCapModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.PlacingBulletHole += OnPlacingBulletHole;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
    }
    
    private static readonly Dictionary<CursedPlayer, byte> BulletHoleCounter = new();
    
    private static void OnRestartingRound()
    {
        BulletHoleCounter.Clear();
    }
    
    private static void OnPlacingBulletHole(PlayerPlacingBulletHoleEventArgs args)
    {
        if (BulletHoleCounter.TryGetValue(args.Player, out byte count))
        {
            if (count >= 75)
            {
                args.IsAllowed = false;
                return;
            }
            
            BulletHoleCounter[args.Player]++;
            return;
        }
        
        BulletHoleCounter.Add(args.Player, 1);
    }
}