using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CustomPlayerEffects;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.Zombie;

public class BabyZombieSubclass : SubclassBase
{
    private const byte MovementBoostIntensity = 20;
    private const float DamageAmount = 30;
    
    public override string CodeName => "babyzombie";
    public override string Name => "<color=#456431>B<lowercase>aby</lowercase> Z<lowercase>ombie</lowercase></color>";
    public override string Description => "you run really fast, but you are weak";
    public override Vector3 PlayerSize { get; } = new (0.6f, 0.6f, 0.6f);
    public override float SpawnChance { get; } = 0.3f;
    public override float Health => 75f;
    public override float MaxHealth => 75f;
    
    public override void OnSpawn(CursedPlayer player)
    {
        player.EnableEffect<MovementBoost>().Intensity = MovementBoostIntensity;
        base.OnSpawn(player);
    }
    
    public class BabyZombieSubclassHandler : ISubclassEventsHandler
    {
        public void OnLoaded()
        {
            CursedPlayerEventsHandler.ReceivingDamage += OnPlayerReceivingDamage;
        }
        
        private static void OnPlayerReceivingDamage(PlayerReceivingDamageEventArgs args)
        {
            if (args.Attacker is null || args.Attacker == args.Player)
                return;
            
            if (args.Attacker.Role != RoleTypeId.Scp0492 || !args.Attacker.TryGetSubclass(out SubclassBase attackerSubclass) || attackerSubclass is not BabyZombieSubclass)
                return;

            args.DamageAmount = DamageAmount;
        }
    }
}