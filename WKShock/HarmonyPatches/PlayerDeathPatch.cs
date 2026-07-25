using HarmonyLib;
using UnityEngine;

namespace WKShock.HarmonyPatches
{
    [HarmonyPatch(typeof(ENT_Player))]
    public class PlayerDeathPatch
    {
        [HarmonyPatch("Kill")]
        [HarmonyPrefix]
        public static void BeforePlayerDies(bool ___dead, bool ___godmode)
        {
            if (WKShockPlugin.PiShock == null || WKShockPlugin.PiShock.Controller == null) return;
            if (!WKShockPlugin.PiShock.ModEnabled.Value) return;
            if (___dead || ___godmode) return;

            Debug.Log("Player died.");

            if (WKShockPlugin.PiShock.EnableDeathShock.Value)
            {
                WKShockPlugin.PiShock.Controller.OperatePiShock(
                    WKShockPlugin.PiShock.DeathShockIntensity.Value,
                    WKShockPlugin.PiShock.DeathShockDuration.Value,
                    "shock"
                );
            }
        }
    }
}