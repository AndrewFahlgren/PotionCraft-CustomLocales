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
using Locale = PotionCraft.LocalizationSystem.LocalizationManager.Locale;

namespace CustomLocales
{
    internal static class LocaleRipper
    {
        public static ManualLogSource Log => CustomLocalesPlugin.Log;

        public static void Rip()
        {
            Log.LogDebug("Creating locale repos");
            Dictionary<Locale, Dictionary<string, string>> inverted = new();
            foreach (Locale l in Enum.GetValues(typeof(Locale)))
            {
                inverted[l] = new();
            }

            Log.LogDebug("Ripping localization");
            var localizationData = (LocalizationData)Traverse.Create(typeof(LocalizationManager)).Field("localizationData").GetValue();
            var data = (SerializedDictionaryStringLocalizationTextData)Traverse.Create(localizationData).Field("data").GetValue();
            foreach ((string key, LocalizationTextData value) in data.Select(kvp => (kvp.Key, kvp.Value)))
            {
                foreach (Locale l in Enum.GetValues(typeof(Locale)))
                {
                    inverted[l].Add(key, value.Contains((int)l) ? value.Get((int)l) : "");
                }
            }

            string assPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dumpDir = Path.Combine(assPath, "Ripped");
            if (!Directory.Exists(dumpDir))
            {
                Directory.CreateDirectory(dumpDir);
            }

            Log.LogDebug("Dumping locale repos to file");
            foreach (Locale l in Enum.GetValues(typeof(Locale)))
            {
                string json = JsonConvert.SerializeObject(inverted[l], Formatting.Indented);

                string localePath = Path.Combine(dumpDir, $"{l}.json");
                using var outStream = new StreamWriter(localePath, false);
                outStream.WriteLine(json);
            }
        }
    }
}
