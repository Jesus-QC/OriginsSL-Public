using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CursedMod.Events.Arguments;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Extensions;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Modules.Subclasses.ClassD;
using PlayerRoles;
using PluginAPI.Core;

namespace OriginsSL.Modules.Subclasses;

public class SubclassManager : OriginsModule
{
    public static readonly Dictionary<CursedPlayer, ISubclass> Subclasses = new ();

    private static readonly Dictionary<RoleTypeId, ISubclass[]> AvailableSubclasses = new()
    {
        [RoleTypeId.ClassD] = [new JanitorSubclass()]
    };
    
    public override void OnLoaded()
    {
        LoadEventsHandlers();
        CursedPlayerEventsHandler.ChangingRole += OnPlayerChangingRole;
        CursedPlayerEventsHandler.Dying += OnPlayerDying;
    }

    private static void OnPlayerChangingRole(PlayerChangingRoleEventArgs args)
    {
        ISubclass subclass = GetRandomSubclass(args.NewRole);
        args.Player.SetSubclass(subclass);
        subclass?.OnSpawn(args.Player);
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
            Subclasses.Remove(player);
            return;
        }
        
        Subclasses.SetOrAddElement(player, subclass);
    }
    
    private static void LoadEventsHandlers()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsInterface || !typeof(ISubclassEventsHandler).IsAssignableFrom(type)) 
                continue;
            
            Log.Info(type.FullName);
            
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