using HarmonyLib;
using Il2CppSystem.Linq;
using Last.Battle;
using Last.Data.User;
using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace FF5PR.OriginalATB.Patches
{
    public static class ATBFormulaPatches
    {
        private static bool isDuringInit = false;

        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.Init))]
        [HarmonyPrefix]
        static void InitPrefix()
        {
            isDuringInit = true;
        }

        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.Init))]
        [HarmonyPostfix]
        static void InitPostfix()
        {
            isDuringInit = false;
        }

        /// <summary>
        /// Reset ATB to MinATB at end of turn instead of 0.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__0"></param>
        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.ActExectionEndDelegate))]
        [HarmonyPostfix]
        static void ResetATBToMin(BattleProgressATB __instance, BattleUnitData __0)
        {
            if (Plugin.Config.ATBFormula.Value == ATBFormula.PixelRemaster)
            {
                return;
            }

            __instance.ChangeATBGaugeByUnitData(__0, __0.MinATB());
        }

        /// <summary>
        /// Override ATBCalc function to not depend on Agility or Weight stats or unit type.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__0"></param>
        [HarmonyPatch(typeof(BattleProgressATBFF0), nameof(BattleProgressATBFF0.ATBCalc))]
        [HarmonyPostfix]
        static void ATBCalcHook(BattleProgressATBFF0 __instance, ref float __result, BattleUnitData __0)
        {
            if (Plugin.Config.ATBFormula.Value == ATBFormula.PixelRemaster)
            {
                return;
            }

            //TODO: more research into PR ATBCalc formula to see if a formula for ATBValue at 0 speed, 0 weight, 1.0 gamespeed can be deduced for a given deltaTime.
            // This is my best guess at the PR atbDelta formula with agi/weight factors removed.
            var atbDelta = Time.deltaTime * __instance.ATBCalcCoefficient * BattlePlugManager.Instance().ATBSpeed;

            if(Plugin.Config.ATBFormula.Value == ATBFormula.OriginalNoHaste)
            {
                //Also apply haste/slow to atbDelta.
                atbDelta *= __0.timeMagnification;
            }

            __result = atbDelta;
        }

        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetPreeMptive))]
        [HarmonyPrefix]
        static bool SetPreeMptivePrefix()
        {
            return !isDuringInit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetPreeMptive))]
        [HarmonyPostfix]
        static void SetPreeMptiveHook(BattleProgressATB __instance)
        {
            //SetPreeMptive is called twice at the start of battle.
            //We only want to apply our changes in the second call.
            if (isDuringInit)
            {
                Plugin.Log.LogInfo($"Bailing from SetPreeMptive() call during Init().");
                return;
            }

            Plugin.Log.LogInfo($"Ding!");

            var state = BattlePlugManager.Instance().BattlePopPlug.GetResult();

            //For whatever reason, auto-haste from equipped hermes sandles has not been applied yet.
            //TODO: figure out how to get autohaste applied, or figure out how to postpone applying PreeMptive ATB changes until it is.
            // Try and force the component that applies the time magnification to run.
            //Plugin.Log.LogInfo($"Trying something potentially dumb...");
            //BattlePlugManager.Instance().BattleStatusControl.Update();
            //BattlePlugManager.Instance().BattleFieldConditionController.Update();
            //BattlePlugManager.Instance().BattleConditionController.CheckAddCondition();
            foreach ((var unitData, var _) in __instance.gaugeStatusDictionary.ToManaged())
            {
                //For whatever reason, the auto-haste from equipped hermes sandles might not have been applied yet.
                //BattlePlugManager.Instance().BattleConditionController.FlagCondition(unitData);
                
                //BattlePlugManager.Instance().BattleConditionController.CheckConditionFunction(unitData);
                //BattlePlugManager.Instance().BattleConditionController.CheckAddCondition(unitData);
                //BattlePlugManager.Instance().BattleConditionController.CheckConditionFunction(unitData);
                //BattlePlugManager.Instance().BattleConditionController.CheckAddCondition(unitData);

                //if (unitData.GetOwnedCharacterData() is not null)
                //{
                //    Plugin.Log.LogInfo($"ding!");
                //    unitData.SetTimeMagnification(2.0f);
                //}

                ////For whatever reason, the auto-haste from equipped hermes sandles might not have been applied yet.
                //if (unitData.BattleUnitDataInfo.GetConditionType().Contains(Last.Defaine.ConditionType.Heist))
                //{
                //    unitData.SetTimeMagnification(2.0f);
                //}

                if (GameDetection.Version == GameVersion.FF5
                    && unitData.HasMasamuneEquipped())
                {
                    Plugin.Log.LogInfo($"Masamune found, setting ATB to: {BattleProgressATB.MaxATBGauge}");
                    __instance.ChangeATBGaugeByUnitData(unitData, BattleProgressATB.MaxATBGauge);

                    continue;
                }

                __instance.ChangeATBGaugeByUnitData(unitData, unitData.MinATB(state));
            }

            if (Plugin.Config.AdvanceFirstTurn.Value)
            {
                __instance.AdvanceToNextTurn();
            }
        }
    }
}
