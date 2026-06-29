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
    /// Hooks <see cref="BattleProgressATB.Update"/> to determine which updates should be skipped.
    /// Also handles decrementing <see cref="BattleDelayState.DelayTimer"/>.
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns>True if <see cref="BattleProgressATB.Update"/> should be skipped.</returns>
    [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.Update))]
    [HarmonyPrefix]
    static bool ShouldAllowATBUpdate(BattleProgressATB __instance)
    {
        //Allow update to continue for conditions where update wouldnt occur anyway or if delay timer is expired.
        if(!__instance.isEnabled
            || BattlePlugManager.instance.BattleActExection.GetStatingAct()
            || !BattleUtility.IsStagingEnd()
            || (SystemConfig.instance.ATBBattleType == ATBBattleType.Wait && BattleUIManager.instance.IsWaiting())
            || BattleUIManager.instance.IsForceWaiting()
            || (Plugin.Config.SkipDelayWhileRunning.Value && BattleUIManager.instance.IsEscape())
            || !ModComponent.Instance.CurrentBattleDelayState.IsWaiting)
        {
            return true;
        }
        else
        {
            //Plugin.Log.LogInfo($"Begin: {typeof(BattleProgressATB).FullName}.{nameof(BattleProgressATB.Update)} DelayTimer={ModComponent.Instance.CurrentBattleDelayState.DelayTimer:F4} (delaying)");
            ModComponent.Instance.CurrentBattleDelayState.DelayTimer -= Time.deltaTime;

            return false;
        }
    }

    [HarmonyPatch(typeof(BattlePlugManager), nameof(BattlePlugManager.Start))]
    [HarmonyPostfix]
    static void InitStartBattle()
    {
        ModComponent.Instance.CurrentBattleDelayState.Reset();
    }

}
