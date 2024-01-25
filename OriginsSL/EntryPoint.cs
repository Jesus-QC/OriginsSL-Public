using CursedMod.Loader;
using CursedMod.Loader.Modules;
using HarmonyLib;
using OriginsSL.Loader;
using OriginsSL.Modules.LevelingSystem;
using OriginsSL.Modules.ServerStatusMessage;
using PluginAPI.Core;

namespace OriginsSL;

public class EntryPoint : CursedModule
{
    public const string Version = "1.0.0.0";

    public override string ModuleName => "Origins SL";
    public override string ModuleAuthor => "Jesus-QC & xNexusACS";
    public override string ModuleVersion => Version;
    public override string CursedModVersion => CursedModInformation.Version;

    private Harmony _harmony;
    
    public static EntryPoint Instance { get; private set; }
    
    public override void OnLoaded()
    {
        Instance = this;
        
        ModuleLoader.Config = GetConfig<OriginsLoaderConfig>("loader");
        LevelingSystemModule.Config = GetConfig<LevelingConfig>("leveling");
        ServerStatusMessageModule.Config = GetConfig<ServerStatusMessageConfig>("serverstatusmessage");
        
        ModuleLoader.LoadModules();
        Log.Info("Loaded all modules");
        
        _harmony = new Harmony("origins.jesusqc.com");
        _harmony.PatchAll();
        
        base.OnLoaded();
    }
}
