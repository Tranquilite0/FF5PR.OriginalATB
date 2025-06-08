using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using FF5PR.OriginalATB.Patches;
using HarmonyLib;
using System;

namespace FF5PR.OriginalATB
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public partial class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;
        internal static new ModConfiguration Config;

        public override void Load()
        {
            Log = base.Log;

            Log.LogInfo($"Game detected: {GameDetection.Version}");
            Log.LogInfo("Loading...");

            Config = new ModConfiguration(base.Config);
            Config.Init();
            if (ModComponent.Inject())
            {
                ApplyPatches();
            }

            // Plugin startup logic
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private static void ApplyPatches()
        {
            if (Config.DelayAtTurnStart.Value)
            {
                ApplyPatch(typeof(BattleATBDelayPatches), GameVersion.FF4 | GameVersion.FF5 | GameVersion.FF6);
            }

            //TODO: does this even work in FF4? If it does, it would probably have weird balance issues since agility might not have the same ranges as V.
            ApplyPatch(typeof(ATBFormulaPatches), GameVersion.FF5 | GameVersion.FF4);
            //ApplyPatch(typeof(TestPatches), GameVersion.FF5 | GameVersion.FF4);

            Log.LogInfo("Patches applied!");
        }

        private static void ApplyPatch(Type type, GameVersion versionsFlag = GameVersion.Any)
        {
            if ((GameDetection.Version & versionsFlag) != GameDetection.Version)
            {
                return;
            }

            Log.LogInfo($"Patching {type.Name}...");

            Harmony.CreateAndPatchAll(type);
        }
    }
}
