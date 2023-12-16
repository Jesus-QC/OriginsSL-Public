using System.Collections.Generic;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;

namespace OriginsSL.Modules.LevelingSystem;

public class LevelingRateLimiter
{
    private readonly Dictionary<int, byte> _registeredTimes = new();
    private readonly byte _maxTimesPerRound;
    
    public LevelingRateLimiter(byte maxTimesPerRound)
    {
        _maxTimesPerRound = maxTimesPerRound;
        CursedRoundEventsHandler.RestartingRound += Clear;
    }
    
    public bool IsRateLimited(CursedPlayer player)
    {
        if (!player.TryGetId(out int id))
            return false;
        
        if (!_registeredTimes.ContainsKey(id))
            return false;

        return _registeredTimes[id] >= _maxTimesPerRound;
    }
        
    public void AddUsage(CursedPlayer player)
    {
        if (!player.TryGetId(out int id))
            return;

        if (!_registeredTimes.ContainsKey(id))
        {
            _registeredTimes.Add(id, 1);
            return;
        }
            
        _registeredTimes[id]++;
    }

    public void AddExpAndUsage(CursedPlayer player, int exp)
    {
        if (player.DoNotTrack)
            return;
        
        player.AddExp(exp);
        AddUsage(player);
    }
    
    public void AddExpWithCheck(CursedPlayer player, int exp)
    {
        if (player.DoNotTrack)
            return;
        
        if (IsRateLimited(player))
            return;
        
        player.AddExp(exp);
        AddUsage(player);
    }

    private void Clear()
    {
        _registeredTimes.Clear();
    }
}