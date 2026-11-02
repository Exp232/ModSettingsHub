using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Railroader.Modding;
using TMPro;
using UI.Builder;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ModSettingsHub
{
    /// <summary>
    /// Entry point for the game's own mod loader: Info.json lists this assembly under "assemblies", and the game
    /// creates this class and calls OnModLoad once at start-up. It must stay public with a parameterless constructor.
    /// </summary>
    public sealed class ModEntryPoint : IModEntryPoint
    {
        public void OnModLoad(IModContext context)
        {
            Main.Load(context);
        }

        public void OnModUnload()
        {
            Main.Shutdown();
        }
    }

    public static class Main
    {
        internal const string ModId = "exp232/modsettingshub";

        private static IModLogger _log;
        private static string _dir;
        private static string _version = "?";
        internal static Settings Settings = new Settings();

        /// <summary>True while the mod is running.</summary>
        internal static bool Active;

        private static GameObject _hotkey;
        private static GameObject _button;

        internal static string SettingsPath
        {
            get { return Path.Combine(_dir, "Settings.xml"); }
        }

        // Mods that start before the hub register before it has a logger; keep their lines and write them once it has.
        private static readonly List<KeyValuePair<bool, string>> Early = new List<KeyValuePair<bool, string>>();

        internal static void Log(string message)
        {
            if (_log != null) _log.Info(message);
            else Early.Add(new KeyValuePair<bool, string>(false, message));
        }

        internal static void Warn(string message)
        {
            if (_log != null) _log.Warning(message);
            else Early.Add(new KeyValuePair<bool, string>(true, message));
        }

        internal static void Load(IModContext context)
        {
            _log = context.Log;
            foreach (var line in Early)
            {
                if (line.Key) _log.Warning(line.Value + " (before the hub started)");
                else _log.Info(line.Value + " (before the hub started)");
            }
            Early.Clear();
            _dir = context.ModDirectory;
            _version = ReadVersion(_dir);

            string problem;
            Settings = Settings.Load(SettingsPath, out problem);
            if (problem != null) Warn("Settings: " + problem);

            Active = true;

            try
            {
                _hotkey = new GameObject("ModSettingsHub.Hotkey");
                UnityEngine.Object.DontDestroyOnLoad(_hotkey);
                _hotkey.AddComponent<HubHotkey>();
            }
            catch (Exception e)
            {
                Warn("Hotkey could not be set up: " + e.Message);
            }

            try
            {
                _button = new GameObject("ModSettingsHub.TopRightButton");
                UnityEngine.Object.DontDestroyOnLoad(_button);
                _button.AddComponent<TopRightButton>();
            }
            catch (Exception e)
            {
                Warn("Top-right button could not be set up: " + e.Message);
            }

            // The hub is its own first customer: a page for the hub's few settings.
            Hub.Register(ModId, "Mod Settings Hub", _version, BuildOwnPage);
        }

        internal static void Shutdown()
        {
            Active = false;
            Hub.Shutdown();
            if (_hotkey != null)
            {
                UnityEngine.Object.Destroy(_hotkey);
                _hotkey = null;
            }
            if (_button != null)
            {
                UnityEngine.Object.Destroy(_button);
                _button = null;
            }
        }

        internal static void SaveSettings()
        {
            try
            {
                Settings.Save(SettingsPath);
            }
            catch (Exception e)
            {
                Warn("Could not save settings: " + e.Message);
            }
        }

        private static void BuildOwnPage(UIPanelBuilder page)
        {
            page.AddTitle("Mod Settings Hub", "ModSettingsHub " + _version + " (API " + Hub.ApiVersion + ")");

            page.AddSection("Opening this window", b =>
            {
                HubUi.Toggle(b, "Button in top-right menu", () => Settings.ShowTopRightButton, v => Settings.ShowTopRightButton = v, SaveSettings);
                string key = string.IsNullOrWhiteSpace(Settings.Hotkey) ? "none" : Settings.Hotkey.Replace("<Keyboard>/", "").ToUpperInvariant();
                HubUi.Note(b, "Hotkey: " + (Settings.HotkeyCtrl && key != "none" ? "Ctrl+" : "") + key + ". Change Hotkey and HotkeyCtrl in Settings.xml in the mod folder; it takes effect after a restart.");
            });

            page.AddSection("Mods using the hub", b =>
            {
                foreach (var entry in Hub.All())
                {
                    HubUi.Note(b, entry.Title + (string.IsNullOrEmpty(entry.Version) ? "" : " " + entry.Version));
                }
            });
        }

        private static string ReadVersion(string modDirectory)
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(modDirectory, "Info.json"));
                Match m = Regex.Match(json, "\"version\"\\s*:\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups[1].Value;
            }
            catch (Exception)
            {
            }
            return "?";
        }
    }

    /// <summary>Listens for the hub hotkey. Lives on its own object for as long as the mod is loaded.</summary>
    internal sealed class HubHotkey : MonoBehaviour
    {
        private InputAction _action;

        private void OnEnable()
        {
            try
            {
                string key = Main.Settings.Hotkey;
                if (string.IsNullOrWhiteSpace(key)) return;

                _action = new InputAction("ModSettingsHub.Toggle", InputActionType.Button);
                if (Main.Settings.HotkeyCtrl)
                {
                    _action.AddCompositeBinding("ButtonWithOneModifier")
                        .With("Modifier", "<Keyboard>/ctrl")
                        .With("Button", key);
                }
                else
                {
                    _action.AddBinding(key);
                }
                _action.performed += OnPerformed;
                _action.Enable();
            }
            catch (Exception e)
            {
                Main.Warn("Hotkey not available: " + e.Message);
                _action = null;
            }
        }

        private void OnPerformed(InputAction.CallbackContext context)
        {
            if (!Main.Active) return;
            Hub.Toggle();
        }

        private void OnDisable()
        {
            if (_action == null) return;
            _action.performed -= OnPerformed;
            _action.Disable();
            _action.Dispose();
            _action = null;
        }
    }
}
