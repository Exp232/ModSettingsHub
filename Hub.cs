using System;
using System.Collections.Generic;
using System.Linq;
using UI.Builder;
using UI.Windows;
using UnityEngine;

namespace ModSettingsHub
{
    internal sealed class HubEntry
    {
        public string Id;
        public string Title;
        public string Version;
        public Action<UIPanelBuilder> Build;
    }

    /// <summary>
    /// The public API of Mod Settings Hub (API version 1). Other mods call it by reflection (see HubLinkTemplate.cs),
    /// so they need no reference to this assembly and still work when it is not installed. Keep these signatures
    /// stable: add overloads, never change or remove one.
    /// </summary>
    public static class Hub
    {
        /// <summary>Bumped only for a change that breaks existing callers.</summary>
        public const int ApiVersion = 1;

        internal const string WindowId = "exp232.modsettingshub.window";

        private static readonly Dictionary<string, HubEntry> Entries = new Dictionary<string, HubEntry>(StringComparer.OrdinalIgnoreCase);
        private static readonly UIState<string> Selected = new UIState<string>(null);

        /// <summary>
        /// Adds (or replaces) a mod's page in the hub window.
        /// <paramref name="build"/> is called with a builder that sits inside the hub's own scroll view: add titles,
        /// sections, sliders and so on, but do not add a scroll view of your own. It runs again whenever the page is
        /// shown or rebuilt, so read live values inside the controls' getters.
        /// Works before or after the hub has started, and without a running game scene.
        /// </summary>
        /// <param name="id">The mod's identifier (the one in its Info.json).</param>
        /// <param name="title">Name shown in the list.</param>
        /// <param name="version">Shown next to the name; may be null.</param>
        public static void Register(string id, string title, string version, Action<UIPanelBuilder> build)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id must not be empty", "id");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("title must not be empty", "title");
            if (build == null) throw new ArgumentNullException("build");

            Entries[id] = new HubEntry { Id = id, Title = title, Version = version, Build = build };
            Main.Log("Registered '" + title + "' (" + id + ")");
            Refresh();
        }

        /// <summary>Removes a mod's page. Call it when the mod unloads.</summary>
        public static void Unregister(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (Entries.Remove(id))
            {
                Main.Log("Unregistered " + id);
                if (string.Equals(Selected.Value, id, StringComparison.OrdinalIgnoreCase)) Selected.Value = null;
                Refresh();
            }
        }

        public static bool IsRegistered(string id)
        {
            return !string.IsNullOrEmpty(id) && Entries.ContainsKey(id);
        }

        // Opens the hub, on the given mod's page when it is registered (the page last shown otherwise).
        private static bool Open(string id)
        {
            try
            {
                var windows = WindowManager.Shared;
                if (windows == null) return false;   // main menu: the window system is not up yet
                if (IsRegistered(id)) Selected.Value = Entries[id].Id;
                EnsureRegistered(windows);
                if (windows.IsWindowOpen(WindowId))
                {
                    var open = windows.GetCustomWindow(WindowId);
                    if (open != null) open.Rebuild();
                }
                else
                {
                    windows.ShowWindow(WindowId);
                }
                return true;
            }
            catch (Exception e)
            {
                Main.Warn("Open failed: " + e.Message);
                return false;
            }
        }

        /// <summary>Opens or closes the hub window (the hotkey and the top-right button).</summary>
        public static void Toggle()
        {
            try
            {
                var windows = WindowManager.Shared;
                if (windows == null) return;
                if (windows.IsWindowOpen(WindowId))
                {
                    windows.CloseWindow(WindowId);
                    return;
                }
                Open(null);
            }
            catch (Exception e)
            {
                Main.Warn("Toggle failed: " + e.Message);
            }
        }

        /// <summary>Rebuilds the hub window if it is open (after the list of pages changed).</summary>
        public static void Refresh()
        {
            try
            {
                var windows = WindowManager.Shared;
                if (windows == null) return;
                var window = windows.GetCustomWindow(WindowId);
                if (window != null) window.Rebuild();
            }
            catch (Exception e)
            {
                Main.Warn("Refresh failed: " + e.Message);
            }
        }

        internal static int Count
        {
            get { return Entries.Count; }
        }

        internal static IEnumerable<HubEntry> All()
        {
            return Entries.Values.OrderBy(e => e.Title, StringComparer.OrdinalIgnoreCase).ToList();
        }

        internal static void Shutdown()
        {
            try
            {
                var windows = WindowManager.Shared;
                if (windows != null && windows.IsWindowOpen(WindowId)) windows.CloseWindow(WindowId);
            }
            catch (Exception)
            {
            }
            Entries.Clear();
            Selected.Value = null;
        }

        // The game forgets custom windows when a map unloads, so register again whenever the definition is gone.
        private static void EnsureRegistered(WindowManager windows)
        {
            if (windows.HasDefinition(WindowId)) return;
            windows.RegisterCustomWindow(new CustomWindowDefinition
            {
                Identifier = WindowId,
                Title = "Mod Settings",
                Config = WindowConfig.Resizable(new Vector2Int(760, 560), new Vector2Int(520, 320)),
                BuilderAction = BuildWindow,
            });
        }

        private static void BuildWindow(UIPanelBuilder builder)
        {
            try
            {
                var items = new List<UIPanelBuilder.ListItem<HubEntry>>();
                foreach (var entry in All())
                {
                    items.Add(new UIPanelBuilder.ListItem<HubEntry>(entry.Id, entry, "Mods", entry.Title));
                }

                if (items.Count == 0)
                {
                    builder.AddLabel("No mod has added its settings here yet.");
                    return;
                }

                if (string.IsNullOrEmpty(Selected.Value) || !Entries.ContainsKey(Selected.Value))
                {
                    Selected.Value = items[0].Identifier;
                }

                builder.AddListDetail<HubEntry>(items, Selected, BuildDetail, 200f);
            }
            catch (Exception e)
            {
                Main.Warn("Window could not be built: " + e);
            }
        }

        private static void BuildDetail(UIPanelBuilder detail, HubEntry entry)
        {
            if (entry == null)
            {
                detail.AddLabel("Select a mod from the list.");
                return;
            }

            detail.VScrollView(page =>
            {
                try
                {
                    entry.Build(page);
                }
                catch (Exception e)
                {
                    Main.Warn("The page of '" + entry.Title + "' failed: " + e);
                    page.AddTitle(entry.Title, entry.Version ?? "");
                    page.AddLabel("This mod's settings page could not be built: " + e.Message);
                }
            });
        }
    }

    /// <summary>
    /// Small builders for mod authors who reference this assembly directly. Mods that link by reflection keep their
    /// own copies (they are the same few lines).
    /// </summary>
    public static class HubUi
    {
        /// <summary>Explanatory text: smaller and dimmer than the controls, so the sliders stay in view.</summary>
        public static void Note(UIPanelBuilder b, string text)
        {
            b.AddLabel(text, t =>
            {
                t.fontSize *= 0.75f;
                t.alpha = 0.8f;
            });
        }

        /// <summary>A labelled slider that snaps to <paramref name="step"/>. <paramref name="onChanged"/> runs when the drag ends (save here).</summary>
        public static void Slider(UIPanelBuilder b, string label, Func<float> get, Action<float> set, float min, float max, float step, Func<float, string> format, Action onChanged = null)
        {
            b.AddField(label, b.AddSliderQuantized(
                get,
                () => format(get()),
                v => set(Mathf.Clamp(Mathf.Round(v / step) * step, min, max)),
                step, min, max,
                v => { if (onChanged != null) onChanged(); }));
        }

        /// <summary>A labelled on/off switch. <paramref name="onChanged"/> runs after each change (save here).</summary>
        public static void Toggle(UIPanelBuilder b, string label, Func<bool> get, Action<bool> set, Action onChanged = null)
        {
            b.AddFieldToggle(label, get, v =>
            {
                set(v);
                if (onChanged != null) onChanged();
            });
        }
    }
}
