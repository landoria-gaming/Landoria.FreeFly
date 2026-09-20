using BepInEx.Configuration;
using UnityEngine;

namespace Landoria.FreeFly
{
    // Toggles the free camera with the configured shortcut.
    internal static class Shortcut
    {
        private static bool suppressEscapeMenu;

        // Handles the toggle and Escape shortcuts.
        internal static void Update()
        {
            if (GameCamera.InFreeFly() && ZInput.GetKeyDown(KeyCode.Escape))
            {
                Controller.Disable();
                suppressEscapeMenu = true;
                if (Menu.IsActive())
                {
                    Menu.instance.Hide();
                }
                return;
            }

            if (!CanToggle() || !IsToggleShortcutDown())
            {
                return;
            }

            Controller.Toggle();
        }

        // Closes the menu opened by a consumed Escape key.
        internal static void CloseSuppressedMenu()
        {
            if (!suppressEscapeMenu)
            {
                return;
            }

            if (Menu.instance)
            {
                Menu.instance.Hide();
            }
            suppressEscapeMenu = false;
        }

        // Checks whether shortcuts are safe to handle.
        private static bool CanToggle()
        {
            Player player = Player.m_localPlayer;
            return player && !player.IsDead() && !player.InCutscene() &&
                   GameCamera.instance && !Console.IsVisible() &&
                   !Menu.IsVisible() && !InventoryGui.IsVisible() &&
                   !StoreGui.IsVisible() && !Minimap.IsOpen() &&
                   !Hud.IsPieceSelectionVisible() && !Hud.InRadial() &&
                   (Chat.instance == null || !Chat.instance.HasFocus());
        }

        // Checks the configured key and its modifiers.
        private static bool IsToggleShortcutDown()
        {
            KeyboardShortcut shortcut = Preference.ToggleShortcut;
            if (shortcut.MainKey == KeyCode.None ||
                !ZInput.GetKeyDown(shortcut.MainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!ZInput.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
