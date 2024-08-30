using BepInEx;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using GameAnalyticsSDK;
using Rewired.UI.ControlMapper;
using System;
using System.CodeDom;
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
        private static string autoComplete = "";

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
        private static GUIStyle autoCompleteLabelStyle;
        private static GUIStyle helpTitleStyle;
        private static GUIStyle helpTextStyle;

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

        internal static void ClearConsole() {
            userInput = "";
        }

        internal static void LoadImages() {
            consoleLeftCurve = ModUtils.LoadTexture2DFromFile("ConsoleCommands.Images.Left.png");
            consoleCenter = ModUtils.LoadTexture2DFromFile("ConsoleCommands.Images.Center.png");
            consoleRightCurve = ModUtils.LoadTexture2DFromFile("ConsoleCommands.Images.Right.png");
        }

        // Draw Functions

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

            if(userInput == "" && autoComplete != "") {
                userInput = autoComplete;
                autoComplete = "";
            }

            GUI.SetNextControlName("Console");
            userInput = GUI.TextField(new Rect(20, Screen.height - 50, Screen.width - 30, 40), userInput, textBoxStyle);
            if (userInput.EndsWith("/")) {
                CloseConsole();
                return;
            }

            DrawAutoCompleteLabel();

            if(userInput.Contains(" ") && CommandManager.TryGetCommand(userInput.Split(' ').First(), out Command command)) {
                DrawHelpPanelBackground(command);
                DrawHelpPanelContent(command);
            }
        }

        internal static void DrawAutoCompleteLabel() {
            if (userInput.Length == 0) return;

            List<string> results = CommandManager.GetNames().Where(name => name.StartsWith(userInput.ToLower())).ToList();
            if (results.Count == 0) return;

            results = results.OrderBy(name => name.Length).ToList();
            string topResult = results.First();
            topResult = topResult.Replace(userInput.ToLower(), "");

            autoCompleteLabelStyle.CalcMinMaxWidth(new GUIContent(userInput.ToLower()), out float minWidth, out float maxWidth);

            GUI.SetNextControlName("AutoCompleteLabel");
            GUI.Label(new Rect(20 + minWidth, Screen.height - 50, Screen.width - 30 - minWidth, 40), topResult, autoCompleteLabelStyle);

            if (Event.current.keyCode == KeyCode.Tab) {
                autoComplete = userInput + topResult;
                userInput = "";
            }

            if (ShouldShowHelpPanel(topResult, out Command command)) 
            {
                DrawHelpPanelBackground(command);
                DrawHelpPanelContent(command);
            }
        }

        internal static void DrawHelpPanelBackground(Command command) {
            CalculateContentSize(command, out float height, out float yPos);
            float width = Screen.width - 20;

            Images.HelpBox.topLeft.Draw(10, yPos);
            Images.HelpBox.topRight.Draw(10 + width - Images.HelpBox.topRight.width, yPos);
            Images.HelpBox.bottomRight.Draw(10 + width - Images.HelpBox.topRight.width, yPos + height - Images.HelpBox.bottomRight.height);
            Images.HelpBox.bottomLeft.Draw(10, yPos + height - Images.HelpBox.bottomLeft.height);

            Images.HelpBox.top.Draw(10 + Images.HelpBox.topLeft.width, yPos, width - 2 * Images.HelpBox.topLeft.width, Images.HelpBox.top.height);
            Images.HelpBox.bottom.Draw(10 + Images.HelpBox.topLeft.width, yPos + height - Images.HelpBox.bottom.height, width - 2 * Images.HelpBox.topLeft.width, Images.HelpBox.bottom.height);
            Images.HelpBox.left.Draw(10, yPos + Images.HelpBox.topLeft.height, Images.HelpBox.left.width, height - 2 * Images.HelpBox.topLeft.height);
            Images.HelpBox.right.Draw(Screen.width - 10 - Images.HelpBox.right.width, yPos + Images.HelpBox.topRight.height, Images.HelpBox.right.width, height - 2 * Images.HelpBox.topRight.height);
            Images.HelpBox.center.Draw(10 + Images.HelpBox.topLeft.width, yPos + Images.HelpBox.topLeft.height, width - 2 * Images.HelpBox.topLeft.width, height - 2 * Images.HelpBox.topLeft.height);
        }

        internal static void DrawHelpPanelContent(Command command) {
            CalculateContentSize(command, out float height, out float yPos);
            GUI.Label(new Rect(20, yPos + 10, Screen.width, 40), command.name, helpTitleStyle);

            Images.HelpBox.orangePixel.Draw(20, yPos + 50, Screen.width - 40, 2);

            string content = GetHelpContentForCommand(command);
            GUI.Label(new Rect(20, yPos + 70, Screen.width, height), content, helpTextStyle);
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
            
            autoCompleteLabelStyle = new GUIStyle() {
                fontSize = 18,
                alignment = TextAnchor.MiddleLeft,
                font = Font.CreateDynamicFontFromOSFont("Roboto", 18),
                normal = {
                    textColor = Color.gray,
                    background = null
                }
            };

            helpTitleStyle = new GUIStyle() {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                font = Font.CreateDynamicFontFromOSFont("Roboto", 20),
                alignment = TextAnchor.MiddleLeft,
                normal = {
                    textColor = Color.yellow,
                    background = null
                }
            };

            helpTextStyle = new GUIStyle() {
                fontSize = 16,
                font = Font.CreateDynamicFontFromOSFont("Roboto", 16),
                alignment = TextAnchor.UpperLeft,
                normal = {
                    textColor = Color.white,
                    background = null
                }
            };
        }

        private static bool ShouldShowHelpPanel(string topResult, out Command command) {
            return CommandManager.TryGetCommand(userInput, out command) ||
                   CommandManager.TryGetCommand(userInput + topResult, out command);
        }

        private static void CalculateContentSize(Command command, out float height, out float yPos) {
            string content = GetHelpContentForCommand(command);
            height = helpTextStyle.CalcSize(new GUIContent(content)).y;
            height += 70;
            yPos = Screen.height - 60 - height;
        }

        private static string GetHelpContentForCommand(Command command) {
            string content = $"{command.description}\n\nArguments:\n\n";
            foreach (Argument argument in command.arguments) {
                content += $"• {argument.name}\n  • Description: {argument.description}\n  • Type: {GetPrettyType(argument.type)}\n  • Optional: {argument.optional}\n\n";
            }

            if (command.examples.Count != 0) {
                if (!content.EndsWith("\n")) content += "\n\n";
                content += "Examples:\n\n";
                foreach (string example in command.examples) {
                    content += $"• {example}\n";
                }
            }

            return content;
        }

        private static string GetPrettyType(Type type) {
            if (type == typeof(string)) return "String";
            if (type == typeof(uint)) return "Positive Integer";
            if (type == typeof(int)) return "Interger";
            if (type == typeof(float)) return "Decimal Number";
            if (type == typeof(bool)) return "Bool";
            if (type == typeof(KeyCode)) return "Key Code";

            return type.ToString();
        }
    }
}
