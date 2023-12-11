using System;
using System.Reflection;

namespace OriginsSL;

public static class ModuleLoader
{
    public static void LoadModules()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!type.IsSubclassOf(typeof(OriginsModule))) 
                continue;
            
            if (type.GetCustomAttribute<DisabledModuleAttribute>() != null)
                continue;
            
            OriginsModule module = (OriginsModule) Activator.CreateInstance(type);
            module.OnLoaded();
        }
    }
}

public class DisabledModuleAttribute : Attribute
{}