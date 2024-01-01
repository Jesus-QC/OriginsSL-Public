using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OriginsSL;

public static class ModuleLoader
{
    private static readonly List<OriginsModule> LoadedModules = [];
    
    public static void LoadModules()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!type.IsSubclassOf(typeof(OriginsModule))) 
                continue;
            
            if (type.GetCustomAttribute<DisabledModuleAttribute>() != null)
                continue;
            
            OriginsModule module = (OriginsModule) Activator.CreateInstance(type);
            LoadedModules.Add(module);
        }

        IOrderedEnumerable<OriginsModule> modules = LoadedModules.OrderBy(x => x.Priority);

        foreach (OriginsModule module in modules)
        {
            module.OnLoaded();
        }
    }
}

public class DisabledModuleAttribute : Attribute
{}