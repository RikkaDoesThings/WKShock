using BepInEx;
using HarmonyLib;

namespace WKShock
{
    [BepInPlugin("com.xironix.wkshock", "WKShock", "1.0.0")]
    public class WKShockPlugin : BaseUnityPlugin
    {
        public static PiShockLogin PiShock { get; private set; }

        private void Awake()
        {
            Logger.LogInfo("WKShock is waking up...");
            PiShock = new PiShockLogin(Config, Logger);
            _ = PiShock.InitializeAsync();

            var harmony = new Harmony("com.xironix.wkshock");
            harmony.PatchAll();
        }
    }
}