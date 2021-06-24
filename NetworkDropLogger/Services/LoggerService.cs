using System;
using System.IO;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetworkDropLogger.Configuration;
using NetworkDropLogger.Entities;

namespace NetworkDropLogger.Services
{
    public class LoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        private readonly StreamWriter _fileStream;
        private DateTime? _downTime;

        public LoggerService(ILogger<LoggerService> logger, IOptions<DetectorConfiguration> detectorConfig, IOptions<LoggerConfiguration> config)
        {
            _logger = logger;

            _fileStream = File.AppendText(config.Value.FilePath);
            _fileStream.AutoFlush = true;
            _fileStream.WriteLine($"{GetTime()} | Application started");
            _fileStream.WriteLine($"{GetTime()} | Pinging {detectorConfig.Value.PingAddress} every {detectorConfig.Value.DelayBetweenPings}ms with a timeout of {detectorConfig.Value.PingTimeout}ms");
        }
        
        public void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            _logger.LogInformation("State changed from {Old} to {New}", e.Old, e.New);
            if (e.New == ConnectionState.Disconnected)
            {
                _downTime = DateTime.Now;
                _fileStream.WriteLine($"{GetTime()} | Disconnected");
            }

            if (e.New == ConnectionState.Connected)
            {
                if(_downTime is null)
                    _fileStream.WriteLine($"{GetTime()} | Connected");
                else
                {
                    var downFor = DateTime.Now - _downTime.Value;
                    _fileStream.WriteLine($"{GetTime()} | Connected, was disconnected for {downFor.Humanize(2)}");
                }
                    
            }
        }

        public async Task StopAsync()
        {
            await _fileStream.WriteLineAsync($"{GetTime()} | Application stopped");
            await _fileStream.FlushAsync();
        }

        private static string GetTime()
        {
            return DateTime.Now.ToString("G");
        }
    }
}
