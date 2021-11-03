using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using UnityEngine;
using DiskCardGame;
using HarmonyLib;

namespace InscryptionConstantDeckMod
{

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInIncompatibility("porta.inscryption.traderstart")]
    public class Plugin : BaseUnityPlugin
    {



        public List<string> getConfigs()
        {
            List<string> list = new List<string>(4);
            list.Add(Config.Bind("Prebuilt Deck", "Card1", "Wolf").Value);
            list.Add(Config.Bind("Prebuilt Deck", "Card2", "Wolf").Value);
            list.Add(Config.Bind("Prebuilt Deck", "Card3", "Wolf").Value);
            list.Add(Config.Bind("Prebuilt Deck", "Card4", "Wolf").Value);
            return list;
        }
        private void Awake()
        {

            Logger.LogInfo("Loading Prebuilt Deck mod");
            Plugin.Log = base.Logger;
            Log.LogInfo("Collecting Config Info");
            var configs = getConfigs();
            Log.LogInfo("Loading Cards");
            var AllData = ScriptableObjectLoader<CardInfo>.AllData;
            if (Config.Bind("Prebuilt Deck", "PrintCards", false).Value)
            {
                foreach (var item in AllData)
                {
                    Log.LogInfo(item.name);
                }
            }
            Log.LogInfo("Parsing Config with loaded cards.");
            bool passed = true;
            foreach (var item in configs)
            {
                if (AllData.Find((CardInfo x) => x.name == item))
                {
                    Log.LogInfo($"Found {item} from config in Card list");
                } else
                {
                    Log.LogError($"Failed to find {item} in Card list.");
                    passed = false;
                }
            }
            if (passed)
            {
                Log.LogInfo("Passed all cards in config, Patching.");
                Harmony harmony = new Harmony(PluginGuid);
                harmony.PatchAll();
            }

        }

        [HarmonyPatch(typeof(DeckInfo), "InitializeAsPlayerDeck")]
        public class DeckInfo_InitializeAsPlayerDeck : DeckInfo
        {
            [HarmonyPrefix]
            public static bool Prefix(ref DeckInfo __instance)
            {
                Plugin p = new Plugin();
                var Configs = p.getConfigs();
                __instance.AddCard(CardLoader.GetCardByName(Configs[0]));
                __instance.AddCard(CardLoader.GetCardByName(Configs[1]));
                __instance.AddCard(CardLoader.GetCardByName(Configs[2]));
                __instance.AddCard(CardLoader.GetCardByName(Configs[3]));
                return false;
            }
        }


        internal static ManualLogSource Log;
        private const string PluginGuid = "porta.inscryption.constantdeck";
        private const string PluginName = "Constant Deck Mod";
        private const string PluginVersion = "1.0.0";

        
    }
    
}
