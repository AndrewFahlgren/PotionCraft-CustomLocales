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

        public static string localeDir = "UpdatedLocales";

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
                Log.LogDebug("Library is null");
                return;
            }

            var assPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var localePath = Path.Combine(assPath, localeDir);

            if (!Directory.Exists(localePath))
            {
                Log.LogDebug($"ParseLocales: Directory '{localeDir}' does not exist");
                return;
            }

            foreach (LocalizationManager.Locale lang in Enum.GetValues(typeof(LocalizationManager.Locale)))
            {
                string localeFile = Path.Combine(localePath, $"{lang}.json");

                if (!File.Exists(localeFile))
                {
                    Log.LogDebug($"ParseLocales: Skipping '{lang}' - no json file found");
                    continue;
                }

                string json = File.ReadAllText(localeFile);

                if (!string.IsNullOrEmpty(json))
                {
                    if (ParseJsonLocales(json, lang, out int counter))
                    {
                        Log.LogDebug($"ParseLocales: Overwrote {counter} key entries for '{lang}'");
                    }
                    else
                    {
                        Log.LogInfo($"Error encountered trying to parse '{lang}.json'");
                    }
                }
            }
        }

        private static bool ParseJsonLocales(string json, LocalizationManager.Locale lang, out int counter)
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

            foreach ((string key, string text) in data.Select(kvp => (kvp.Key, kvp.Value)))
            {
                if (!library.Contains(key, (int)lang, (int)lang))
                {
                    Log.LogWarning($"Skipping '{key}' as it doesn't exist");
                    continue;
                }
                library.data[key].Add((int)lang, text);
                counter++;
            }

            return true;
        }
    }
}
