using HarmonyLib;
using Last.Battle;
using Last.Defaine.Master;
using Last.Management;
using Last.UI;
using Last.UI.KeyInput;
using UnityEngine;

namespace FF5PR.OriginalATB.Patches;

public static class BattleATBDelayPatches
{
    [HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.NonInit))]
    [HarmonyPostfix]
    static void SetTurnEndedOnBattleIdle()
    {
        //Plugin.Log.LogInfo($"  End: {typeof(BattleInfomationController).FullName}.{nameof(BattleInfomationController.NonInit)}");

        //NonInit() seems to always be called when going to the No-Selection-Windows-Open state.
        //So we know there are no turns active and the next CommandSelectInit() will be a new turn.
        ModComponent.Instance.CurrentBattleDelayState.TurnEnded();
    }

    [HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.CommandSelectInit))]
    [HarmonyPostfix]
    static void CheckStartOfTurn()
    {
        //Plugin.Log.LogInfo($"  End: {typeof(BattleInfomationController).FullName}.{nameof(BattleInfomationController.CommandSelectInit)}");
        if (ModComponent.Instance.CurrentBattleDelayState.IsNewTurn)
        {
            ModComponent.Instance.CurrentBattleDelayState.RestartDelayTimer();
        }
    }

    /// <summary>
    /// If the delay timer still has time, then just decrement the timer and skip ATB updates.
    /// There might be other battle things which still run (condition timers?),
    /// but the delay time is at most 4 seconds so it shouldnt be too big of a deal.
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateGaugeIncrease))]
    [HarmonyPrefix]
    static bool UpdateDelayTimer(BattleProgressATB __instance)
    {
        

        if (ModComponent.Instance.CurrentBattleDelayState.IsWaiting)
        {
            //Plugin.Log.LogInfo($"Begin: {typeof(BattleProgressATB).FullName}.{nameof(BattleProgressATB.UpdateGaugeIncrease)} DelayTimer={ModComponent.Instance.CurrentBattleDelayState.DelayTimer:F4} (skipping)");
            ModComponent.Instance.CurrentBattleDelayState.DelayTimer -= Time.deltaTime;

            return false;
        }

        //Plugin.Log.LogInfo($"Begin: {typeof(BattleProgressATB).FullName}.{nameof(BattleProgressATB.UpdateGaugeIncrease)} DelayTimer={ModComponent.Instance.CurrentBattleDelayState.DelayTimer:F4} (not skipping)");
        return true;
    }

    [HarmonyPatch(typeof(BattlePlugManager), nameof(BattlePlugManager.Start))]
    [HarmonyPostfix]
    static void InitStartBattle()
    {
        ModComponent.Instance.CurrentBattleDelayState.Reset();
    }

}
