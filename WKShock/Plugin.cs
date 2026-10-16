using BepInEx;
using HarmonyLib;

namespace WKShock
{
    [BepInPlugin("com.xironix.wkshock", "WKShock", "1.0.1")]
    public class WKShockPlugin : BaseUnityPlugin
    {
        public static PiShockLogin PiShock { get; private set; }

        private void Awake()
        {
            Logger.LogInfo("WKShock is waking up...");
            Logger.LogInfo($"WKShock 1.0.1 loaded. Config: {Config.ConfigFilePath}");
            PiShock = new PiShockLogin(Config, Logger);
            _ = PiShock.InitializeAsync();

            var harmony = new Harmony("com.xironix.wkshock");
            harmony.PatchAll();
        }
    }
}