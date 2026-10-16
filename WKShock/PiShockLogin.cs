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
        private ConfigEntry<string> _legacyApiBaseUrl;
        private ConfigEntry<string> _legacyApiToken;
        private ConfigEntry<string> _legacyDeviceIds;
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
            _apiToken = config.Bind("1. Shock Provider", "API Token", "", "OpenShock bearer token.");
            _deviceIds = config.Bind("1. Shock Provider", "Device IDs", "", "Comma-separated OpenShock device UUIDs.");
            _legacyApiBaseUrl = config.Bind("1. Shock Credentials", "API Base URL", "", "Legacy OpenShock API base URL.");
            _legacyApiToken = config.Bind("1. Shock Credentials", "API Token", "", "Legacy OpenShock API token.");
            _legacyDeviceIds = config.Bind("1. Shock Credentials", "Device IDs", "", "Legacy comma-separated OpenShock device IDs.");
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
                    var deviceIdsValue = FirstConfiguredValue(_deviceIds.Value, _legacyDeviceIds.Value);
                    var apiBaseUrl = FirstConfiguredValue(_apiBaseUrl.Value, _legacyApiBaseUrl.Value, "https://api.openshock.org");
                    var apiToken = FirstConfiguredValue(_apiToken.Value, _legacyApiToken.Value);

                    if (string.IsNullOrWhiteSpace(deviceIdsValue))
                    {
                        _logger.LogError("OpenShock: Device IDs are empty. Set Device IDs in [1. Shock Provider].");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(apiToken))
                    {
                        _logger.LogError("OpenShock: API Token is empty. Add a valid token in [1. Shock Provider].");
                        return;
                    }

                    Uri parsedBaseUrl;
                    if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out parsedBaseUrl) ||
                        (parsedBaseUrl.Scheme != Uri.UriSchemeHttps && parsedBaseUrl.Scheme != Uri.UriSchemeHttp))
                    {
                        _logger.LogError("OpenShock: API Base URL must be a valid HTTP or HTTPS URL.");
                        return;
                    }

                    var deviceIds = new List<string>();
                    foreach (var deviceId in deviceIdsValue.Split(','))
                    {
                        var trimmed = deviceId.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmed))
                        {
                            Guid parsedDeviceId;
                            if (!Guid.TryParse(trimmed, out parsedDeviceId))
                            {
                                _logger.LogError("OpenShock: Device IDs must be OpenShock UUIDs. The configured value is not valid.");
                                return;
                            }
                            deviceIds.Add(trimmed);
                        }
                    }

                    if (deviceIds.Count == 0)
                    {
                        _logger.LogError("OpenShock: No valid device IDs were found in config file!");
                        return;
                    }

                    Controller = new PiShockController(_httpClient, parsedBaseUrl.ToString(), apiToken, deviceIds, _logger);
                    _logger.LogInfo($"OpenShock initialized with {deviceIds.Count} device ID(s).");
                }
                else
                {
                    _logger.LogError("PiShock mode is not implemented in this build. Set Use OpenShock = true.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Shock backend initialization failed: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private static string FirstConfiguredValue(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }
    }
}