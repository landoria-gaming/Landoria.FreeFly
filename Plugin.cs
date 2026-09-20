using BepInEx;
using HarmonyLib;
using Landoria.Shared;

namespace Landoria.FreeFly
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    // Loads and unloads the FreeFly mod.
    public sealed class Plugin : BaseUnityPlugin
    {
        internal const string PluginGuid = "Landoria.FreeFly";
        internal const string PluginName = "Landoria.FreeFly";
        internal const string PluginVersion = "1.0.0";
        private Harmony _harmony;

        // Loads settings, commands, and patches.
        private void Awake()
        {
            Logger.LogInfo($"AssemblyVersion: {GetType().Assembly.GetName().Version}.");
            _harmony = new Harmony(PluginGuid);
            _harmony.CreateClassProcessor(typeof(InitializationPatch)).Patch();
            _harmony.CreateClassProcessor(typeof(EscapeMenuPatch)).Patch();
            _harmony.CreateClassProcessor(typeof(MouseWheelPatch)).Patch();
            _harmony.CreateClassProcessor(typeof(MovementPatch)).Patch();
            Preference.Initialize(Config);
            ConfigWatcher.Initialize(
                Config,
                Logger,
                "Free Fly",
                () => Preference.RestoreDefaults(Config));
            Commands.Register();
            Logger.LogInfo($"{PluginName} {PluginVersion} is loaded.");
        }

        // Reads shortcuts and updates interface visibility.
        private void Update()
        {
            ConfigWatcher.Update();
            Shortcut.Update();
            InterfaceController.Update();
        }

        // Restores game state when the mod unloads.
        private void OnDestroy()
        {
            ConfigWatcher.Dispose();
            Controller.CompleteDisable();
            Controller.Reset();
            InterfaceController.Restore();
            _harmony?.UnpatchSelf();
            _harmony = null;
            Logger.LogInfo($"{PluginName} {PluginVersion} is unloaded.");
        }
    }
}
