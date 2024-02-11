using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CursedMod.Events.Arguments;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Extensions;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Roles;
using OriginsSL.Loader;
using OriginsSL.Modules.CustomItems;
using OriginsSL.Modules.LevelingSystem;
using OriginsSL.Modules.Subclasses.DefinedClasses.Chaos;
using OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;
using OriginsSL.Modules.Subclasses.DefinedClasses.FoundationForces;
using OriginsSL.Modules.Subclasses.DefinedClasses.Guard;
using OriginsSL.Modules.Subclasses.DefinedClasses.Scientist;
using OriginsSL.Modules.Subclasses.DefinedClasses.Zombie;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public class SubclassManager : OriginsModule
{
    public override byte Priority { get; set; } = 200;

    public static readonly Dictionary<CursedPlayer, SubclassBase> Subclasses = new ();

    public static readonly Dictionary<RoleTypeId, SubclassBase[]> AvailableSubclasses = new()
    {
        [RoleTypeId.ClassD] = 
        [
            new JanitorSubclass(),
            new OrcSubclass(),
            new KidSubclass(),
            new NtfSpySubclass(),
            new PriestSubclass(),
            new DrugDealerSubclass(),
            new SignalIntruderSubclass(),
            new MsSweetieSubclass(),
            new TestSubjectSubclass(),
        ],
        [RoleTypeId.Scientist] = 
        [
            new HeadResearcherSubclass(),
            new ChaosSpySubclass(),
            new DoctorSubclass(),
            new VigilantSubclass(),
            new MidgetSubclass(),
            new TestSubjectSubclass(),
        ],
        [RoleTypeId.FacilityGuard] = 
        [
            new DrugDealerSubclass(),
            new SeniorGuardSubclass(),
        ],
        [RoleTypeId.NtfSpecialist] =
        [
            new PriestSubclass(),
        ],
        [RoleTypeId.NtfPrivate] = 
        [
            new PriestSubclass(),
            new CandyLover(),
        ],
        [RoleTypeId.NtfSergeant] =
        [
            new CandyLover(),
            new PriestSubclass(),
        ],
        [RoleTypeId.NtfCaptain] = 
        [
            new CandyLover(),
        ],
        [RoleTypeId.ChaosConscript] =
        [
            new PriestSubclass(),
            new ChaosSpySubclass(),
        ],
        [RoleTypeId.ChaosMarauder] = 
        [
            new PriestSubclass(),
            new ChaosJuggernautSubclass(),
            new ChaosSupportSubclass(),
        ],
        [RoleTypeId.ChaosRepressor] = 
        [
            new PriestSubclass(),
            new ChaosSupportSubclass(),
        ],
        [RoleTypeId.ChaosRifleman] = 
        [
            new PriestSubclass(),
            new ChaosSupportSubclass(),
        ],
        [RoleTypeId.Scp0492] = 
        [
            new VampireSubclass(),
            new BabyZombieSubclass()
        ],
    };
    
    public override void OnLoaded()
    {
        LoadEventsHandlers();
        CursedPlayerEventsHandler.ChangingRole += OnPlayerChangingRole;
        CursedPlayerEventsHandler.Dying += OnPlayerDying;
        CursedPlayerEventsHandler.Spawning += OnSpawning;
        CursedPlayerEventsHandler.Disconnecting += OnPlayerDisconnecting;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
    }

    private static void OnRestartingRound()
    {
        Subclasses.Clear();

        foreach (CoroutineHandle coroutine in SubclassBase.ActiveCoroutines)
            Timing.KillCoroutines(coroutine);
        
        SubclassBase.ActiveCoroutines.Clear();
    }

    private static void OnPlayerChangingRole(PlayerChangingRoleEventArgs args)
    {
        if (args.NewRole == args.Player.Role)
        {
            args.IsAllowed = false;
            return;
        }
        
        if (args.Player.TryGetSubclass(out SubclassBase oldSubclass))
        {
            if (oldSubclass.IsLocked)
                return;

            if (oldSubclass.KeepAfterEscaping && args.ChangeReason is RoleChangeReason.Escaped)
            {
                oldSubclass.SkipSpawning = true;
                return;
            }
        }
        
        SubclassBase subclass = GetRandomSubclass(args.NewRole, args.Player);
        args.Player.SetSubclass(subclass);
        
        if (subclass is null)
            return;

        if (subclass.SpawnRole != RoleTypeId.None)
            args.NewRole = subclass.SpawnRole;
        if (subclass.PlayerSize != Vector3.zero)
            args.Player.Scale = subclass.PlayerSize;
        if (subclass.FakeSize != Vector3.zero)
            args.Player.FakeScale = subclass.FakeSize;
        
        // If the player is revived then spawning isn't called
        if (args.ChangeReason != RoleChangeReason.Revived)
            return;

        Timing.CallDelayed(0.4f, () =>
        {
            SetSpawningProperties(args.Player, subclass);
        });
    }

    public static void OnSpawning(PlayerSpawningEventArgs args)
    {
        if (!args.Player.TryGetSubclass(out SubclassBase subclass))
            return;

        if (subclass.SkipSpawning)
        {
            subclass.SkipSpawning = false;
            return;
        }
        
        if (subclass.SpawnLocation != RoleTypeId.None)
            args.SpawnPosition = CursedRoleManager.GetRoleSpawnPosition(subclass.SpawnLocation);
        
        Timing.CallDelayed(0.4f, () =>
        {
            SetSpawningProperties(args.Player, subclass);
        });
    }

    private static void SetSpawningProperties(CursedPlayer player, SubclassBase subclass)
    {
        if (!subclass.Spoofed)
            player.CustomInfo = $"{GetLevelingCustomInfo(player)}\n<size=22><color=#50C878>{subclass.CodeName}\n(Custom Class)</color></size>";
        if (subclass.Health > 0)
            player.Health = subclass.Health;
        if (subclass.MaxHealth > 0)
            player.MaxHealth = subclass.MaxHealth;
        if (subclass.ArtificialHealth > 0)
            player.ArtificialHealth = subclass.ArtificialHealth;
        if (subclass.HumeShield > 0)
            player.HumeShield = subclass.HumeShield;
        if (subclass.OverrideInventory != null)
            player.SetItems(subclass.OverrideInventory);
        if (subclass.OverrideAmmo != null)
            player.SetAmmo(subclass.OverrideAmmo);
        if (subclass.AdditiveInventory != null)
            player.AddItems(subclass.AdditiveInventory);
        if (subclass.AdditiveAmmo != null)
            player.AddAmmo(subclass.AdditiveAmmo);
        if (!subclass.AllowCustomItems)
            foreach (ushort serial in player.Items.Keys) CustomItemManager.RemoveCustomItem(serial);
            
        subclass.OnSpawn(player);
    }
    
    private static void OnPlayerDying(ICursedPlayerEvent args)
    {
        if (!args.Player.TryGetSubclass(out SubclassBase subclass))
            return;
        
        subclass.OnDeath(args.Player);
    }

    private static void OnPlayerDisconnecting(PlayerDisconnectingEventArgs args)
    {
        if (!args.Player.TryGetSubclass(out _))
            return;
        
        args.Player.SetSubclass(null);
    }
    
    public static void SetSubclass(CursedPlayer player, SubclassBase subclass)
    {
        if (subclass is null)
        {
            ResetTemporaryData(player);
            Subclasses.Remove(player);
            return;
        }
        
        ResetTemporaryData(player);
        Subclasses.SetOrAddElement(player, subclass);
    }

    private static void ResetTemporaryData(CursedPlayer player)
    {
        player.CustomInfo = GetLevelingCustomInfo(player);
        
        if (!Subclasses.TryGetValue(player, out SubclassBase oldSubclass))
            return;
            
        if (oldSubclass.PlayerSize != Vector3.zero || oldSubclass.FakeSize != Vector3.zero)
            player.Scale = Vector3.one;
        
        oldSubclass.OnDestroy(player);
    }
    
    private static void LoadEventsHandlers()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsInterface || !typeof(ISubclassEventsHandler).IsAssignableFrom(type)) 
                continue;
            
            ISubclassEventsHandler module = (ISubclassEventsHandler) Activator.CreateInstance(type);
            module.OnLoaded();
        }
    }

    private static SubclassBase GetRandomSubclass(RoleTypeId roleTypeId, CursedPlayer player)
    {
        if (!AvailableSubclasses.TryGetValue(roleTypeId, out SubclassBase[] subclasses) || subclasses.Length == 0)
            return null;
        
        float totalChance = subclasses.Sum(subclass => subclass.FilterSubclass(player) ? subclass.SpawnChance : 0) + 1;
        float finalChance = UnityEngine.Random.Range(0f, totalChance);
        
        foreach (SubclassBase subclass in subclasses)
        {
            finalChance -= subclass.FilterSubclass(player) ? subclass.SpawnChance : 0;
            
            if (finalChance <= 0f)
                return Activator.CreateInstance(subclass.GetType()) as SubclassBase;
        }
        
        return null;
    }

    private static string GetLevelingCustomInfo(CursedPlayer player)
    {
        if (player.DoNotTrack)
            return string.Empty;

        (int level, _, _) = player.GetLevelingProgress();
        return $"<size=15><color=#DC143C>Level {level}</color></size>";
    }
}