using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace WKShock
{
    public class PiShockLogin
    {
        private readonly ManualLogSource _logger;
        private readonly HttpClient _httpClient;
        private ConfigEntry<bool> _useOpenShock;
        private ConfigEntry<string> _apiBaseUrl;
        private ConfigEntry<string> _apiToken;
        private ConfigEntry<string> _deviceIds;
        public ConfigEntry<bool> ModEnabled { get; private set; }
        public ConfigEntry<bool> EnableShock { get; private set; }
        public ConfigEntry<int> ShockIntensity { get; private set; }
        public ConfigEntry<int> ShockDuration { get; private set; }
        public ConfigEntry<bool> EnableVibrate { get; private set; }
        public ConfigEntry<int> VibrateIntensity { get; private set; }
        public ConfigEntry<int> VibrateDuration { get; private set; }
        public ConfigEntry<bool> EnableDeathShock { get; private set; }
        public ConfigEntry<int> DeathShockIntensity { get; private set; }
        public ConfigEntry<int> DeathShockDuration { get; private set; }
        public PiShockController Controller { get; private set; }

        public PiShockLogin(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _useOpenShock = config.Bind("1. Shock Provider", "Use OpenShock", true, "Set to true to use OpenShock, false to use PiShock.");
            _apiBaseUrl = config.Bind("1. Shock Provider", "API Base URL", "https://api.openshock.org", "OpenShock API base URL.");
            _apiToken = config.Bind("1. Shock Provider", "API Token", "", "Optional bearer token for OpenShock.");
            _deviceIds = config.Bind("1. Shock Provider", "Device IDs", "", "Comma-separated OpenShock device IDs.");
            ModEnabled = config.Bind("2. Gameplay Settings", "Mod Enabled", true, "Master switch to enable or disable all mod triggers.");
            EnableShock = config.Bind("3. Shock Settings", "Enable Shock", true, "Send a shock when taking damage.");
            ShockIntensity = config.Bind("3. Shock Settings", "Shock Intensity", 25, new ConfigDescription("Intensity of the shock (1-100).", new AcceptableValueRange<int>(1, 100)));
            ShockDuration = config.Bind("3. Shock Settings", "Shock Duration", 1, new ConfigDescription("Duration of the shock in seconds (1-10).", new AcceptableValueRange<int>(1, 10)));
            EnableVibrate = config.Bind("4. Vibrate Settings", "Enable Vibrate", false, "Send a vibration when taking damage.");
            VibrateIntensity = config.Bind("4. Vibrate Settings", "Vibrate Intensity", 50, new ConfigDescription("Intensity of the vibration (1-100).", new AcceptableValueRange<int>(1, 100)));
            VibrateDuration = config.Bind("4. Vibrate Settings", "Vibrate Duration", 1, new ConfigDescription("Duration of the vibration in seconds (1-10).", new AcceptableValueRange<int>(1, 10)));
            EnableDeathShock = config.Bind("5. Death Settings", "Enable Death Shock", true, "Send a massive shock when dying.");
            DeathShockIntensity = config.Bind("5. Death Settings", "Death Shock Intensity", 50, new ConfigDescription("Intensity of the death shock (1-100).", new AcceptableValueRange<int>(1, 100)));
            DeathShockDuration = config.Bind("5. Death Settings", "Death Shock Duration", 3, new ConfigDescription("Duration of the death shock in seconds (1-10).", new AcceptableValueRange<int>(1, 10)));
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_useOpenShock.Value)
                {
                    if (string.IsNullOrWhiteSpace(_deviceIds.Value))
                    {
                        _logger.LogError("OpenShock: Device IDs missing in config file!");
                        return;
                    }

                    var deviceIds = new List<string>();
                    foreach (var deviceId in _deviceIds.Value.Split(','))
                    {
                        var trimmed = deviceId.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmed))
                        {
                            deviceIds.Add(trimmed);
                        }
                    }

                    if (deviceIds.Count == 0)
                    {
                        _logger.LogError("OpenShock: No valid device IDs were found in config file!");
                        return;
                    }

                    Controller = new PiShockController(_httpClient, _apiBaseUrl.Value, _apiToken.Value, deviceIds, _logger);
                    _logger.LogInfo("OpenShock initialization successful.");
                }
                else
                {
                    _logger.LogInfo("PiShock mode selected; no additional initialization is performed in this build.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Shock backend initialization failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}