using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetworkDropLogger.Configuration;
using NetworkDropLogger.Entities;

namespace NetworkDropLogger.Services
{
    internal class DetectorService
    {
        public event EventHandler<StateChangedEventArgs> StateChanged;

        private readonly DetectorConfiguration _config;

        public DetectorService(IOptions<DetectorConfiguration> config)
        {
            _config = config.Value;
        }

        public async Task RunAsync(CancellationToken stoppingToken)
        {
            var currentConnectionState = ConnectionState.Undefined;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_config.DelayBetweenPings, stoppingToken);
                var newConnectionState = await CheckAsync();
                if (currentConnectionState != newConnectionState)
                {
                    var eventArgs = new StateChangedEventArgs
                    {
                        Old = currentConnectionState,
                        New = newConnectionState
                    };
                    currentConnectionState = newConnectionState;
                    StateChanged.Invoke(this, eventArgs);
                }
            }
        }

        private async Task<ConnectionState> CheckAsync()
        {
            try
            {
                var response = await new Ping().SendPingAsync(_config.PingAddress, _config.PingTimeout);
                return response.Status == IPStatus.Success
                    ? ConnectionState.Connected
                    : ConnectionState.Disconnected;
            }
            catch
            {
                return ConnectionState.Disconnected;
            }
        }
    }
}
