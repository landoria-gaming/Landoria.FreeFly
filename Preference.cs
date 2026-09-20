using BepInEx.Configuration;
using Landoria.Shared;
using UnityEngine;

namespace Landoria.FreeFly
{
    // Reads the player's free-camera settings.
    internal static class Preference
    {
        private static ConfigEntry<KeyboardShortcut> toggleShortcut;

        internal static KeyboardShortcut ToggleShortcut => toggleShortcut.Value;

        // Creates the saved shortcut setting.
        internal static void Initialize(ConfigFile config)
        {
            toggleShortcut = config.Bind(
                "Controls", "ToggleShortcut",
                new KeyboardShortcut(KeyCode.F7),
                "Shortcut used to enable or disable free fly.\n" +
                "\nExamples: Mouse2 for the middle mouse button.\n" +
                "\nMouse3/Mouse4 for the Forward/Back side button.\n" +
                "\nSpace + LeftControl for Left Ctrl + Space.\n" +
                "\nhttps://docs.unity3d.com/ScriptReference/KeyCode.html");
        }

        // Restores the shortcut and recreates a deleted configuration file.
        internal static void RestoreDefaults(ConfigFile config)
        {
            bool saveOnConfigSet = config.SaveOnConfigSet;
            config.SaveOnConfigSet = false;
            try
            {
                toggleShortcut.Value = new KeyboardShortcut(KeyCode.F7);
            }
            finally
            {
                config.SaveOnConfigSet = saveOnConfigSet;
            }

            config.Save();
            ConfigWatcher.IgnoreCurrentFileVersion();
        }
    }
}
