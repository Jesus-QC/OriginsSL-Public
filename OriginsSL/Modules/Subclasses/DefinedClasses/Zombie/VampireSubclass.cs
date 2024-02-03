using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using PlayerRoles;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Zombie;

public class VampireSubclass : SubclassBase
{
    public override string CodeName { get; } = "vampire";
    public override string Name { get; } = "<color=#ed4281>V<lowercase>ampire</lowercase></color>";
    public override string Description { get; } = "you can suck the blood of your victims to heal yourself";
    
    public override float SpawnChance { get; } = 0.1f;

    public class VampireSubclassHandler : ISubclassEventsHandler
    {
        public void OnLoaded()
        {
            CursedPlayerEventsHandler.ReceivingDamage += OnPlayerReceivingDamage;
        }
        
        private static void OnPlayerReceivingDamage(PlayerReceivingDamageEventArgs args)
        {
            if (args.Attacker == args.Player)
                return;
            
            if (args.Attacker.Role != RoleTypeId.Scp0492 || !args.Attacker.TryGetSubclass(out SubclassBase attackerSubclass) || attackerSubclass is not VampireSubclass vampireSubclass)
                return;
            
            args.Attacker.Heal(20);
        }
    }
}