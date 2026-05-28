using HarmonyLib;
using Il2CppSystem.Linq;
using Last.Battle;
using Last.Data.User;
using Last.Defaine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace FF5PR.OriginalATB.Patches
{
    public static class ATBFormulaPatches
    {
        public static BattlePopPlug.PreeMptiveState PreeMptiveState { get; set; } = default;

        /// <summary>
        /// Reset ATB to MinATB at end of turn instead of 0.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="battleUnitData"></param>
        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.ActExectionEndDelegate))]
        [HarmonyPostfix]
        static void ResetATBToMinOnAction(BattleProgressATB __instance, BattleUnitData battleUnitData)
        {
            if (Plugin.Config.ATBFormula.Value == ATBFormula.PixelRemaster)
            {
                return;
            }

            if (__instance.GetAtbGaugeByUnitData(battleUnitData) >= BattleProgressATB.MaxATBGauge)
            {
                //Dont reset if ATB is still full (Quick).
                return;
            }

            __instance.ChangeATBGaugeByUnitData(battleUnitData, battleUnitData.MinATB());
        }

        [HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.RemoveFunction))]
        [HarmonyPostfix]
        static void ResetATBToMinOnConditionRecover(BattleConditionController __instance, BattleUnitData battleUnitData, int id)
        {
            if (Plugin.Config.ATBFormula.Value == ATBFormula.PixelRemaster
                || BattlePlugManager.Instance().BattleProgress.TryCast<BattleProgressATB>() is not BattleProgressATB battleProgressATB)
            {
                return;
            }

            //var condition = (Last.Defaine.ConditionType)id;
            var currAtb = battleProgressATB.GetAtbGaugeByUnitData(battleUnitData);
            if (currAtb == 0f)
            {
                battleProgressATB.ChangeATBGaugeByUnitData(battleUnitData, battleUnitData.MinATB());
            }

            //Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.RemoveFunction)}({battleUnitData.GetUnitName()}, {condition})");
        }

        /// <summary>
        /// Override ATBCalc function to account for current ATB Formula.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="battleUnitData"></param>
        [HarmonyPatch(typeof(BattleProgressATBFF0), nameof(BattleProgressATBFF0.ATBCalc))]
        [HarmonyPostfix]
        static void ATBCalcHook(BattleProgressATBFF0 __instance, ref float __result, BattleUnitData battleUnitData)
        {
            //var derivedResult = battleUnitData.CalcATBFF0ForDelta(Time.deltaTime);
            //if(__result != derivedResult)
            //{
            //    Plugin.Log.LogInfo($"ATB Calc Mismatch: {battleUnitData.GetUnitName()}=>{__result} != {derivedResult}");
            //}

            //No need to recalculate ATB Delta for PR.
            if (Plugin.Config.ATBFormula.Value == ATBFormula.PixelRemaster)
            {
                return;
            }

            __result = battleUnitData.CalcATBForDelta();

            //Hack to avoid trigging ATB reset after conditions are removed
            __result = __result == 0f ? 0.01f : __result;

            //Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATBFF0.ATBCalc)}({battleUnitData.GetUnitName()})->{__result:F2}");
        }

        /// <summary>
        /// Just skip the original <see cref="BattleProgressATB.SetPreeMptive"/> (unless we are using original ATB formula).
        /// We are reimplementing this and at a slightly later time in the battle init sequence after conditions have been applied.
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetPreeMptive))]
        [HarmonyPrefix]
        static bool SetPreeMptivePrefix()
        {
            PreeMptiveState = BattlePlugManager.Instance().BattlePopPlug.GetResult();
            //Plugin.Log.LogInfo($"PreeMptiveState={PreeMptiveState}");
            if (Plugin.Config.ATBFormula.Value == ATBFormula.PixelRemaster)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set start of fight ATB at the earliest possible point I could find in the battle
        /// setup routines where Auto-Haste effects have been applied.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(typeof(BattleController), nameof(BattleController.StartInBattle))]
        [HarmonyPrefix]
        static void LateSetPreeMptive(BattleController __instance)
        {
            if (BattlePlugManager.Instance().BattleProgress.TryCast<BattleProgressATB>() is not BattleProgressATB battleProgressATB)
            {
                Plugin.Log.LogError("BattlePlugManager.BattleProgress is not BattleProgressATB!");
                return;
            }

            if (Plugin.Config.ATBFormula.Value != ATBFormula.PixelRemaster)
            {
                foreach ((var unitData, var _) in battleProgressATB.gaugeStatusDictionary.ToManaged())
                {
                    if (GameDetection.Version == GameVersion.FF5
                        && unitData.HasMasamuneEquipped())
                    {
                        //Plugin.Log.LogInfo($"Masamune found, setting ATB to: {BattleProgressATB.MaxATBGauge}");
                        battleProgressATB.ChangeATBGaugeByUnitData(unitData, BattleProgressATB.MaxATBGauge);

                        continue;
                    }

                    battleProgressATB.ChangeATBGaugeByUnitData(unitData, unitData.MinATB(PreeMptiveState));
                }
            }

            if (Plugin.Config.AdvanceFirstTurn.Value)
            {
                battleProgressATB.AdvanceToNextTurn();
            }
        }

        [HarmonyPatch(typeof(BattleUnitData), nameof(BattleUnitData.SetTimeMagnification))]
        [HarmonyPrefix]
        static void ApplySlowOrHaste(BattleUnitData __instance, float timeMagnification)
        {
            //Only apply change when using the original ATB formula,
            if (Plugin.Config.ATBFormula.Value == ATBFormula.Original
                //and our time magnification has actually changed,
                && __instance.timeMagnification != timeMagnification
                && BattlePlugManager.Instance().BattleProgress.TryCast<BattleProgressATB>() is BattleProgressATB battleProgressATB)
            {
                var currAtb = battleProgressATB.GetAtbGaugeByUnitData(__instance);

                //Only apply if the character;s turn is not already up
                if (currAtb < BattleProgressATB.MaxATBGauge
                    && timeMagnification != 1.0f)
                {
                    //ATB is range of 0 +- BattleProgressATB.MaxATBGauge
                    //Shift currAtb back to inverted absolute range (0 to 2*BattleProgressATB.MaxATBGauge), apply inverse timeMagnification, then invert and shift back.
                    //Some of these operations might be unnescesarry, but I dont want to spend any more time wrapping my head around the math.
                    currAtb = (BattleProgressATB.MaxATBGauge - currAtb) / timeMagnification; //Invert and apply time magnification
                    currAtb = Math.Clamp(BattleProgressATB.MaxATBGauge - currAtb, -BattleProgressATB.MaxATBGauge, BattleProgressATB.MaxATBGauge); //Re-invert and clamp
                    battleProgressATB.ChangeATBGaugeByUnitData(__instance, currAtb);
                }
            }
        }

        [HarmonyPatch(typeof(BattleStatusControl), nameof(BattleStatusControl.CreateBattleEnemyData))]
        [HarmonyPostfix]
        static void AddEnemyAgilityVariance(ref BattleEnemyData __result)
        {
            int agiIncrement = CommonUtility.GetRand(-1, 2);
            //Plugin.Log.LogInfo($"Adjusting {__result.GetUnitName()} Agility {__result.BattleUnitDataInfo.Parameter.BaseAgility} -> {__result.BattleUnitDataInfo.Parameter.BaseAgility + agiIncrement} ({agiIncrement:+0;-#})");
            __result.BattleUnitDataInfo.Parameter.BaseAgility += agiIncrement;
        }

        [HarmonyPatch(typeof(SongConditionFunction), nameof(SongConditionFunction.Start))]
        [HarmonyPostfix]
        static void OverrideSongConditionFunctionUpdateCycle(ref SongConditionFunction __instance)
        {
            __instance.updateCycle = Plugin.Config.SingDuration.Value == AbilityBehavior.Original ? 2 : 3;
        }

        [HarmonyPatch(typeof(SongConditionFunction), nameof(SongConditionFunction.Update))]
        [HarmonyPrefix]
        static void AdjustSongTimeForTimeMagnification(ref SongConditionFunction __instance)
        {
            //Song Condition normally increments currentTime at the rate of 2 * ATBSpeed * deltaTime.
            //We want to correct it so that the formula becomes: 2 * ATBSpeed * deltaTime * timeMagnification.

            //Bail if we dont need to make any adjustments.
            if (__instance.currentTime >= __instance.updateCycle 
                || __instance.BattleUnitData.timeMagnification == 1.0f
                || BattlePlugManager.instance is not BattlePlugManager battlePlugManager)
            {
                return;
            }

            var atbSpeed = battlePlugManager.ATBSpeed;

            //Undo the correction the update function is about to do while also applying our time-magnified version.
            __instance.currentTime += (atbSpeed + atbSpeed) * Time.deltaTime * (__instance.BattleUnitData.timeMagnification - 1);
        }

        [HarmonyPatch(typeof(BattleProgress), nameof(BattleProgress.GetChantingTime))]
        [HarmonyPostfix]
        static void OverrideAbilityWaitTimes(ref float __result, BattleActData battleActData)
        {
            __result = battleActData.abilityList.ToManaged().Select(x => x.Id).FirstOrDefault() switch
            {
                //Wait times should be multiplied by two to get realtime since these timers tick at double speed.

                //Jump has two ability IDs (one for jumping up, the other for air time)
                13 => Plugin.Config.JumpDuration.Value == AbilityBehavior.Original ? 2f/3f : __result,
                607 => Plugin.Config.JumpDuration.Value == AbilityBehavior.Original ? 16f/3f : __result,
                //Focus (also has two ability IDs but we only care about the charging one)
                821 => Plugin.Config.FocusDuration.Value == AbilityBehavior.Original ? 4f : __result,
                //Iainuki
                20 => Plugin.Config.IainukiDuration.Value == AbilityBehavior.Original ? 4f : __result,
                //Aim
                22 => Plugin.Config.AimDuration.Value == AbilityBehavior.Original ? 1f/3f : __result,
                //Check
                25 => Plugin.Config.CheckDuration.Value == AbilityBehavior.Original ? 1f/3f : __result,
                //Scan
                26 => Plugin.Config.ScanDuration.Value == AbilityBehavior.Original ? 1f/3f : __result,
                //Recover
                33 => Plugin.Config.RecoverDuration.Value == AbilityBehavior.Original ? 1f/3f : __result,
                //Revive
                34 => Plugin.Config.ReviveDuration.Value == AbilityBehavior.Original ? 1f/3f : __result,
                //Mimic
                44 => Plugin.Config.MimicDuration.Value == AbilityBehavior.Original ? 1f/3f : __result,

                _ => __result
            };
        }
    }
}
