using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;

namespace WKShock
{
    public class PiShockController
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiToken;
        private readonly List<string> _deviceIds;
        private readonly ManualLogSource _logger;

        public PiShockController(HttpClient httpClient, string baseUrl, string apiToken, List<string> deviceIds, ManualLogSource logger)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _apiToken = apiToken;
            _deviceIds = deviceIds;
            _logger = logger;
        }

        public void OperatePiShock(int intensity, int duration, string operation)
        {
            _ = Task.Run(async () =>
            {
                var normalizedOperation = (operation ?? string.Empty).Trim().ToLowerInvariant();
                if (normalizedOperation != "shock" && normalizedOperation != "vibrate")
                {
                    _logger.LogWarning($"Unsupported OpenShock operation '{operation}'.");
                    return;
                }

                var safeIntensity = Math.Max(1, Math.Min(100, intensity));
                var safeDuration = Math.Max(1, Math.Min(10, duration));
                var operationType = normalizedOperation == "shock" ? "Shock" : "Vibrate";
                var shockEntries = new List<string>();
                foreach (var deviceId in _deviceIds)
                {
                    shockEntries.Add(string.Format(CultureInfo.InvariantCulture,
                        "{{\"id\":\"{0}\",\"type\":\"{1}\",\"intensity\":{2},\"duration\":{3}}}",
                        deviceId, operationType, safeIntensity, safeDuration * 1000));
                }

                var payload = "{\"shocks\":[" + string.Join(",", shockEntries) + "],\"customName\":\"WKShock\"}";
                var endpoint = _baseUrl.TrimEnd('/') + "/2/shockers/control";
                using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                {
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    request.Headers.Add("User-Agent", "WKShock/1.0.1");
                    request.Headers.Add("Open-Shock-Token", _apiToken);

                    try
                    {
                        using (var response = await _httpClient.SendAsync(request))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                var body = await response.Content.ReadAsStringAsync();
                                _logger.LogWarning($"OpenShock request failed: {(int)response.StatusCode} {response.ReasonPhrase} - {body}");
                            }
                            else
                            {
                                _logger.LogInfo($"OpenShock {normalizedOperation} command accepted for {_deviceIds.Count} device(s).");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"OpenShock request failed: {ex}");
                    }
                }
            });
        }
    }
}