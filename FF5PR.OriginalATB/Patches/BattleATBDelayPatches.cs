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
    [HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.IsWaiting))]
    [HarmonyPostfix]
    static void IsWaitingHook(BattleInfomationController __instance, ref bool __result)
    {
        //var battleType = SystemConfig.Instance()?.ATBBattleType;
        //var state = __instance.stateMachine?.Current;

        //if (battleType != ATBBattleType.Wait ||
        //    state != BattleInfomationController.State.CommandSelect)
        //{
        //    return;
        //}

        //__result = ModComponent.Instance.CurrentBattleState.IsWaiting;
        //TODO: this is NEVER called when ATB is active. Not sure how we are going to address this since the delay happens in active mode too in vanilla.
        //TODO: get rid of OriginalIsWaiting
        //TODO: Hook something in BattleProgressATB To halt ATB generation when the delay timer is running.
        //TODO: Alternatively. Create a Fake Wait mode state.
        ModComponent.Instance.CurrentBattleDelayState.OriginalIsWaiting = __result;
        __result |= ModComponent.Instance.CurrentBattleDelayState.IsWaiting;
    }

    [HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.NonInit))]
    [HarmonyPostfix]
    static void SetTurnEnded()
    {
        //NonInit() seems to always be called when going to the No-Selection-Windows-Open state.
        //So we know there are no turns active and the next CommandSelectInit() will be a new turn.
        ModComponent.Instance.CurrentBattleDelayState.TurnEnded();
    }

    [HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.CommandSelectInit))]
    [HarmonyPostfix]
    static void CheckStartOfTurn()
    {
        if(ModComponent.Instance.CurrentBattleDelayState.IsNewTurn)
        {
            ModComponent.Instance.CurrentBattleDelayState.RestartDelayTimer();
        }
    }

    //[HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.CommandSelectUpdate))]
    //[HarmonyPostfix]
    //static void UpdateDelayTimer()
    //{
    //    //Fortunately this method only seems to be called when you are idle in the root CommandSelect window.
    //    //TODO: Should work correctly when in Wait mode, but might not work correctly in active mode.
    //    //TODO: Might need to hook a different Update Function to have this work correctly in active mode.
    //    if (ModComponent.Instance.CurrentBattleDelayState.IsWaiting)
    //    {
    //        ModComponent.Instance.CurrentBattleDelayState.DelayTimer -= Time.deltaTime;
    //    }
    //}

    [HarmonyPatch(typeof(BattleInfomationController), nameof(BattleInfomationController.UpdateController))]
    [HarmonyPostfix]
    static void UpdateDelayTimer(BattleInfomationController __instance)
    {
        //Only tick down the timer if it has time on it
        // and we arent already waiting for normal reasons.
        if (ModComponent.Instance.CurrentBattleDelayState.IsWaiting &&
            !ModComponent.Instance.CurrentBattleDelayState.OriginalIsWaiting)
        {
            ModComponent.Instance.CurrentBattleDelayState.DelayTimer -= Time.deltaTime;
        }
    }

    [HarmonyPatch(typeof(BattlePlugManager), nameof(BattlePlugManager.Start))]
    [HarmonyPostfix]
    static void InitStartBattle()
    {
        ModComponent.Instance.CurrentBattleDelayState.Reset();
    }

    

    //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetActData))]
    //[HarmonyPostfix]
    //static void NextTurn(BattleProgressATB __instance, BattleActData battleActData)
    //{
    //    if (battleActData == null ||
    //        battleActData.AttackUnitData == null)
    //    {
    //        return;
    //    }

    //    // Ignore units who do nothing during their turn. The game doesn't even indicate in any way to the player they didn't act.
    //    var nonAct = false;
    //    if (battleActData.CurrentUseType == BattleActData.UseType.Ability)
    //    {
    //        nonAct = true;

    //        var abilities = battleActData.abilityList;
    //        if (abilities != null)
    //        {
    //            foreach (var ability in abilities)
    //            {
    //                if (ability.TypeId != (int)AbilityType.NonAct)
    //                {
    //                    nonAct = false;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    if (!nonAct)
    //    {
    //        ModComponent.Instance.CurrentBattleState.TurnPassed();
    //    }
    //}

    //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateAlways))]
    //[HarmonyPostfix]
    //static void Update(BattleProgressATB __instance)
    //{
    //    ref var battleState = ref ModComponent.Instance.CurrentBattleState;

    //    // Don't pause when escaping
    //    var escaping = BattleUIManager.Instance?.IsEscape() ?? false;
    //    battleState.Escaping = escaping;

    //    // Pause when we pass a turn and an other ally is ready
    //    var playerUnitsReadyCount = __instance.battleATBOrderUiStack?.orderUiStack?.Count ?? 0;
    //    if (playerUnitsReadyCount != battleState.PlayerUnitsReadyCount)
    //    {
    //        battleState.TurnPassed();
    //        battleState.PlayerUnitsReadyCount = playerUnitsReadyCount;
    //    }
        
    //}
}
