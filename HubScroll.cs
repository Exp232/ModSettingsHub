using System;
using System.Collections.Generic;
using System.Reflection;
using UI.Builder;

namespace ModSettingsHub
{
    /// <summary>
    /// Remembers how far each mod's page is scrolled, so a rebuild (a toggle, a slider, a button) does not throw the
    /// player back to the top. Railroader 0.6.3 added VScrollView(closure, UIScrollViewState) for this. The hub still
    /// runs on 0.6.0 to 0.6.2, where that overload and type do not exist, so everything here goes through reflection and
    /// falls back to the plain VScrollView when it is missing.
    /// </summary>
    internal static class HubScroll
    {
        private static bool _looked;
        private static Type _stateType;
        private static MethodInfo _scroll;
        private static readonly Dictionary<string, object> States = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Puts <paramref name="closure"/> in a scroll view that remembers its position under <paramref name="key"/>. False when the game has no such overload; the caller then uses the plain VScrollView.</summary>
        internal static bool TryScroll(UIPanelBuilder builder, string key, Action<UIPanelBuilder> closure)
        {
            Look();
            if (_scroll == null || string.IsNullOrEmpty(key)) return false;
            try
            {
                object state;
                if (!States.TryGetValue(key, out state))
                {
                    state = Activator.CreateInstance(_stateType);
                    States[key] = state;
                }
                _scroll.Invoke(builder, new object[] { closure, state });
                return true;
            }
            catch (TargetInvocationException e)
            {
                // The scroll view was made and the page started to draw; drawing it again would double it.
                Main.Warn("Scroll position could not be kept: " + (e.InnerException ?? e).Message);
                return true;
            }
            catch (Exception e)
            {
                Main.Warn("Scroll position is not kept: " + e.Message);
                _scroll = null;
                return false;
            }
        }

        /// <summary>Forgets a mod's scroll position (when it unregisters).</summary>
        internal static void Forget(string key)
        {
            if (!string.IsNullOrEmpty(key)) States.Remove(key);
        }

        internal static void Clear()
        {
            States.Clear();
        }

        private static void Look()
        {
            if (_looked) return;
            _looked = true;
            try
            {
                _stateType = typeof(UIPanelBuilder).Assembly.GetType("UI.Builder.UIScrollViewState", false);
                if (_stateType == null) return;   // Railroader 0.6.2 or older
                _scroll = typeof(UIPanelBuilder).GetMethod("VScrollView", BindingFlags.Public | BindingFlags.Instance, null,
                    new[] { typeof(Action<UIPanelBuilder>), _stateType }, null);
            }
            catch (Exception e)
            {
                Main.Warn("Scroll position is not kept: " + e.Message);
                _scroll = null;
            }
        }
    }
}
