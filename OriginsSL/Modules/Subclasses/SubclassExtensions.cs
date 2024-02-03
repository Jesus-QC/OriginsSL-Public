using CursedMod.Events.Arguments.Player;
using CursedMod.Features.Extensions;
using CursedMod.Features.Wrappers.Player;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public static class SubclassExtensions
{
    public static bool TryGetSubclass(this CursedPlayer player, out SubclassBase subclass)
        => SubclassManager.Subclasses.TryGetValue(player, out subclass);

    public static void SetSubclass(this CursedPlayer player, SubclassBase subclass)
        => SubclassManager.SetSubclass(player, subclass);

    public static void ForceSavedSubclass(this CursedPlayer player, SubclassBase subclass)
    {
        if (subclass is null)
        {
            SubclassManager.Subclasses.Remove(player);
            return;
        }
        
        SubclassManager.Subclasses.SetOrAddElement(player, subclass);
    }
    
    public static string GetSubclassName(this CursedPlayer player) 
        => TryGetSubclass(player, out SubclassBase subclass) ? subclass.Name : string.Empty;
    
    public static string GetSubclassDescription(this CursedPlayer player) 
        => TryGetSubclass(player, out SubclassBase subclass) ? subclass.Description : string.Empty;

    public static void ForceSubclass(this CursedPlayer player, SubclassBase subclass)
    {
        player.SetSubclass(subclass);
        if (subclass.PlayerSize != Vector3.zero)
            player.Scale = subclass.PlayerSize;
        if (subclass.FakeSize != Vector3.zero)
            player.FakeScale = subclass.FakeSize;
        PlayerSpawningEventArgs spawningArgs = new (player.ReferenceHub, player.RoleBase, player.Position, player.HorizontalRotation);
        SubclassManager.OnSpawning(spawningArgs);
        player.Position = spawningArgs.SpawnPosition;
    }
}