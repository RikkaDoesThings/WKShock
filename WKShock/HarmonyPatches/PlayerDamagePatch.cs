using HarmonyLib;
using UnityEngine;
using PiShockLibrary.Enums;

namespace WKShock.HarmonyPatches
{
    [HarmonyPatch(typeof(ENT_Player))]
    public class PlayerDamagePatch
    {
        [HarmonyPatch("Damage")]
        [HarmonyPrefix]
        public static void BeforePlayerTakesDamage(ENT_Player __instance, Damageable.DamageInfo info, bool ___godmode, float ___health)
        {
            if (WKShockPlugin.PiShock == null || WKShockPlugin.PiShock.Controller == null) return;
            if (!WKShockPlugin.PiShock.ModEnabled.Value) return;
            if (___godmode || info.amount <= 0f) return;
            bool isLethal = (___health - info.amount) <= 0f;
            if (isLethal) return;

            Debug.Log($"Player hit for {info.amount} damage.");

            if (WKShockPlugin.PiShock.EnableShock.Value)
            {
                WKShockPlugin.PiShock.Controller.OperatePiShock(
                    WKShockPlugin.PiShock.ShockIntensity.Value, 
                    WKShockPlugin.PiShock.ShockDuration.Value, 
                    PiShockOperations.Shock
                );
            }

            if (WKShockPlugin.PiShock.EnableVibrate.Value)
            {
                WKShockPlugin.PiShock.Controller.OperatePiShock(
                    WKShockPlugin.PiShock.VibrateIntensity.Value, 
                    WKShockPlugin.PiShock.VibrateDuration.Value, 
                    PiShockOperations.Vibrate
                );
            }
        }
    }
}