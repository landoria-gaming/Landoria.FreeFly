using BepInEx;
using HarmonyLib;

namespace Landoria.FreeFly
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    // Loads and unloads the FreeFly mod.
    public sealed class FreeFlyPlugin : BaseUnityPlugin
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
            _harmony.CreateClassProcessor(typeof(FreeFlyInitializationPatch)).Patch();
            _harmony.CreateClassProcessor(typeof(FreeFlyEscapeMenuPatch)).Patch();
            _harmony.CreateClassProcessor(typeof(FreeFlyMouseWheelPatch)).Patch();
            _harmony.CreateClassProcessor(typeof(FreeFlyMovementPatch)).Patch();
            FreeFlyPreference.Initialize(Config);
            FreeFlyCommands.Register();
            Logger.LogInfo($"{PluginName} {PluginVersion} is loaded.");
        }

        // Reads shortcuts and updates interface visibility.
        private void Update()
        {
            FreeFlyShortcut.Update();
            FreeFlyInterfaceController.Update();
        }

        // Restores game state when the mod unloads.
        private void OnDestroy()
        {
            FreeFlyController.CompleteDisable();
            FreeFlyController.Reset();
            FreeFlyInterfaceController.Restore();
            _harmony?.UnpatchSelf();
            _harmony = null;
            Logger.LogInfo($"{PluginName} {PluginVersion} is unloaded.");
        }
    }
}
