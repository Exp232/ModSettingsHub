using System;
using System.Reflection;
using TMPro;
using UI;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace ModSettingsHub
{
    /// <summary>
    /// Adds a button for the hub to the game's top-right menu (the clock, company, timetable... strip). The strip is
    /// part of the game's scene, so the button is made by copying one of its existing buttons next to it, and made
    /// again whenever the strip is rebuilt (a new map). If the game's layout is not what this expects, nothing is
    /// added and the hotkey still works.
    /// </summary>
    internal sealed class TopRightButton : MonoBehaviour
    {
        private const string ButtonName = "ModSettingsHub.Button";

        private float _nextCheck;
        private GameObject _button;
        private bool _loggedProblem;
        private static Sprite _icon;

        private void Update()
        {
            if (Time.unscaledTime < _nextCheck) return;
            _nextCheck = Time.unscaledTime + 1f;
            try
            {
                Ensure();
            }
            catch (Exception e)
            {
                if (!_loggedProblem)
                {
                    _loggedProblem = true;
                    Main.Warn("Top-right button could not be added (the hotkey still works): " + e);
                }
            }
        }

        private void OnDestroy()
        {
            if (_button != null) Destroy(_button);
            _button = null;
        }

        private void Ensure()
        {
            if (!Main.Settings.ShowTopRightButton)
            {
                if (_button != null)
                {
                    Destroy(_button);
                    _button = null;
                }
                return;
            }

            if (_button != null) return;   // still there (it goes with the strip when the scene changes)

            var areas = FindObjectsByType<TopRightArea>(FindObjectsSortMode.None);
            if (areas.Length == 0) return;
            var area = areas[0];

            // The strip keeps its buttons in serialized private fields; either one is a plain button in the right row.
            Button source = ButtonField(area, "timetableButton");
            if (source == null) source = ButtonField(area, "tutorialButton");
            if (source == null)
            {
                if (!_loggedProblem)
                {
                    _loggedProblem = true;
                    Main.Warn("The top-right menu has no button to copy (the game changed?); only the hotkey opens the hub.");
                }
                return;
            }

            var clone = Instantiate(source.gameObject, source.transform.parent, false);
            clone.name = ButtonName;
            clone.SetActive(true);
            clone.transform.SetSiblingIndex(source.transform.GetSiblingIndex() + 1);

            var button = clone.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();   // drops the copied button's own (saved) click handlers
            button.onClick.AddListener(Hub.Toggle);
            button.interactable = true;

            // Same look as its neighbours, with its own icon (or, if the button shows text, its own text).
            foreach (var text in clone.GetComponentsInChildren<TMP_Text>(true))
            {
                text.text = "Mods";
            }
            var face = button.targetGraphic as Image;
            Image iconImage = null;
            foreach (var image in clone.GetComponentsInChildren<Image>(true))
            {
                if (image == face || image.sprite == null) continue;
                iconImage = image;
                break;
            }
            if (iconImage == null) iconImage = face;
            if (iconImage != null)
            {
                iconImage.sprite = Icon();
                iconImage.type = Image.Type.Simple;
                iconImage.preserveAspect = true;
            }

            var tip = clone.GetComponentInChildren<UITooltipProvider>(true);
            if (tip != null)
            {
                tip.DynamicTooltipInfo = null;
                tip.TooltipInfo = new TooltipInfo("Mod Settings", "Settings of installed mods, in one window.");
            }

            _button = clone;
            Main.Log("Added the button to the top-right menu (copied from " + source.name + ")");
        }

        private static Button ButtonField(object owner, string field)
        {
            var info = owner.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (info == null) return null;
            return info.GetValue(owner) as Button;
        }

        /// <summary>Three slider tracks with a knob each, drawn in white so the button's own tint applies.</summary>
        private static Sprite Icon()
        {
            if (_icon != null) return _icon;

            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            float[] trackY = { 46f, 32f, 18f };
            float[] knobX = { 22f, 42f, 30f };
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float coverage = 0f;
                    for (int i = 0; i < 3; i++)
                    {
                        // Track: a 4 px line from x=8 to x=56.
                        float cx = Mathf.Clamp(x + 0.5f, 8f, 56f);
                        float dx = x + 0.5f - cx;
                        float dy = y + 0.5f - trackY[i];
                        float lineDist = Mathf.Sqrt(dx * dx + dy * dy);
                        coverage = Mathf.Max(coverage, Mathf.Clamp01(2.5f - lineDist));

                        // Knob: a disc of radius 6.
                        float kx = x + 0.5f - knobX[i];
                        float knobDist = Mathf.Sqrt(kx * kx + dy * dy);
                        coverage = Mathf.Max(coverage, Mathf.Clamp01(6.5f - knobDist));
                    }
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(coverage * 255f));
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            _icon = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return _icon;
        }
    }
}
