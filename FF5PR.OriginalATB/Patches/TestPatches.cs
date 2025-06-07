using HarmonyLib;
using Last.Battle;
using Last.Data.User;
using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace FF5PR.OriginalATB.Patches
{
    /// <summary>
    /// Maybe now I can Figure out whats going on.
    /// </summary>
    public static class TestPatches
    {
        //------- BattleProgressATBFF0 -------
        //[HarmonyPatch(typeof(BattleProgressATBFF0), nameof(BattleProgressATBFF0.Init))]
        //[HarmonyPrefix]
        //static void InitPrefix(BattleProgressATBFF0 __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATBFF0.Init)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATBFF0), nameof(BattleProgressATBFF0.Init))]
        //[HarmonyPostfix]
        //static void InitPostfix(BattleProgressATBFF0 __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATBFF0.Init)}");
        //}

        [HarmonyPatch(typeof(BattleProgressATBFF0), nameof(BattleProgressATBFF0.ATBCalc))]
        [HarmonyPrefix]
        static void ATBCalcPre(BattleProgressATBFF0 __instance, BattleUnitData __0)
        {
            Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATBFF0.ATBCalc)}({__0.GetUnitName()})");
        }

        [HarmonyPatch(typeof(BattleProgressATBFF0), nameof(BattleProgressATBFF0.ATBCalc))]
        [HarmonyPostfix]
        static void ATBCalcPost(BattleProgressATBFF0 __instance, ref float __result, BattleUnitData __0)
        {
            Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATBFF0.ATBCalc)}({__0.GetUnitName()})->{__result:F2}");
        }


        //------- BattleProgressATB -------
        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.ActExectionEndDelegate))]
        [HarmonyPrefix]
        static void ActExectionEndDelegatePrefix(BattleProgressATB __instance, BattleUnitData __0)
        {
            Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.ActExectionEndDelegate)}({__0.GetUnitName()})");
        }

        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.ActExectionEndDelegate))]
        [HarmonyPostfix]
        static void ActExectionEndDelegatePostfix(BattleProgressATB __instance, BattleUnitData __0)
        {
            Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.ActExectionEndDelegate)}({__0.GetUnitName()})");
        }

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetPreeMptive))]
        //[HarmonyPrefix]
        //static void SetPreeMptivePre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.SetPreeMptive)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetPreeMptive))]
        //[HarmonyPostfix]
        //static void SetPreeMptivePost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.SetPreeMptive)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.Update))]
        //[HarmonyPrefix]
        //static void UpdatePre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.Update)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.Update))]
        //[HarmonyPostfix]
        //static void UpdatePost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.Update)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateAlways))]
        //[HarmonyPrefix]
        //static void UpdateAlwaysPre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateAlways)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateAlways))]
        //[HarmonyPostfix]
        //static void UpdateAlwaysPost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateAlways)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateGauge))]
        //[HarmonyPrefix]
        //static void UpdateGaugePre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateGauge)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateGauge))]
        //[HarmonyPostfix]
        //static void UpdateGaugePost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateGauge)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateUnableFight))]
        //[HarmonyPrefix]
        //static void UpdateUnableFightPre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateUnableFight)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateUnableFight))]
        //[HarmonyPostfix]
        //static void UpdateUnableFightPost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateUnableFight)}");
        //}

        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateGaugeIncrease))]
        [HarmonyPrefix]
        static void UpdateGaugeIncreasePre(BattleProgressATB __instance)
        {
            Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateGaugeIncrease)}");
        }

        [HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateGaugeIncrease))]
        [HarmonyPostfix]
        static void UpdateGaugeIncreasePost(BattleProgressATB __instance)
        {
            Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateGaugeIncrease)}");
        }

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateChantTime))]
        //[HarmonyPrefix]
        //static void UpdateChantTimePre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateChantTime)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateChantTime))]
        //[HarmonyPostfix]
        //static void UpdateChantTimePost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateChantTime)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateUI))]
        //[HarmonyPrefix]
        //static void UpdateUIPre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateUI)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateUI))]
        //[HarmonyPostfix]
        //static void UpdateUIPost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateUI)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateEscape))]
        //[HarmonyPrefix]
        //static void UpdateEscapePre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateEscape)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateEscape))]
        //[HarmonyPostfix]
        //static void UpdateEscapePost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateEscape)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateAlwaysEscape))]
        //[HarmonyPrefix]
        //static void UpdateAlwaysEscapePre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateAlwaysEscape)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.UpdateAlwaysEscape))]
        //[HarmonyPostfix]
        //static void UpdateAlwaysEscapePost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.UpdateAlwaysEscape)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.AddGaugeStatus))]
        //[HarmonyPrefix]
        //static void AddGaugeStatusPre(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.AddGaugeStatus)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.AddGaugeStatus))]
        //[HarmonyPostfix]
        //static void AddGaugeStatusPost(BattleProgressATB __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.AddGaugeStatus)}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.RemoveReadyAct))]
        //[HarmonyPrefix]
        //static void RemoveReadyActPre(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.RemoveReadyAct)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.RemoveReadyAct))]
        //[HarmonyPostfix]
        //static void RemoveReadyActPost(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.RemoveReadyAct)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.ChangeATBGaugeByUnitData))]
        //[HarmonyPrefix]
        //static void ChangeATBGaugeByUnitDataPre(BattleProgressATB __instance, BattleUnitData __0, float __1)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.ChangeATBGaugeByUnitData)}({__0.GetUnitName()}, {__1:f2})");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.ChangeATBGaugeByUnitData))]
        //[HarmonyPostfix]
        //static void ChangeATBGaugeByUnitDataPost(BattleProgressATB __instance, BattleUnitData __0, float __1)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.ChangeATBGaugeByUnitData)}({__0.GetUnitName()}, {__1:f2})");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.GetAtbGaugeByUnitData))]
        //[HarmonyPrefix]
        //static void GetAtbGaugeByUnitDataPre(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.GetAtbGaugeByUnitData)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.GetAtbGaugeByUnitData))]
        //[HarmonyPostfix]
        //static void GetAtbGaugeByUnitDataPost(BattleProgressATB __instance, ref float __result, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.GetAtbGaugeByUnitData)}({__0.GetUnitName()})->{__result:F2}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetNotUpdateATBGaugeUnit))]
        //[HarmonyPrefix]
        //static void SetNotUpdateATBGaugeUnitPre(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.SetNotUpdateATBGaugeUnit)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetNotUpdateATBGaugeUnit))]
        //[HarmonyPostfix]
        //static void SetNotUpdateATBGaugeUnitPost(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.SetNotUpdateATBGaugeUnit)}({__0.GetUnitName()}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.RemoveNotUpdateATBGaugeUnit))]
        //[HarmonyPrefix]
        //static void RemoveNotUpdateATBGaugeUnitPre(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.RemoveNotUpdateATBGaugeUnit)}({__0.GetUnitName()}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.RemoveNotUpdateATBGaugeUnit))]
        //[HarmonyPostfix]
        //static void RemoveNotUpdateATBGaugeUnitPost(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.RemoveNotUpdateATBGaugeUnit)}({__0.GetUnitName()}");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetNextTurn))]
        //[HarmonyPrefix]
        //static void SetNextTurnPre(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleProgressATB.SetNextTurn)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleProgressATB), nameof(BattleProgressATB.SetNextTurn))]
        //[HarmonyPostfix]
        //static void SetNextTurnPost(BattleProgressATB __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleProgressATB.SetNextTurn)}({__0.GetUnitName()}");
        //}

        ////------- BattleUnitData -------
        //[HarmonyPatch(typeof(BattleUnitData), nameof(BattleUnitData.SetTimeMagnification))]
        //[HarmonyPrefix]
        //static void ChangeATBGaugeByUnitDataPre(BattleUnitData __instance, float __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}({__instance.UnitName()}).{nameof(BattleUnitData.SetTimeMagnification)}({__0:f2})");
        //}

        //[HarmonyPatch(typeof(BattleUnitData), nameof(BattleUnitData.SetTimeMagnification))]
        //[HarmonyPostfix]
        //static void ChangeATBGaugeByUnitDataPost(BattleUnitData __instance, float __0)
        //{
        //    Plugin.Log.LogInfo($"     : {__instance.GetType().FullName}({__instance.GetUnitName()}).{nameof(BattleUnitData.SetTimeMagnification)}({__0:f2})");
        //}

        ////------- BattlePlugManager -------
        //[HarmonyPatch(typeof(BattlePlugManager), nameof(BattlePlugManager.Init))]
        //[HarmonyPrefix]
        //static void InitPrefix(BattlePlugManager __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattlePlugManager.Init)}");
        //}

        //[HarmonyPatch(typeof(BattlePlugManager), nameof(BattlePlugManager.Init))]
        //[HarmonyPostfix]
        //static void InitPostfix(BattlePlugManager __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattlePlugManager.Init)}");
        //}

        //[HarmonyPatch(typeof(BattlePlugManager), nameof(BattlePlugManager.Start))]
        //[HarmonyPrefix]
        //static void StartPrefix(BattlePlugManager __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattlePlugManager.Start)}");
        //}

        //[HarmonyPatch(typeof(BattlePlugManager), nameof(BattlePlugManager.Start))]
        //[HarmonyPostfix]
        //static void StartPostfix(BattlePlugManager __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattlePlugManager.Start)}");
        //}

        ////------- BattleConditionController -------
        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.Init))]
        //[HarmonyPrefix]
        //static void InitPrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.Init)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.Init))]
        //[HarmonyPostfix]
        //static void InitPostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.Init)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.SetStartCondition))]
        //[HarmonyPrefix]
        //static void SetStartConditionPrefix(BattleConditionController __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.SetStartCondition)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.SetStartCondition))]
        //[HarmonyPostfix]
        //static void SetStartConditionPostfix(BattleConditionController __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.SetStartCondition)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.Update))]
        //[HarmonyPrefix]
        //static void UpdatePrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.Update)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.Update))]
        //[HarmonyPostfix]
        //static void UpdatePostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.Update)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.ActEndUpdate))]
        //[HarmonyPrefix]
        //static void ActEndUpdatePrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.ActEndUpdate)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.ActEndUpdate))]
        //[HarmonyPostfix]
        //static void ActEndUpdatePostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.ActEndUpdate)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CalcEndUpdate))]
        //[HarmonyPrefix]
        //static void CalcEndUpdatePrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.CalcEndUpdate)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CalcEndUpdate))]
        //[HarmonyPostfix]
        //static void CalcEndUpdatePostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.CalcEndUpdate)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.EntityUpdate))]
        //[HarmonyPrefix]
        //static void EntityUpdatePrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.EntityUpdate)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.EntityUpdate))]
        //[HarmonyPostfix]
        //static void EntityUpdatePostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.EntityUpdate)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.UnitsCalc))]
        //[HarmonyPrefix]
        //static void UnitsCalcPrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.UnitsCalc)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.UnitsCalc))]
        //[HarmonyPostfix]
        //static void UnitsCalcPostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.UnitsCalc)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CheckAddCondition))]
        //[HarmonyPatch([])]
        //[HarmonyPrefix]
        //static void CheckAddConditionPrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.CheckAddCondition)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CheckAddCondition))]
        //[HarmonyPatch([])]
        //[HarmonyPostfix]
        //static void CheckAddConditionPostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.CheckAddCondition)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CheckAddCondition))]
        //[HarmonyPatch([typeof(BattleUnitData)])]
        //[HarmonyPrefix]
        //static void CheckAddConditionPrefix2(BattleConditionController __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.CheckAddCondition)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CheckAddCondition))]
        //[HarmonyPatch([typeof(BattleUnitData)])]
        //[HarmonyPostfix]
        //static void CheckAddConditionPostfix2(BattleConditionController __instance, BattleUnitData __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.CheckAddCondition)}({__0.GetUnitName()})");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.EquipmentUpdate))]
        //[HarmonyPrefix]
        //static void EquipmentUpdatePrefix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.EquipmentUpdate)}");
        //}

        //[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.EquipmentUpdate))]
        //[HarmonyPostfix]
        //static void EquipmentUpdatePostfix(BattleConditionController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.EquipmentUpdate)}");
        //}

        ////[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CheckConditionFunction))]
        ////[HarmonyPrefix]
        ////static void CheckConditionFunctionPrefix(BattleConditionController __instance, BattleUnitData __0)
        ////{
        ////    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleConditionController.CheckConditionFunction)}({__0.GetUnitName()})");
        ////}

        ////[HarmonyPatch(typeof(BattleConditionController), nameof(BattleConditionController.CheckConditionFunction))]
        ////[HarmonyPostfix]
        ////static void CheckConditionFunctionPostfix(BattleConditionController __instance, BattleUnitData __0)
        ////{
        ////    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleConditionController.CheckConditionFunction)}({__0.GetUnitName()})");
        ////}

        ////------- BattleConditionController -------
        //[HarmonyPatch(typeof(BattlePopPlug), nameof(BattlePopPlug.Calculation))]
        //[HarmonyPrefix]
        //static void CalculationPrefix(BattlePopPlug __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattlePopPlug.Calculation)}");
        //}

        //[HarmonyPatch(typeof(BattlePopPlug), nameof(BattlePopPlug.Calculation))]
        //[HarmonyPostfix]
        //static void CalculationPostfix(BattlePopPlug __instance, BattlePopPlug.PreeMptiveState __result)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattlePopPlug.Calculation)}->{__result}");
        //}

        //[HarmonyPatch(typeof(BattlePopPlug), nameof(BattlePopPlug.SetPreeMptiveState))]
        //[HarmonyPrefix]
        //static void SetPreeMptiveStatePrefix(BattlePopPlug __instance, BattlePopPlug.PreeMptiveState __0)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattlePopPlug.SetPreeMptiveState)}({__0})");
        //}

        //[HarmonyPatch(typeof(BattlePopPlug), nameof(BattlePopPlug.SetPreeMptiveState))]
        //[HarmonyPostfix]
        //static void SetPreeMptiveStatePostfix(BattlePopPlug __instance, BattlePopPlug.PreeMptiveState __0)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattlePopPlug.SetPreeMptiveState)}({__0})");
        //}

        ////[HarmonyPatch(typeof(BattlePopPlug), nameof(BattlePopPlug.GetResult))]
        ////[HarmonyPostfix]
        ////static void GetResultPostfix(BattlePopPlug __instance, BattlePopPlug.PreeMptiveState __result)
        ////{
        ////    Plugin.Log.LogInfo($"     : {__instance.GetType().FullName}.{nameof(BattlePopPlug.GetResult)}->{__result}");
        ////}

        ////------- BattleController -------

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StateChange))]
        //[HarmonyPostfix]
        //static void StateChangePostfix(BattleController __instance, BattleController.BattleState __0)
        //{
        //    Plugin.Log.LogInfo($"     : {__instance.GetType().FullName}.{nameof(BattleController.StateChange)}({__0})");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartBattle))]
        //[HarmonyPatch([])]
        //[HarmonyPrefix]
        //static void StartBattlePrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.StartBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartBattle))]
        //[HarmonyPatch([])]
        //[HarmonyPostfix]
        //static void StartBattlePostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.StartBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartBattle))]
        //[HarmonyPatch([typeof(InstantiateManager), typeof(bool), typeof(bool), typeof(int)])]
        //[HarmonyPrefix]
        //static void StartBattlePrefix2(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.StartBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartBattle))]
        //[HarmonyPatch([typeof(InstantiateManager), typeof(bool), typeof(bool), typeof(int)])]
        //[HarmonyPostfix]
        //static void StartBattlePostfix2(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.StartBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartInit))]
        //[HarmonyPrefix]
        //static void StartInitPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.StartInit)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartInit))]
        //[HarmonyPostfix]
        //static void StartInitPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.StartInit)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.UpdateInit))]
        //[HarmonyPrefix]
        //static void UpdateInitPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.UpdateInit)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.UpdateInit))]
        //[HarmonyPostfix]
        //static void UpdateInitPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.UpdateInit)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartCreateObj))]
        //[HarmonyPrefix]
        //static void StartCreateObjPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.StartCreateObj)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartCreateObj))]
        //[HarmonyPostfix]
        //static void StartCreateObjPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.StartCreateObj)}");
        //}

        ////[HarmonyPatch(typeof(BattleController), nameof(BattleController.UpdateCreateObj))]
        ////[HarmonyPrefix]
        ////static void UpdateCreateObjPrefix(BattleController __instance)
        ////{
        ////    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.UpdateCreateObj)}");
        ////}

        ////[HarmonyPatch(typeof(BattleController), nameof(BattleController.UpdateCreateObj))]
        ////[HarmonyPostfix]
        ////static void UpdateCreateObjPostfix(BattleController __instance)
        ////{
        ////    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.UpdateCreateObj)}");
        ////}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitUniqueBattle))]
        //[HarmonyPrefix]
        //static void InitUniqueBattlePrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitUniqueBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitUniqueBattle))]
        //[HarmonyPostfix]
        //static void InitUniqueBattlePostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitUniqueBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartInBattle))]
        //[HarmonyPrefix]
        //static void StartInBattlePrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.StartInBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartInBattle))]
        //[HarmonyPostfix]
        //static void StartInBattlePostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.StartInBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartPlay))]
        //[HarmonyPrefix]
        //static void StartPlayPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.StartPlay)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.StartPlay))]
        //[HarmonyPostfix]
        //static void StartPlayPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.StartPlay)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitBattleData))]
        //[HarmonyPrefix]
        //static void InitBattleDataPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitBattleData)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitBattleData))]
        //[HarmonyPostfix]
        //static void InitBattleDataPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitBattleData)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitSetDelegate))]
        //[HarmonyPrefix]
        //static void InitSetDelegatePrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitSetDelegate)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitSetDelegate))]
        //[HarmonyPostfix]
        //static void InitSetDelegatePostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitSetDelegate)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitBattleFormation))]
        //[HarmonyPrefix]
        //static void InitBattleFormationPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitBattleFormation)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitBattleFormation))]
        //[HarmonyPostfix]
        //static void InitBattleFormationPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitBattleFormation)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.GetPreeMptiveState))]
        //[HarmonyPrefix]
        //static void GetPreeMptiveStatePrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.GetPreeMptiveState)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.GetPreeMptiveState))]
        //[HarmonyPostfix]
        //static void GetPreeMptiveStatePostfix(BattleController __instance, BattlePopPlug.PreeMptiveState __result)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.GetPreeMptiveState)}->{__result}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.ExitPreeMptive))]
        //[HarmonyPrefix]
        //static void ExitPreeMptivePrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.ExitPreeMptive)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.ExitPreeMptive))]
        //[HarmonyPostfix]
        //static void ExitPreeMptivePostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.ExitPreeMptive)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitInBattle))]
        //[HarmonyPrefix]
        //static void InitInBattlePrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitInBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitInBattle))]
        //[HarmonyPostfix]
        //static void InitInBattlePostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitInBattle)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitUnitStatus))]
        //[HarmonyPrefix]
        //static void InitUnitStatusPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitUnitStatus)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitUnitStatus))]
        //[HarmonyPostfix]
        //static void InitUnitStatusPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitUnitStatus)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitPlayerStatus))]
        //[HarmonyPrefix]
        //static void InitPlayerStatusPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitPlayerStatus)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitPlayerStatus))]
        //[HarmonyPostfix]
        //static void InitPlayerStatusPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitPlayerStatus)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitEnemyStatus))]
        //[HarmonyPrefix]
        //static void InitEnemyStatusPrefix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"Begin: {__instance.GetType().FullName}.{nameof(BattleController.InitEnemyStatus)}");
        //}

        //[HarmonyPatch(typeof(BattleController), nameof(BattleController.InitEnemyStatus))]
        //[HarmonyPostfix]
        //static void InitEnemyStatusPostfix(BattleController __instance)
        //{
        //    Plugin.Log.LogInfo($"  End: {__instance.GetType().FullName}.{nameof(BattleController.InitEnemyStatus)}");
        //}
    }
}
