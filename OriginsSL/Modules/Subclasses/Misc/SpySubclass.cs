using CursedMod.Events.Arguments.Player;
using PlayerRoles;

namespace OriginsSL.Modules.Subclasses.Misc;

public abstract class SpySubclass : SubclassBase
{
    public virtual RoleTypeId DisguisedAs { get; } = RoleTypeId.None;
    
    public class SpySubclassHandler : ISubclassEventsHandler
    {
        public void OnLoaded()
        {
            
        }

        private static void OnChangingRole(PlayerChangingRoleEventArgs args)
        {
            
        }
    }
}