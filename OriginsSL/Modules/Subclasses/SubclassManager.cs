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
using MEC;
using OriginsSL.Modules.LevelingSystem;
using OriginsSL.Modules.Subclasses.DefinedClasses.Chaos;
using OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;
using OriginsSL.Modules.Subclasses.DefinedClasses.FoundationForces;
using OriginsSL.Modules.Subclasses.DefinedClasses.Scientist;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public class SubclassManager : OriginsModule
{
    public static readonly Dictionary<CursedPlayer, ISubclass> Subclasses = new ();

    public static readonly Dictionary<RoleTypeId, ISubclass[]> AvailableSubclasses = new()
    {
        [RoleTypeId.ClassD] = 
        [
            new JanitorSubclass(),
            new OrcSubclass(),
            new KidSubclass(),
            new NtfSpy(),
            new LanternHolderSubclass(),
            new DrugDealerSubclass(),
        ],
        [RoleTypeId.Scientist] = 
        [
            new HeadResearcherSubclass(),
            new ChaosSpy(),
        ],
        [RoleTypeId.NtfPrivate] = 
        [
            new LanternHolderSubclass(),
        ],
        [RoleTypeId.ChaosConscript] =
        [
            new LanternHolderSubclass(),
        ],
    };
    
    public override void OnLoaded()
    {
        LoadEventsHandlers();
        CursedPlayerEventsHandler.ChangingRole += OnPlayerChangingRole;
        CursedPlayerEventsHandler.Dying += OnPlayerDying;
        CursedPlayerEventsHandler.Spawning += OnSpawning;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
    }

    private static void OnRestartingRound()
    {
        Subclasses.Clear();
    }

    private static void OnPlayerChangingRole(PlayerChangingRoleEventArgs args)
    {
        if (args.NewRole == args.Player.Role)
        {
            args.IsAllowed = false;
            return;
        }
        
        if (args.Player.TryGetSubclass(out ISubclass oldSubclass))
        {
            if (oldSubclass.IsLocked)
                return;
            
            if (oldSubclass.KeepAfterEscaping && args.ChangeReason is RoleChangeReason.Escaped)
                return;
        }
        
        ISubclass subclass = GetRandomSubclass(args.NewRole, args.Player);
        args.Player.SetSubclass(subclass);
        
        if (subclass is null)
            return;

        if (subclass.SpawnRole != RoleTypeId.None)
            args.NewRole = subclass.SpawnRole;
        if (subclass.PlayerSize != Vector3.zero)
            args.Player.Scale = subclass.PlayerSize;
        if (subclass.FakeSize != Vector3.zero)
            args.Player.FakeScale = subclass.FakeSize;
    }

    public static void OnSpawning(PlayerSpawningEventArgs args)
    {
        if (!args.Player.TryGetSubclass(out ISubclass subclass))
            return;
        
        if (subclass.SpawnLocation != RoleTypeId.None)
            args.SpawnPosition = CursedRoleManager.GetRoleSpawnPosition(subclass.SpawnLocation);
        
        Timing.CallDelayed(0.4f, () =>
        {
            if (!subclass.Spoofed)
                args.Player.CustomInfo = $"{GetLevelingCustomInfo(args.Player)}\n<size=22><color=#50C878>{subclass.CodeName}\n(Custom Class)</color></size>";
            if (subclass.Health > 0)
                args.Player.Health = subclass.Health;
            if (subclass.ArtificialHealth > 0)
                args.Player.ArtificialHealth = subclass.ArtificialHealth;
            if (subclass.HumeShield > 0)
                args.Player.HumeShield = subclass.HumeShield;
            if (subclass.OverrideInventory != null)
                args.Player.SetItems(subclass.OverrideInventory);
            if (subclass.OverrideAmmo != null)
                args.Player.SetAmmo(subclass.OverrideAmmo);
            if (subclass.AdditiveInventory != null)
                args.Player.AddItems(subclass.AdditiveInventory);
            if (subclass.AdditiveAmmo != null)
                args.Player.AddAmmo(subclass.AdditiveAmmo);

            subclass.OnSpawn(args.Player);
        });
    }

    private static void OnPlayerDying(ICursedPlayerEvent args)
    {
        if (!args.Player.TryGetSubclass(out ISubclass subclass))
            return;
        
        subclass.OnDeath(args.Player);
        args.Player.SetSubclass(null);
    }

    public static void SetSubclass(CursedPlayer player, ISubclass subclass)
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
        
        if (!Subclasses.TryGetValue(player, out ISubclass oldSubclass))
            return;
            
        if (oldSubclass.PlayerSize != Vector3.zero || oldSubclass.FakeSize != Vector3.zero)
            player.Scale = Vector3.one;
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

    private static ISubclass GetRandomSubclass(RoleTypeId roleTypeId, CursedPlayer player)
    {
        if (!AvailableSubclasses.TryGetValue(roleTypeId, out ISubclass[] subclasses))
            return null;
        
        float totalChance = subclasses.Sum(subclass => subclass.FilterSubclass(player) ? subclass.SpawnChance : 0) + 1;
        float finalChance = UnityEngine.Random.Range(0f, totalChance);
        
        foreach (ISubclass subclass in subclasses)
        {
            finalChance -= subclass.FilterSubclass(player) ? subclass.SpawnChance : 0;
            
            if (finalChance <= 0f)
                return Activator.CreateInstance(subclass.GetType()) as ISubclass;
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