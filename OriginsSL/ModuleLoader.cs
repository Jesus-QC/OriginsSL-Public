using System;
using System.Collections.Generic;
using System.Reflection;

namespace OriginsSL;

public static class ModuleLoader
{
    public static readonly HashSet<OriginsModule> Modules = new();
    
    public static void LoadModules()
    {
        Modules.Clear();
        
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!type.IsSubclassOf(typeof(OriginsModule))) 
                continue;
            
            if (type.GetCustomAttribute<DisabledModuleAttribute>() != null)
                continue;
            
            OriginsModule module = (OriginsModule) Activator.CreateInstance(type);
            Modules.Add(module);
            module.OnLoaded();
        }
    }
    
    public static void UnloadModules()
    {
        foreach (OriginsModule module in Modules)
        {
            module.OnUnloaded();
        }
        
        Modules.Clear();
    }
}

public class DisabledModuleAttribute : Attribute
{}