using CursedMod.Events.Handlers;
using CursedMod.Loader;
using CursedMod.Loader.Modules;
using HarmonyLib;

namespace OriginsSL;

public class EntryPoint : CursedModule
{
    public const string Version = "1.0.0.0";

    public override string ModuleName => "Origins SL";
    public override string ModuleAuthor => "Jesus-QC & xNexusACS";
    public override string ModuleVersion => Version;
    public override string CursedModVersion => CursedModInformation.Version;

    private Harmony _harmony;
    
    public override void OnLoaded()
    {
        ModuleLoader.LoadModules();
        
        _harmony = new Harmony("cursed.jesusqc.com");
        _harmony.PatchAll();
   
        base.OnLoaded();
    }
}
