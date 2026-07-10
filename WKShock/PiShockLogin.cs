//Code borrowed and modified from https://github.com/MxPuffin/REPOShock/blob/master/src/REPOShock.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx.Configuration;
using BepInEx.Logging;
using PiShockClassLibrary.Models;
using PiShockClassLibrary.Services;

namespace WKShock
{
    public class PiShockLogin
    {
        private readonly ManualLogSource _logger;
        private readonly PiShockAPI _api;
        private ConfigEntry<string> _username;
        private ConfigEntry<string> _apiKey;
        private ConfigEntry<string> _shareCodes;
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
            _api = new PiShockAPI(new HttpClient());
            _username = config.Bind("1. PiShock Credentials", "Username", "", "Your PiShock Username");
            _apiKey = config.Bind("1. PiShock Credentials", "APIKey", "", "Your PiShock API Key");
            _shareCodes = config.Bind("1. PiShock Credentials", "ShareCodes", "", "Comma separated share codes");
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
                if (string.IsNullOrEmpty(_username.Value) || string.IsNullOrEmpty(_apiKey.Value))
                {
                    _logger.LogError("PiShock: Username or APIKey missing in config file!");
                    return;
                }

                if (string.IsNullOrEmpty(_shareCodes.Value))
                {
                    _logger.LogError("PiShock: ShareCode missing in config file!");
                    return;
                }

                var sharecodes = _shareCodes.Value.Split(',');
                var devices = new List<PiShockDeviceInfo>();

                for (int i = 0; i < sharecodes.Length; i++)
                {
                    devices.Add(new PiShockDeviceInfo($"Shocker {i}", sharecodes[i].Trim()));
                }

                var userInfo = await _api.GetUserInfoFromAPI(_username.Value, _apiKey.Value);
                userInfo = userInfo.WithDevices(devices);

                Controller = new PiShockController(_api, userInfo, _logger); 

                _logger.LogInfo("PiShock login successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"PiShock login failed: {ex.Message}");
            }
        }
    }
}