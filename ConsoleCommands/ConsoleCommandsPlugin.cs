using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EquinoxsModUtils.Additions;
using EquinoxsModUtils;
using HarmonyLib;
using System;
using UnityEngine;
using ConsoleCommands.Patches;

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

        internal static bool quitOnSaveFinished = false;

        // Config Entries

        internal static ConfigEntry<KeyCode> OpenConsoleShortcut;

        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            CreateConfigEntries();
            ApplyPatches();

            EMUAdditions.CustomData.Add(0, "WarpPoints", "");
            ModUtils.GameDefinesLoaded += OnGameDefinesLoaded;
            ModUtils.SaveStateLoaded += OnSaveStateLoaded;
            ModUtils.GameSaved += OnGameSaved;

            ConsoleGUI.LoadImages();
            CommandManager.LoadDefaultCommands();
            Commands.InitialiseKeyCodeMap();
            Commands.LoadBoundCommands();


            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void Update() {
            if (!ModUtils.hasGameLoaded) return;

            ConsoleGUI.sSinceKeyPress += Time.deltaTime;
            if (UnityInput.Current.GetKeyDown(OpenConsoleShortcut.Value)) {
                ConsoleGUI.OpenConsole();
            }

            if (GlobalData.noClipEnabled) {
                Player.instance.transform.position = Player.instance.camController.camParent.position;
                Player.instance.transform.rotation = Player.instance.camController.camParent.rotation;
            }

            Commands.ExecuteBoundCommands();
        }

        private void OnGUI() {
            ConsoleGUI.DrawConsole();
        }

        // Events

        private void OnGameDefinesLoaded(object sender, EventArgs e) {
            Commands.InitialiseItemsCache();
            Commands.InitialiseUnlocksCache();
        }

        private void OnSaveStateLoaded(object sender, EventArgs e) {
            WarpManager.LoadData();
            CommandSettings.Load();
            CommandSettings.Apply();

            Player.instance.cheats.disableEncumbrance = CommandSettings.weightless;
        }

        private void OnGameSaved(object sender, EventArgs e) {
            //CommandSettings.Save();

            if (quitOnSaveFinished) {
                Application.Quit();
            }
        }

        // Private Functions

        private void CreateConfigEntries() {
            OpenConsoleShortcut = Config.Bind("General", "Open Console Shortcut", KeyCode.Slash, new ConfigDescription("The key to press to open the console"));
        }

        private void ApplyPatches() {
            Harmony.CreateAndPatchAll(typeof(InstaMolePatch));
            Harmony.CreateAndPatchAll(typeof(OpenSesamePatch));
            Harmony.CreateAndPatchAll(typeof(ScannableDataPatch));
        }
    }
}
