using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EquinoxsModUtils;
using HarmonyLib;
using UnityEngine;

namespace ConsoleCommands
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class ConsoleCommandsPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.lunar.ConsoleCommands";
        private const string PluginName = "ConsoleCommands";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        internal static ManualLogSource Log = new ManualLogSource(PluginName);

        // Config Entries

        internal static ConfigEntry<KeyCode> OpenConsoleShortcut;

        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            ConsoleGUI.LoadImages();
            CreateConfigEntries();
            ApplyPatches();

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void Update() {
            if (!ModUtils.hasGameLoaded) return;

            ConsoleGUI.sSinceKeyPress += Time.deltaTime;
            if (UnityInput.Current.GetKeyDown(OpenConsoleShortcut.Value)) {
                ConsoleGUI.OpenConsole();
            }
        }

        private void OnGUI() {
            ConsoleGUI.DrawConsole();
        }

        // Private Functions

        private void CreateConfigEntries() {
            OpenConsoleShortcut = Config.Bind("General", "Open Console Shortcut", KeyCode.Slash, new ConfigDescription("The key to press to open the console"));
        }

        private void ApplyPatches() {

        }
    }
}
