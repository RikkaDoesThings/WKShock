//Code borrowed and modified from https://github.com/MxPuffin/REPOShock/blob/master/src/PiShockController.cs
using PiShockClassLibrary.Services;
using PiShockClassLibrary.Models;
using PiShockLibrary.Models;
using PiShockLibrary.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BepInEx.Logging;

namespace WKShock
{
    public class PiShockController
    {
        private PiShockAPI _piShockAPI;
        private PiShockUserInfo _userInfo;
        private ManualLogSource _logger;
        
        public PiShockController(PiShockAPI piShockAPI, PiShockUserInfo piShockUserInfo, ManualLogSource logger)
        {
            _piShockAPI = piShockAPI;
            _userInfo = piShockUserInfo;
            _logger = logger;
        }

        public void OperatePiShock(int intensity, int duration, PiShockOperations op)
        {
            Task.Run(async () =>
            {
                var tasks = new List<Task<bool>>();

                foreach (var device in _userInfo.Devices)
                {
                    var body = new PiShockOperateRequestBody(_userInfo, op, device.ShareCode, intensity, duration, "White Knuckle");
                    tasks.Add(_piShockAPI.SendOperationRequest(body));
                }

                var results = await Task.WhenAll(tasks);
                if (results.Any(success => !success))
                {
                    _logger.LogWarning("One or more PiShock requests failed to execute!");
                }
            });
        }
    }
}