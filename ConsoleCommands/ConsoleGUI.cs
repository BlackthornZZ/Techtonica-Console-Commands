using BepInEx;
using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleCommands
{
    internal static class ConsoleGUI
    {
        // Objects & Variables
        private static bool shouldShow;
        private static string userInput = "";
        private static bool initialisedStyles;
        internal static float sSinceKeyPress;

        // Textures

        private static Texture2D consoleLeftCurve;
        private static Texture2D consoleCenter;
        private static Texture2D consoleRightCurve;

        // Styles

        private static GUIStyle consoleLeftStyle;
        private static GUIStyle consoleCenterStyle; 
        private static GUIStyle consoleRightStyle;
        private static GUIStyle textBoxStyle;

        // Internal Functions

        internal static void OpenConsole() {
            if (sSinceKeyPress < 0.2f) return;
            userInput = "";
            shouldShow = true;
            ModUtils.FreeCursor(true);
            sSinceKeyPress = 0;
        }

        internal static void CloseConsole() {
            if (sSinceKeyPress < 0.2f) return;
            shouldShow = false;
            ModUtils.FreeCursor(false);
            sSinceKeyPress = 0;
        }

        internal static void DrawConsole() {
            if (!shouldShow) return;
            if (!initialisedStyles) InitialiseStyles();

            if (Event.current.keyCode == KeyCode.Return) {
                if (!string.IsNullOrEmpty(userInput)) {
                    CommandManager.ParseAndExecute(userInput.ToLower());
                }

                CloseConsole();
            }

            GUI.FocusControl("Console");

            GUI.Box(new Rect(10, Screen.height - 50, 10, 40), "", consoleLeftStyle);
            GUI.Box(new Rect(20, Screen.height - 50, Screen.width - 40, 40), "", consoleCenterStyle);
            GUI.Box(new Rect(Screen.width - 20, Screen.height - 50, 10, 40), "", consoleRightStyle);

            GUI.SetNextControlName("Console");
            userInput = GUI.TextField(new Rect(20, Screen.height - 50, Screen.width - 30, 40), userInput, textBoxStyle);
            if (userInput.EndsWith("/")) {
                CloseConsole();
            }
            
        }

        internal static void ClearConsole() {
            userInput = "";
        }

        internal static void LoadImages() {
            consoleLeftCurve = ModUtils.LoadTexture2DFromFile("ConsoleCommands.Images.Left.png");
            consoleCenter = ModUtils.LoadTexture2DFromFile("ConsoleCommands.Images.Center.png");
            consoleRightCurve = ModUtils.LoadTexture2DFromFile("ConsoleCommands.Images.Right.png");
        }

        // Private Functions

        private static void InitialiseStyles() {
            consoleLeftStyle = new GUIStyle() { normal = { background = consoleLeftCurve } };
            consoleCenterStyle = new GUIStyle() { normal = { background = consoleCenter } };
            consoleRightStyle = new GUIStyle() { normal = { background = consoleRightCurve } };
            textBoxStyle = new GUIStyle() { 
                fontSize = 18,
                alignment = TextAnchor.MiddleLeft,
                font = Font.CreateDynamicFontFromOSFont("Roboto", 18),
                normal = { 
                    textColor = Color.white,
                    background = null 
                } 
            };
        }
    }
}
