using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace NetworkDropLogger.Services
{
    internal class NetworkDropLoggerService : IHostedService
    {
        private readonly DetectorService _detector;
        private readonly LoggerService _logger;

        public NetworkDropLoggerService(DetectorService detector, LoggerService logger)
        {
            _detector = detector;
            _logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _detector.StateChanged += _logger.OnStateChanged;
            _ = _detector.RunAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _logger.StopAsync();
        }
    }
}
