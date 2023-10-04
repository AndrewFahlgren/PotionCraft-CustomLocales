using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using PotionCraft.Assemblies.DataBaseSystem.PreparedObjects;
using PotionCraft.LocalizationSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CustomLocales
{
    internal static class LocaleReplacer
    {
        static ManualLogSource Log => CustomLocalesPlugin.Log;

        public static readonly LocalizationManager.Locale DefaultLocale = LocalizationManager.Locale.en;
        public static LocalizationData library;

        public static string localeDir = "Custom";

        [HarmonyPostfix, HarmonyPatch(typeof(LocalizationManager), "LoadLocalizationData")]
        public static void LoadLocalizationData_Postfix(ref LocalizationData __result)
        {
            library = __result;
            ParseLocalizationData();
        }

        public static void ParseLocalizationData()
        {
            if (library == null)
            {
                Log.LogWarning("Library is null");
                return;
            }

            var assPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var localePath = Path.Combine(assPath, localeDir);

            if (!Directory.Exists(localePath))
            {
                Log.LogInfo($"ParseLocales: Directory '{localeDir}' does not exist");
                return;
            }

            foreach (string localeFile in Directory.GetFiles(localePath))
            {
                string json = File.ReadAllText(localeFile);
                string lang = Path.GetFileNameWithoutExtension(localeFile);

                if (!string.IsNullOrEmpty(json))
                {
                    if (ParseJsonLocales(json, out int counter))
                    {
                        Log.LogInfo($"ParseLocales: Overwrote {counter} key entries for '{lang}'");
                        break;
                    }
                    else
                    {
                        Log.LogWarning($"Error encountered trying to parse '{lang}.json'. Looking for more files to parse.");
                    }
                }
            }
        }

        private static bool ParseJsonLocales(string json, out int counter)
        {
            counter = 0;
            Dictionary<string, string> data;
            
            try
            {
                data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception e)
            {
                Log.LogError(e);
                return false;
            }
            var libraryData = (SerializedDictionaryStringLocalizationTextData)Traverse.Create(library).Field("data").GetValue();
            var currentLocale = (int)LocalizationManager.CurrentLocale;

            foreach ((string key, string text) in data.Select(kvp => (kvp.Key, kvp.Value)))
            {
                if (!libraryData.ContainsKey(key))
                {
                    Log.LogWarning($"Skipping '{key}' as it doesn't exist");
                    continue;
                }
                libraryData[key].Add(currentLocale, text);
                counter++;
            }

            return true;
        }
    }
}
