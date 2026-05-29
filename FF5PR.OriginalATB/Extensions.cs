using Last.Battle;
using Last.Data.Master;
using Last.Data.User;
using Last.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FF5PR.OriginalATB
{
    public static class Extensions
    {
        public const int MasamuneId = 66;
        public const int BackAttackPenalty = 60;
        public const int PreeMptivePenalty = 90;
        public const int ATBMinOriginal = 1;
        public const int ATBMaxOriginal = 255;

        public const int ATBCalcCoefficient = 64;
        public const float ATBCalcBossEnemyCoefficient = 0.90f;

        /// <summary>
        /// Accounts for realtime ATB filling differences between Original (0-128 and PR (0-100)
        /// </summary>
        public const float ATBGuageRescalingFactor = 100f / 127f;

        /// <summary>
        /// Calculate the minimum ATB value using the original FFV ATB formula rescaled to the new ATB system.
        /// </summary>
        /// <param name="battleUnitData">The unit to calculate min ATB for.</param>
        /// <returns>Min ATB value in range of 0 to <see cref="BattleProgressATB.MaxATBGauge"/>.</returns>
        public static float MinATB(this BattleUnitData battleUnitData, BattlePopPlug.PreeMptiveState preeMptiveState = BattlePopPlug.PreeMptiveState.Normal)
        {
            //Stats used in the formula.
            var agi = battleUnitData.BattleUnitDataInfo.Parameter.ConfirmedAgility();
            var weight = battleUnitData.BattleUnitDataInfo.Parameter.ConfirmedWeight();
            //As far as I know, this is always 0.5 when slow 2.0 when hasted and 1.0 otherwise.
            var timeMagnification = Plugin.Config.ATBFormula.Value == ATBFormula.Original ? battleUnitData.timeMagnification : 1.0f;

            //Plugin.Log.LogInfo($"Calculating MinATB for {battleUnitData.GetUnitName()}:");
            //Plugin.Log.LogInfo($"Inputs: agi={agi}, weight={weight}, mult={timeMagnification}, state={preeMptiveState}");

            //The original ATB formula calculates integer values in the range of 1-255 (lower is fuller ATB).
            //We need to invert the range so that higher is fuller and rescale the range to BattleProgressATB.MaxATBGauge.
            //We'll stick to using integer math at first for "authenticity" and convert to float at the end.
            var atbMinInt = Math.Clamp(120 - agi + (weight / 8), ATBMinOriginal, ATBMaxOriginal);
            atbMinInt = Math.Clamp((int)(atbMinInt / timeMagnification), ATBMinOriginal, ATBMaxOriginal);
            //Plugin.Log.LogInfo($"PreeMptivePenalty: {CalcPreeMptivePenalty(battleUnitData, preeMptiveState)}");
            atbMinInt = Math.Clamp(atbMinInt + CalcPreeMptivePenalty(battleUnitData, preeMptiveState), ATBMinOriginal, ATBMaxOriginal);

            //Plugin.Log.LogInfo($"MinATB Original: {atbMinInt}");

            //Invert range so that 0 is empty and 255 is full
            atbMinInt = ATBMaxOriginal - atbMinInt;

            //The original game actually shows as "empty" at 127
            //So shift range to be -126 (below empty) to 0 (empty) to 127 (full)
            atbMinInt -= ATBMaxOriginal / 2;

            //Plugin.Log.LogInfo($"MinATB Invert+shift: {atbMinInt}");

            //Rescale shifted original ATB range (1 +/- to 127) to match PR (1 +- 100).
            var atbMinFloat = atbMinInt * BattleProgressATB.MaxATBGauge / (ATBMaxOriginal / 2);
            //Plugin.Log.LogInfo($"MinATB Rescaled: {atbMinFloat:F2}");

            //Hack to avoid trigging ATB reset after conditions
            atbMinFloat = atbMinFloat == 0f ? 0.01f : atbMinFloat;

            return atbMinFloat;
        }

        public static float CalcUnitSpeedCoefficient(this BattleUnitData battleUnitData)
        {
            var agi = battleUnitData.BattleUnitDataInfo.Parameter.ConfirmedAgility();
            var weight = battleUnitData.BattleUnitDataInfo.Parameter.ConfirmedWeight();
            var speedCoeff = (float)(agi - weight) / ATBCalcCoefficient + 1.0f;
            if(battleUnitData.GetMonster() is Monster monsterData && monsterData.IsBoss())
            {
                speedCoeff *= ATBCalcBossEnemyCoefficient;
            }

            return speedCoeff;
        }

        /// <summary>
        /// Calculates ATB Coefficient for given <paramref name="battleUnitData"/>.
        /// Should give (almost) the same value as <see cref="BattleProgressATBFF0.ATBCalc(BattleUnitData)"/> when multiplied by <see cref="Time.deltaTime"/>.
        /// Note: you wont get the exact same value since floating point math is sensitive to order of operations for rounding error.
        ///  Use 
        /// </summary>
        /// <param name="battleUnitData"></param>
        /// <returns></returns>
        public static float CalcATBFF0Coefficient(this BattleUnitData battleUnitData) =>
            BattleProgressATBFF0.UpdateFPSATB * battleUnitData.CalcUnitSpeedCoefficient() * BattlePlugManager.Instance().ATBSpeed * battleUnitData.timeMagnification;

        /// <summary>
        /// Should exactly match the value returned by <see cref="BattleProgressATBFF0.ATBCalc(BattleUnitData)"/> for the same <see cref="Time.deltaTime"/>.
        /// </summary>
        /// <param name="battleUnitData"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static float CalcATBFF0ForDelta(this BattleUnitData battleUnitData, float deltaTime) =>
            deltaTime * BattleProgressATBFF0.UpdateFPSATB * battleUnitData.CalcUnitSpeedCoefficient() * BattlePlugManager.Instance().ATBSpeed * battleUnitData.timeMagnification;

        /// <summary>
        /// Should exactly match the value returned by <see cref="BattleProgressATBFF0.ATBCalc(BattleUnitData)"/>.
        /// </summary>
        /// <param name="battleUnitData"></param>
        /// <returns></returns>
        public static float CalcATBFF0ForDelta(this BattleUnitData battleUnitData) => CalcATBFF0ForDelta(battleUnitData, Time.deltaTime);

        /// <summary>
        /// Calculate <paramref name="battleUnitData"/> ATB Delta for a given <paramref name="deltaTime"/> and <paramref name="atbFormula"/>.
        /// </summary>
        /// <param name="battleUnitData"></param>
        /// <param name="deltaTime"></param>
        /// <param name="atbFormula"></param>
        /// <returns></returns>
        public static float CalcATBForDelta(this BattleUnitData battleUnitData, float deltaTime, ATBFormula atbFormula) => atbFormula switch
        {
            ATBFormula.Original => deltaTime * BattleProgressATBFF0.UpdateFPSATB * BattlePlugManager.Instance().ATBSpeed * ATBGuageRescalingFactor,
            ATBFormula.OriginalFillRate => deltaTime * BattleProgressATBFF0.UpdateFPSATB * BattlePlugManager.Instance().ATBSpeed * battleUnitData.timeMagnification * ATBGuageRescalingFactor,
            ATBFormula.PixelRemaster => deltaTime * BattleProgressATBFF0.UpdateFPSATB * battleUnitData.CalcUnitSpeedCoefficient() * BattlePlugManager.Instance().ATBSpeed * battleUnitData.timeMagnification,
            _ => 0.0f
        };

        /// <summary>
        /// Calculate <paramref name="battleUnitData"/> ATB Delta for a given <paramref name="deltaTime"/> using <see cref="ModConfiguration.ATBFormula"/>.
        /// </summary>
        /// <param name="battleUnitData"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static float CalcATBForDelta(this BattleUnitData battleUnitData, float deltaTime) => CalcATBForDelta(battleUnitData, deltaTime, Plugin.Config.ATBFormula.Value);

        /// <summary>
        /// Calculate <paramref name="battleUnitData"/> ATB Delta using <see cref="Time.deltaTime"/> and <see cref="ModConfiguration.ATBFormula"/>.
        /// </summary>
        /// <param name="battleUnitData"></param>
        /// <returns></returns>
        public static float CalcATBForDelta(this BattleUnitData battleUnitData) => CalcATBForDelta(battleUnitData, Time.deltaTime, Plugin.Config.ATBFormula.Value);

        /// <summary>
        /// Calculates the delta time needed for the <paramref name="battleUnitData"/> to get to their next turn.
        /// </summary>
        /// <param name="battleUnitData">Unit to calculate ATB for.</param>
        /// <param name="guageValue">Unit's current ATB Guage value.</param>
        /// <param name="atbFormula"><see cref="ATBFormula"/> to use.</param>
        /// <returns></returns>
        public static float CalcDeltaToNextTurn(this BattleUnitData battleUnitData, float guageValue, ATBFormula atbFormula)
        {
            //Check if unit already has their turn.
            if(guageValue >= BattleProgressATB.MaxATBGauge)
            {
                return 0.0f;
            }

            var remainingGuage = BattleProgressATB.MaxATBGauge - guageValue;

            return atbFormula switch
            {
                ATBFormula.Original => remainingGuage / (BattleProgressATBFF0.UpdateFPSATB * BattlePlugManager.Instance().ATBSpeed * ATBGuageRescalingFactor),
                ATBFormula.OriginalFillRate => remainingGuage / (BattleProgressATBFF0.UpdateFPSATB * BattlePlugManager.Instance().ATBSpeed * battleUnitData.timeMagnification * ATBGuageRescalingFactor),
                ATBFormula.PixelRemaster => remainingGuage / battleUnitData.CalcATBFF0Coefficient(),
                _ => 0.0f
            };
        }

        public static float CalcDeltaToNextTurn(this BattleUnitData battleUnitData, float guageValue) => CalcDeltaToNextTurn(battleUnitData, guageValue, Plugin.Config.ATBFormula.Value);

        public static float CalcDeltaToNextTurn(this KeyValuePair<BattleUnitData, float> pair) => CalcDeltaToNextTurn(pair.Key, pair.Value);

        public static float CalcDeltaToNextTurn(this KeyValuePair<BattleUnitData, float> pair, ATBFormula atbFormula) => CalcDeltaToNextTurn(pair.Key, pair.Value, atbFormula);

        private static int CalcPreeMptivePenalty(BattleUnitData battleUnitData, BattlePopPlug.PreeMptiveState preeMptiveState) => preeMptiveState switch
        {
            BattlePopPlug.PreeMptiveState.BackAttack => battleUnitData.GetOwnedCharacterData() is not null ? BackAttackPenalty : 0,
            BattlePopPlug.PreeMptiveState.PreeMptive => battleUnitData.GetMonster() is not null ? PreeMptivePenalty : 0,
            //This state doesn't occur in FF5 iirc, but we will include it for fun.
            BattlePopPlug.PreeMptiveState.EnemyPreeMptive => battleUnitData.GetOwnedCharacterData() is not null ? PreeMptivePenalty : 0,
            //TODO: add cases for other states which don't occur in FF5?
            _ => 0,
        };
        
        public static bool HasMasamuneEquipped(this BattleUnitData battleUnitData)
        {
            if (battleUnitData.GetOwnedCharacterData() is not OwnedCharacterData ownedCharacterData)
            {
                return false;
            }

            foreach (var equipData in ownedCharacterData.EquipmentData.Values)
            {
                //There is probably a better way to check.
                if (equipData.Weapon is not null && equipData.ItemId == MasamuneId) return true;
            }
            return false;
        }

        public static void AdvanceToNextTurn(this BattleProgressATB battleProgressATB)
        {
            var atbFormula = Plugin.Config.ATBFormula.Value;
            Plugin.Log.LogInfo($"ATBFormula: {atbFormula}");
            var guageStatusDictionary = battleProgressATB.gaugeStatusDictionary.ToManaged();

            var minDeltas = guageStatusDictionary.Select(x => (Unit: x.Key.GetUnitName(), DeltaToNext: x.CalcDeltaToNextTurn(atbFormula)) );
            foreach (var (Unit, DeltaToNext) in minDeltas)
            {
                Plugin.Log.LogInfo($"{Unit}: DeltaToNextTurn: {DeltaToNext}");
            }

            var minDeltaToNext = guageStatusDictionary.Min(x => x.CalcDeltaToNextTurn(atbFormula));
            Plugin.Log.LogInfo($"MinDeltaToNext: {minDeltaToNext}");

            if (minDeltaToNext <= 0.0f)
            { 
                return;
            }

            //Just add the calculated ATB to next turn to everyone's guage
            foreach ((var unitData, var guageValue) in battleProgressATB.gaugeStatusDictionary.ToManaged())
            {
                var guageDelta = unitData.CalcATBForDelta(minDeltaToNext, atbFormula);
                Plugin.Log.LogInfo($"{unitData.GetUnitName()}: Guage: {guageValue} + {guageDelta} = {guageValue + guageDelta}");
                battleProgressATB.ChangeATBGaugeByUnitData(unitData, guageValue + guageDelta);
            }

        }

        /// <summary>
        /// Determines how many "ATB-units" remain until the next turn.
        /// Uses a simple calculation that optionally factors in a unit's timeMagnification.
        /// </summary>
        /// <param name="battleProgressATB"></param>
        /// <param name="applyTimeMagnification"></param>
        /// <returns></returns>
        public static float CalcATBToNextTurn(this BattleProgressATB battleProgressATB, bool applyTimeMagnification)
        {
            var minAdvanceDelta = BattleProgressATB.MaxATBGauge;

            foreach ((var unitData, var guageValue) in battleProgressATB.gaugeStatusDictionary)
            {
                var advanceDelta = BattleProgressATB.MaxATBGauge - guageValue;
                if (applyTimeMagnification)
                {
                    advanceDelta /= unitData.timeMagnification;
                }

                minAdvanceDelta = Math.Min(advanceDelta, minAdvanceDelta);
                if (minAdvanceDelta <= 0f)
                {
                    return 0f;
                }
            }

            return minAdvanceDelta;
        }



        /// <summary>
        /// Fetches the Units name. Should work as long as <paramref name="unitData"/> has a non-null result for
        /// <see cref="BattleUnitData.GetOwnedCharacterData"/> or <see cref="BattleUnitData.GetMonster"/>.
        /// </summary>
        /// <param name="unitData"></param>
        /// <returns></returns>
        public static string GetUnitName(this BattleUnitData unitData)
        {
            //Check if player character
            if (unitData.GetOwnedCharacterData() is OwnedCharacterData ownedCharacterData)
            {
                return ownedCharacterData.Name;
            }
            else if (unitData.GetMonster() is Monster monster)
            {
                return MessageManager.Instance.GetMessage(monster.MesIdName);
            }

            return "<unknown unit>";
        }
    }
}
