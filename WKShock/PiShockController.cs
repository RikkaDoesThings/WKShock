using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
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
                var payload = string.Format(CultureInfo.InvariantCulture, "{{\"intensity\":{0},\"duration\":{1}}}", safeIntensity, safeDuration);

                foreach (var deviceId in _deviceIds)
                {
                    var endpoint = $"{_baseUrl.TrimEnd('/')}/v1/shockers/{Uri.EscapeDataString(deviceId)}/{normalizedOperation}";
                    using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                    {
                        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        if (!string.IsNullOrWhiteSpace(_apiToken))
                        {
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
                        }

                        try
                        {
                            var response = await _httpClient.SendAsync(request);
                            if (!response.IsSuccessStatusCode)
                            {
                                var body = await response.Content.ReadAsStringAsync();
                                _logger.LogWarning($"OpenShock request failed for device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase} - {body}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"OpenShock request threw for device {deviceId}: {ex.Message}");
                        }
                    }
                }
            });
        }
    }
}