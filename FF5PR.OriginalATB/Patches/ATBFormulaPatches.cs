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
        public static BattlePopPlug.PreeMptiveState PreeMptiveState { get; set; } = default;

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
            //TODO: Alternatively, figure out the time/ATB bar% factor from the original (I know its 16 frames per ATB chunk, but I am not sure how big that chunk is.
            // This is my best guess at deducing the PR atbDelta formula with agi/weight factors removed.
            var atbDelta = Time.deltaTime * __instance.ATBCalcCoefficient * BattlePlugManager.Instance().ATBSpeed;

            if(Plugin.Config.ATBFormula.Value == ATBFormula.OriginalNoHaste)
            {
                //Also apply haste/slow to atbDelta.
                atbDelta *= __0.timeMagnification;
            }

            __result = atbDelta;
            //Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATBFF0.ATBCalc)}({__0.GetUnitName()})->{__result:F2}");
        }

        /// <summary>
        /// Just skip the original <see cref="BattleProgressATB.SetPreeMptive"/>.
        /// We are reimplementing this and at a slightly later time in the battle init sequence.
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetPreeMptive))]
        [HarmonyPrefix]
        static bool SetPreeMptivePrefix()
        {
            PreeMptiveState = BattlePlugManager.Instance().BattlePopPlug.GetResult();
            //TODO: Just let the method run?
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

            foreach ((var unitData, var _) in battleProgressATB.gaugeStatusDictionary.ToManaged())
            {
                if (GameDetection.Version == GameVersion.FF5
                    && unitData.HasMasamuneEquipped())
                {
                    Plugin.Log.LogInfo($"Masamune found, setting ATB to: {BattleProgressATB.MaxATBGauge}");
                    battleProgressATB.ChangeATBGaugeByUnitData(unitData, BattleProgressATB.MaxATBGauge);

                    continue;
                }

                battleProgressATB.ChangeATBGaugeByUnitData(unitData, unitData.MinATB(PreeMptiveState));
            }

            if (Plugin.Config.AdvanceFirstTurn.Value)
            {
                battleProgressATB.AdvanceToNextTurn();
            }
        }
    }
}
