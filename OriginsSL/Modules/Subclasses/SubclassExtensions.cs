using CursedMod.Features.Wrappers.Player;

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
}