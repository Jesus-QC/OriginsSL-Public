using System;
using System.Reflection;
using OriginsSL.Modules.Subclasses;

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
        
        new SubclassManager().OnLoaded(); // Load subclasses as the last one
    }
}

public class DisabledModuleAttribute : Attribute
{}