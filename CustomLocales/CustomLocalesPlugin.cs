using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using QFSW.QC;

namespace CustomLocales
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class CustomLocalesPlugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Log = Logger;

            Harmony.CreateAndPatchAll(typeof(LocaleReplacer));
        }

        [Command("CustomLocales-Rip", "Rip PotionCraft localization to JSON files", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        public static void Cmd_RipLocalization()
        {
            LocaleRipper.Rip();
        }
    }
}
