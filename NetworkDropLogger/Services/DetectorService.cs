using System;
using System.Net.NetworkInformation;
using System.Timers;
using Microsoft.Extensions.Options;
using NetworkDropLogger.Configuration;
using NetworkDropLogger.Entities;

namespace NetworkDropLogger.Services
{
    internal class DetectorService
    {
        public event EventHandler<StateChangedEventArgs> StateChanged;

        private readonly DetectorConfiguration _config;
        
        private readonly Timer _timer;
        private ConnectionState _connectionState;
        
        public DetectorService(IOptions<DetectorConfiguration> config)
        {
            _config = config.Value;
            
            _timer = new(_config.DelayBetweenPings);
            _timer.Elapsed += OnTimerElapsed;
        }
        
        public void Start()
        {
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var newConnectionState = Check();
            if (_connectionState != newConnectionState)
            {
                var eventArgs = new StateChangedEventArgs
                {
                    Old = _connectionState,
                    New = newConnectionState
                };
                _connectionState = newConnectionState;
                StateChanged.Invoke(this, eventArgs);
            }
        }

        private ConnectionState Check()
        {
            return new Ping().Send(_config.PingAddress, _config.PingTimeout).Status == IPStatus.Success
                ? ConnectionState.Connected 
                : ConnectionState.Disconnected;
        }
    }
}
