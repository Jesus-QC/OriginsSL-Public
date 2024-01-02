using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PluginAPI.Core;

namespace OriginsSL.Loader;

public static class ModuleLoader
{
    public static OriginsLoaderConfig Config;
    
    private static readonly List<OriginsModule> LoadedModules = [];
    
    public static void LoadModules()
    {
        Log.Info("Loading modules:");
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!type.IsSubclassOf(typeof(OriginsModule))) 
                continue;
            
            OriginsModule module = (OriginsModule) Activator.CreateInstance(type);
            
            if (module.Disabled || Config.DisabledModules.Contains(type.Name))
                continue;
            
            if (module.DisruptedOnly && Config.DisruptedEnabled)
                continue;
            
            LoadedModules.Add(module);
        }

        IOrderedEnumerable<OriginsModule> modules = LoadedModules.OrderBy(x => x.Priority);

        foreach (OriginsModule module in modules)
        {
            Log.Info($"\tLoading module: {module.GetType().Name}");
            module.OnLoaded();
        }
    }
}