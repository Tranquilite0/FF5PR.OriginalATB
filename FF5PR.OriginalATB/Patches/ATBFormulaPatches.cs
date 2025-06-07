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

            var condition = (Last.Defaine.ConditionType)id;
            var currAtb = battleProgressATB.GetAtbGaugeByUnitData(battleUnitData);
            if (currAtb == 0f)
            {
                battleProgressATB.ChangeATBGaugeByUnitData(battleUnitData, battleUnitData.MinATB());
            }

            Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.RemoveFunction)}({battleUnitData.GetUnitName()}, {condition})");
        }

        /// <summary>
        /// Override ATBCalc function to not depend on Agility or Weight stats or unit type.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="battleUnitData"></param>
        [HarmonyPatch(typeof(BattleProgressATBFF0), nameof(BattleProgressATBFF0.ATBCalc))]
        [HarmonyPostfix]
        static void ATBCalcHook(BattleProgressATBFF0 __instance, ref float __result, BattleUnitData battleUnitData)
        {
            if (Plugin.Config.ATBFormula.Value == ATBFormula.PixelRemaster)
            {
                return;
            }

            //TODO: more research into PR ATBCalc formula to see if a formula for ATBValue at 0 speed, 0 weight, 1.0 gamespeed can be deduced for a given deltaTime.
            //TODO: Alternatively, figure out the time/ATB bar% factor from the original (I know its 16 frames per ATB chunk, but I am not sure how big that chunk is.
            // This is my best guess at deducing the PR atbDelta formula with agi/weight factors removed.
            var atbDelta = Time.deltaTime * __instance.ATBCalcCoefficient * BattlePlugManager.Instance().ATBSpeed;

            if(Plugin.Config.ATBFormula.Value == ATBFormula.OriginalFillRate)
            {
                //Also apply haste/slow to atbDelta.
                atbDelta *= battleUnitData.timeMagnification;
            }

            //Hack to avoid trigging ATB reset after conditions removed
            atbDelta = atbDelta == 0f ? 0.01f : atbDelta;

            __result = atbDelta;
            //Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATBFF0.ATBCalc)}({__0.GetUnitName()})->{__result:F2}");
        }

        /// <summary>
        /// Just skip the original <see cref="BattleProgressATB.SetPreeMptive"/> (unless we are using original ATB formula).
        /// We are reimplementing this and at a slightly later time in the battle init sequence.
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetPreeMptive))]
        [HarmonyPrefix]
        static bool SetPreeMptivePrefix()
        {
            PreeMptiveState = BattlePlugManager.Instance().BattlePopPlug.GetResult();
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
            if(BattlePlugManager.Instance().BattleProgress.TryCast<BattleProgressATB>() is not BattleProgressATB battleProgressATB)
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
                //and we are ac
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

    }
}
