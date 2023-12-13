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
using OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;
using PlayerRoles;
using PluginAPI.Core;
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
        ]
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
        ISubclass subclass = GetRandomSubclass(args.NewRole);
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

    private static void OnSpawning(PlayerSpawningEventArgs args)
    {
        if (!args.Player.TryGetSubclass(out ISubclass subclass))
            return;

        if (subclass.SpawnLocation != RoleTypeId.None)
            args.SpawnPosition = CursedRoleManager.GetRoleSpawnPosition(subclass.SpawnLocation);
        
        Timing.CallDelayed(0.4f, () =>
        {
            if (subclass.Health > 0)
                args.Player.Health = subclass.Health;
            if (subclass.Ahp > 0)
                args.Player.HumeShield = subclass.Ahp;
            if (subclass.Inventory != null)
                args.Player.SetItems(subclass.Inventory);
            if (subclass.Ammo != null)
                args.Player.SetAmmo(subclass.Ammo);

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
            ResetSize(player);
            Subclasses.Remove(player);
            return;
        }
        
        ResetSize(player);
        Subclasses.SetOrAddElement(player, subclass);
    }

    private static void ResetSize(CursedPlayer player)
    {
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

    private static ISubclass GetRandomSubclass(RoleTypeId roleTypeId)
    {
        if (!AvailableSubclasses.TryGetValue(roleTypeId, out ISubclass[] subclasses))
            return null;
        
        float totalChance = subclasses.Sum(subclass => subclass.SpawnChance) + 1;
        float finalChance = UnityEngine.Random.Range(0f, totalChance);
        
        foreach (ISubclass subclass in subclasses)
        {
            finalChance -= subclass.SpawnChance;
            
            if (finalChance <= 0f)
                return Activator.CreateInstance(subclass.GetType()) as ISubclass;
        }
        
        return null;
    }
}