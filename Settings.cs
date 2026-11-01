using System;
using System.IO;
using System.Xml.Serialization;

namespace ModSettingsHub
{
    /// <summary>Kept in Settings.xml in the mod folder.</summary>
    public class Settings
    {
        /// <summary>Opens the hub window: an Input System binding path (for example &lt;Keyboard&gt;/f7), empty for none.</summary>
        public string Hotkey = "<Keyboard>/f7";

        /// <summary>Hold Ctrl with Hotkey (the default, so the key cannot clash with the game's own).</summary>
        public bool HotkeyCtrl = true;

        /// <summary>Adds a button to the game's top-right menu that opens the hub window.</summary>
        public bool ShowTopRightButton = true;

        /// <summary>Writes the file through a temporary copy, so a failed write cannot leave half a file.</summary>
        internal void Save(string path)
        {
            string temp = path + ".tmp";
            using (var stream = File.Create(temp)) new XmlSerializer(typeof(Settings)).Serialize(stream, this);
            if (File.Exists(path)) File.Delete(path);
            File.Move(temp, path);
        }

        /// <summary>Reads the file, or returns defaults (and writes them out when there is no file yet).</summary>
        internal static Settings Load(string path, out string problem)
        {
            problem = null;
            var serializer = new XmlSerializer(typeof(Settings));
            if (File.Exists(path))
            {
                try
                {
                    using (var stream = File.OpenRead(path)) return (Settings)serializer.Deserialize(stream);
                }
                catch (Exception e)
                {
                    // Leave the user's file alone and run on defaults.
                    problem = "could not read " + path + ", using defaults: " + e.Message;
                    return new Settings();
                }
            }
            var settings = new Settings();
            try
            {
                using (var stream = File.Create(path)) serializer.Serialize(stream, settings);
            }
            catch (Exception e)
            {
                problem = "could not write " + path + ": " + e.Message;
            }
            return settings;
        }
    }
}
