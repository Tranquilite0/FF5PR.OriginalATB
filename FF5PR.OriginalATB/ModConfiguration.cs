﻿using BepInEx.Configuration;
using System.ComponentModel;

namespace FF5PR.OriginalATB;

public enum ATBFormula
{
    [Description("Original (Haste/Slow part of min ATB)")]
    Original = 0,
    [Description("Original (Haste/Slow part of ATB fill rate)")]
    OriginalFillRate,
    [Description("Pixel Remaster")]
    PixelRemaster,
}

public sealed class ModConfiguration
{
    private ConfigFile _config;
    public ConfigEntry<ATBFormula> ATBFormula;
    public ConfigEntry<bool> AdvanceFirstTurn;
    public ConfigEntry<bool> MonsterAgiVariance;

    public ConfigEntry<bool> DelayAtTurnStart;
    public ConfigEntry<float> VerySlowDelayTime;
    public ConfigEntry<float> SlowDelayTime;
    public ConfigEntry<float> NormalDelayTime;
    public ConfigEntry<float> FastDelayTime;
    public ConfigEntry<float> VeryFastDelayTime;

    public ModConfiguration(ConfigFile config)
    {
        _config = config;
    }

    public void Init()
    {
        ATBFormula = _config.Bind(
             "ATB",
             nameof(ATBFormula),
             OriginalATB.ATBFormula.Original,
             $"""
             Choose which ATB Formula to use:
              - {FF5PR.OriginalATB.ATBFormula.Original}: Haste/Slow baked into minimum ATB
              - {FF5PR.OriginalATB.ATBFormula.OriginalFillRate}: Haste/Slow part of ATB fill rate
              - {FF5PR.OriginalATB.ATBFormula.PixelRemaster}: Unchanged ATB formula
             """
        );

        MonsterAgiVariance = _config.Bind(
             "ATB",
             nameof(MonsterAgiVariance),
             true,
             "Add a random 0, +1, or -1 to monster Agility/Speed at the start of battle."
        );

        AdvanceFirstTurn = _config.Bind(
             "ATB",
             nameof(AdvanceFirstTurn),
             true,
             "Automatically Advance ATB at the start of battle to the first turn."
        );

        DelayAtTurnStart = _config.Bind(
             "Delay Time",
             nameof(DelayAtTurnStart),
             true,
             "Pause the ATB for a short time at the start of a turn. In the original game the duration of this delay was the only thing the battle speed setting affected."
        );

        VerySlowDelayTime = _config.Bind(
             "Delay Time",
             nameof(VerySlowDelayTime),
             4f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Very Slow."
        );

        SlowDelayTime = _config.Bind(
             "Delay Time",
             nameof(SlowDelayTime),
             2f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Slow."
        );

        NormalDelayTime = _config.Bind(
             "Delay Time",
             nameof(NormalDelayTime),
             1f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Normal."
        );

        FastDelayTime = _config.Bind(
             "Delay Time",
             nameof(FastDelayTime),
             0.5f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Fast."
        );

        VeryFastDelayTime = _config.Bind(
             "Delay Time",
             nameof(VeryFastDelayTime),
             0.25f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Very Fast."
        );

    }
}
