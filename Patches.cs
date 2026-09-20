using HarmonyLib;
using UnityEngine;

namespace Landoria.FreeFly
{
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.ToggleFreeFly))]
    // Prepares camera values when free fly starts.
    internal static class InitializationPatch
    {
        // Resets and positions the newly enabled camera.
        private static void Postfix(
            GameCamera __instance, ref float ___m_freeFlySpeed,
            ref float ___m_freeFlyYaw, ref float ___m_freeFlyPitch,
            ref Quaternion ___m_freeFlyRef, ref Vector3 ___m_freeFlySavedVel,
            ref Transform ___m_freeFlyTarget, ref Transform ___m_freeFlyLockon,
            ref Vector3 ___m_freeFlyVel)
        {
            if (!GameCamera.InFreeFly())
            {
                return;
            }

            Vector3 startPosition = __instance.transform.position;
            Quaternion startRotation = __instance.transform.rotation;
            Controller.SetDefaultSpeed(ref ___m_freeFlySpeed);
            Controller.InitializeActivation(__instance);
            Controller.SynchronizeRotation(
                __instance.transform.forward,
                ref ___m_freeFlyYaw, ref ___m_freeFlyPitch);
            ___m_freeFlyRef = Quaternion.identity;
            ___m_freeFlySavedVel = Vector3.zero;
            ___m_freeFlyTarget = null;
            ___m_freeFlyLockon = null;
            ___m_freeFlyVel = Vector3.zero;
            TransitionController.StartEntering(
                __instance, startPosition, startRotation);
        }
    }

    [HarmonyPatch(typeof(Menu), "Update")]
    // Prevents Escape from opening the menu during exit.
    internal static class EscapeMenuPatch
    {
        // Closes a menu opened by the exit key.
        private static void Postfix()
        {
            Shortcut.CloseSuppressedMenu();
        }
    }

    [HarmonyPatch(typeof(ZInput), nameof(ZInput.GetMouseScrollWheel))]
    // Disables mouse-wheel speed changes while free fly is active.
    internal static class MouseWheelPatch
    {
        // Returns no scrolling while Valheim updates free fly.
        private static bool Prefix(ref float __result)
        {
            if (!GameCamera.InFreeFly())
            {
                return true;
            }

            __result = 0f;
            return false;
        }
    }

    [HarmonyPatch(typeof(GameCamera), "UpdateFreeFly")]
    // Limits and transitions free-fly movement.
    internal static class MovementPatch
    {
        // Saves the frame origin and clamps speed.
        private static void Prefix(
            GameCamera __instance, float dt,
            ref float ___m_freeFlySpeed, out Vector3 __state)
        {
            Controller.UpdateSpeed(ref ___m_freeFlySpeed, dt);
            __state = __instance.transform.position;
        }

        // Applies movement limits and transitions.
        private static void Postfix(
            GameCamera __instance, float dt, ref float ___m_freeFlySpeed, Vector3 __state)
        {
            ___m_freeFlySpeed = Controller.ClampSpeed(___m_freeFlySpeed);
            Controller.ClampFrameMovement(__instance, __state, dt);
            Controller.ClampToCollision(__instance, __state);
            Controller.ClampToPlayer(__instance);
            TransitionController.ApplyEntering(__instance, dt);
            TransitionController.ApplyExiting(__instance, dt);
        }
    }

}
