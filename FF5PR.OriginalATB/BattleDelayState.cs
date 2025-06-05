using Last.Management;
using System;

namespace FF5PR.OriginalATB;

public class BattleDelayState
{
    //TODO Determine if Escaping should not wait (original waits even while trying to run, but unsure what actually happens).
    //public bool Escaping { get; set; }
    //public int PlayerUnitsReadyCount { get; set; }
    public bool IsNewTurn { get; set; } = false;
    public bool OriginalIsWaiting { get; set; }
    public float DelayTimer { get; set; }

    //private bool _wait;

    public bool IsWaiting => /*!Escaping && */ DelayTimer > 0f;

    public BattleDelayState()
    {
        Reset();
    }

    public void Reset()
    {
        //Escaping = false;
        //PlayerUnitsReadyCount = 0;
        IsNewTurn = false;
        OriginalIsWaiting = false;
        TurnEnded();
    }

    //public void TurnPassed()
    //{
    //    //_wait = true;
    //    RestartDelayTimer();
    //}

    //public void PassTurn()
    //{
    //    //_wait = false;
    //    DelayTimer = 0f;
    //}

    public void TurnEnded()
    {
        DelayTimer = 0f;
        IsNewTurn = true;
    }

    public void RestartDelayTimer()
    {
        var battleSpeed = (BattleSpeed)(Last.Management.UserDataManager.Instance()?.Config.BattleSpeed ?? 2);
        DelayTimer = battleSpeed switch
        {
            BattleSpeed.VerySlow => Plugin.Config.VerySlowDelayTime.Value,
            BattleSpeed.Slow => Plugin.Config.SlowDelayTime.Value,
            BattleSpeed.Fast => Plugin.Config.FastDelayTime.Value,
            BattleSpeed.VeryFast => Plugin.Config.VeryFastDelayTime.Value,
            _ => Plugin.Config.NormalDelayTime.Value,
        };
        IsNewTurn = false;
    }
}

public enum BattleSpeed : int
{
    VerySlow = 0,
    Slow,
    Normal,
    Fast,
    VeryFast,
}
