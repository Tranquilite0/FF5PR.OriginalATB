using Last.Battle;
using Last.Data.Master;
using Last.Data.User;
using Last.Management;
using System;
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
            var timeMagnification = Plugin.Config.ATBFormula.Value == ATBFormula.Original ? battleUnitData.timeMagnification : 1f;

            //Plugin.Log.LogInfo($"MinATB Inputs: agi={agi}, weight={weight}, mult={timeMagnification}");

            //The original ATB formula calculates integer values in the range of 1-255 (lower is fuller ATB).
            //We need to invert the range so that higher is fuller and rescale the range to BattleProgressATB.MaxATBGauge.
            //We'll stick to using integer math at first for "authenticity" and convert to float at the end.
            var atbMinInt = Math.Clamp(120 - agi + (weight / 8), ATBMinOriginal, ATBMaxOriginal);
            atbMinInt = Math.Clamp((int)(atbMinInt / timeMagnification), ATBMinOriginal, ATBMaxOriginal);

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

        private static int CalcPreeMptivePenalty(BattleUnitData battleUnitData, BattlePopPlug.PreeMptiveState preeMptiveState) => preeMptiveState switch
        {
            BattlePopPlug.PreeMptiveState.BackAttack => battleUnitData is BattlePlayerData ? BackAttackPenalty : 0,
            BattlePopPlug.PreeMptiveState.PreeMptive => battleUnitData is BattleEnemyData ? PreeMptivePenalty : 0,
            //This state doesn't occur in FF5 iirc, but we will include it for fun.
            BattlePopPlug.PreeMptiveState.EnemyPreeMptive => battleUnitData is BattlePlayerData ? PreeMptivePenalty : 0,
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
            bool applyTimeMag = Plugin.Config.ATBFormula.Value != ATBFormula.Original;
            var atbToNextTurn = battleProgressATB.CalcATBToNextTurn(applyTimeMag);

            //Dont advance if anyone's turn is already up.
            if(atbToNextTurn <= 0)
            {
                return;
            }

            if (Plugin.Config.ATBFormula.Value != ATBFormula.PixelRemaster)
            {
                //Just add the calculated ATB to next turn to everyone's guage
                foreach ((var unitData, var guageValue) in battleProgressATB.gaugeStatusDictionary.ToManaged())
                {
                    battleProgressATB.ChangeATBGaugeByUnitData(unitData, guageValue + atbToNextTurn);
                }
                //Plugin.Log.LogInfo($"AdvanceToNextTurn incremented all ATBs by {atbToNextTurn}.");
            }
            //Since I havent properly reverse engineered the PR CalcATB formula, I have to simulate it with iterative calls to CalcATB until a unit gets their turn.
            //but don't run the iteration if deltaTime is zero or we will enter an infinite loop.
            else if (Time.deltaTime > 0f)
            {
                bool advancingATB = true;
                //Set an upper bound on the number of iteration in case something unexpected happens and ATBs are not being incremented.
                int loopCount = 0;

                for (; advancingATB && loopCount < 1000; loopCount++ )
                {
                    foreach ((var unitData, var guageValue) in battleProgressATB.gaugeStatusDictionary.ToManaged())
                    {
                        var newValue = guageValue + battleProgressATB.ATBCalc(unitData);
                        battleProgressATB.ChangeATBGaugeByUnitData(unitData, newValue);

                        if (newValue >= BattleProgressATB.MaxATBGauge)
                        {
                            advancingATB = false;
                        }
                    }
                }
                Plugin.Log.LogInfo($"AdvanceToNextTurn Completed in {loopCount} iterations.");
            }
            else if (Time.deltaTime <= 0f)
            {
                Plugin.Log.LogInfo($"AdvanceToNextTurn cannot advance because deltaTime={Time.deltaTime}");
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
