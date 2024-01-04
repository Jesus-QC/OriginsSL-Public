using OriginsSL.Loader;
using ServerOutput;

namespace OriginsSL.Modules.AutoRestartTool;

public class AutoRestartToolModule : OriginsModule
{
    public override void OnLoaded()
    {
        ServerLogs.AddLog(ServerLogs.Modules.Administrative, "Scheduled server restart after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
        ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
        ServerConsole.AddOutputEntry(default(ExitActionRestartEntry));
    }
}