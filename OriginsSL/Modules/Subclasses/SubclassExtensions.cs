using CursedMod.Events.Arguments.Player;
using CursedMod.Features.Wrappers.Player;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public static class SubclassExtensions
{
    public static bool TryGetSubclass(this CursedPlayer player, out ISubclass subclass)
        => SubclassManager.Subclasses.TryGetValue(player, out subclass);

    public static void SetSubclass(this CursedPlayer player, ISubclass subclass)
        => SubclassManager.SetSubclass(player, subclass);

    public static string GetSubclassName(this CursedPlayer player) 
        => TryGetSubclass(player, out ISubclass subclass) ? subclass.Name : string.Empty;
    
    public static string GetSubclassDescription(this CursedPlayer player) 
        => TryGetSubclass(player, out ISubclass subclass) ? subclass.Description : string.Empty;

    public static void ForceSubclass(this CursedPlayer player, ISubclass subclass)
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