using CursedMod.Events.Handlers;
using OriginsSL.Loader;

namespace OriginsSL.Modules.CustomItems;

public class CustomItemSpawner : OriginsModule
{
    public override void OnLoaded()
    {
        CursedMapGenerationEventsHandler.MapGenerated += OnMapGenerated;
    }

    private static void OnMapGenerated()
    {
        
    }
}