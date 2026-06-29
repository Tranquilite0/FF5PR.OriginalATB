using BepInEx.Configuration;
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

public enum AbilityBehavior
{

    Original = 0,
    PixelRemaster,
}

public sealed class ModConfiguration(ConfigFile config)
{
    //ATB
    public ConfigEntry<ATBFormula> ATBFormula;
    public ConfigEntry<bool> AdvanceFirstTurn;
    public ConfigEntry<bool> MonsterAgiVariance;

    //Delay
    public ConfigEntry<bool> DelayAtTurnStart;
    public ConfigEntry<bool> SkipDelayWhileRunning;
    public ConfigEntry<float> VerySlowDelayTime;
    public ConfigEntry<float> SlowDelayTime;
    public ConfigEntry<float> NormalDelayTime;
    public ConfigEntry<float> FastDelayTime;
    public ConfigEntry<float> VeryFastDelayTime;

    //Duration
    public ConfigEntry<bool> SingHasteSlow;
    public ConfigEntry<AbilityBehavior> SingDuration;
    public ConfigEntry<AbilityBehavior> JumpDuration;
    public ConfigEntry<AbilityBehavior> FocusDuration;
    public ConfigEntry<AbilityBehavior> IainukiDuration;
    public ConfigEntry<AbilityBehavior> AimDuration;
    public ConfigEntry<AbilityBehavior> CheckDuration;
    public ConfigEntry<AbilityBehavior> ScanDuration;
    public ConfigEntry<AbilityBehavior> RecoverDuration;
    public ConfigEntry<AbilityBehavior> ReviveDuration;
    public ConfigEntry<AbilityBehavior> MimicDuration;

    public void Init()
    {
        ATBFormula = config.Bind(
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

        MonsterAgiVariance = config.Bind(
             "ATB",
             nameof(MonsterAgiVariance),
             true,
             "Add a random 0, +1, or -1 to monster Agility/Speed at the start of battle."
        );

        AdvanceFirstTurn = config.Bind(
             "ATB",
             nameof(AdvanceFirstTurn),
             true,
             "Automatically Advance ATB at the start of battle to the first Unit's turn."
        );

        DelayAtTurnStart = config.Bind(
             "Delay Time",
             nameof(DelayAtTurnStart),
             true,
             "Pause the ATB for a short time at the start of a turn. In the original game the duration of this delay was the only thing the battle speed setting affected."
        );

        SkipDelayWhileRunning = config.Bind(
             "Delay Time",
             nameof(SkipDelayWhileRunning),
             true,
             "Pause the delay timer so that ATBs fill while trying to run. Makes it so you don't have to wait out the current delay timer before you can run."
        );

        VerySlowDelayTime = config.Bind(
             "Delay Time",
             nameof(VerySlowDelayTime),
             4f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Very Slow."
        );

        SlowDelayTime = config.Bind(
             "Delay Time",
             nameof(SlowDelayTime),
             2f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Slow."
        );

        NormalDelayTime = config.Bind(
             "Delay Time",
             nameof(NormalDelayTime),
             1f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Normal."
        );

        FastDelayTime = config.Bind(
             "Delay Time",
             nameof(FastDelayTime),
             0.5f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Fast."
        );

        VeryFastDelayTime = config.Bind(
             "Delay Time",
             nameof(VeryFastDelayTime),
             0.25f,
             "Time to delay ATB in the command window when a player gets their turn when battle speed is set to Very Fast."
        );

        SingHasteSlow = config.Bind(
             "Duration",
             nameof(SingHasteSlow),
             true,
             "Patch the Bard's Sing command to use slow/haste status (like the original)."
        );

        SingDuration = config.Bind(
             "Duration",
             nameof(SingDuration),
             AbilityBehavior.Original,
             $"""
             Have the Song command increase stats in _ intervals:
              - {AbilityBehavior.Original}: 1 second
              - {AbilityBehavior.PixelRemaster}: 1.5 second
             """
        );

        JumpDuration = config.Bind(
             "Duration",
             nameof(JumpDuration),
             AbilityBehavior.Original,
             $"""
             Have the Jump command execute in:
              - {AbilityBehavior.Original}: 0.33 seconds (before jump) and 2.66 seconds (in air)
              - {AbilityBehavior.PixelRemaster}: 3.5 seconds (in air)
             """
        );

        FocusDuration = config.Bind(
             "Duration",
             nameof(FocusDuration),
             AbilityBehavior.Original,
             $"""
             Have the Focus command execute in:
              - {AbilityBehavior.Original}: 2 seconds
              - {AbilityBehavior.PixelRemaster}: 3 seconds
             """
        );

        IainukiDuration = config.Bind(
             "Duration",
             nameof(IainukiDuration),
             AbilityBehavior.Original,
             $"""
             Have the Iainuki command execute in:
              - {AbilityBehavior.Original}: 2 seconds
              - {AbilityBehavior.PixelRemaster}: 3 seconds
             """
        );

        AimDuration = config.Bind(
             "Duration",
             nameof(AimDuration),
             AbilityBehavior.Original,
             $"""
             Have the Aim command execute in:
              - {AbilityBehavior.Original}: 1/6 second
              - {AbilityBehavior.PixelRemaster}: 1/2 second
             """
        );

        CheckDuration = config.Bind(
             "Duration",
             nameof(CheckDuration),
             AbilityBehavior.Original,
             $"""
             Have the Check command execute in:
              - {AbilityBehavior.Original}: 1/6 second
              - {AbilityBehavior.PixelRemaster}: 1/2 second
             """
        );

        ScanDuration = config.Bind(
             "Duration",
             nameof(ScanDuration),
             AbilityBehavior.Original,
             $"""
             Have the Scan command execute in:
              - {AbilityBehavior.Original}: 1/6 second
              - {AbilityBehavior.PixelRemaster}: 1/2 second
             """
        );

        RecoverDuration = config.Bind(
             "Duration",
             nameof(RecoverDuration),
             AbilityBehavior.Original,
             $"""
             Have the Recover command execute in:
              - {AbilityBehavior.Original}: 1/6 second
              - {AbilityBehavior.PixelRemaster}: 1/2 second
             """
        );

        ReviveDuration = config.Bind(
             "Duration",
             nameof(ReviveDuration),
             AbilityBehavior.Original,
             $"""
             Have the Revive command execute in:
              - {AbilityBehavior.Original}: 1/6 second
              - {AbilityBehavior.PixelRemaster}: 1/2 second
             """
        );

        MimicDuration = config.Bind(
             "Duration",
             nameof(MimicDuration),
             AbilityBehavior.Original,
             $"""
             Have the Mimic command execute in:
              - {AbilityBehavior.Original}: 1/6 second
              - {AbilityBehavior.PixelRemaster}: 1/2 second
             """
        );
    }
}
