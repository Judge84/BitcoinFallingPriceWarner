using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitcoinFallingPriceWarner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BitcoinFallingPriceWarnerWorkerService
{
    public class BitcoinFallingPriceWarnerWorker : BackgroundService
    {
        private readonly ILogger<BitcoinFallingPriceWarnerWorker> _logger;
        private Timer _timer;
        private static object _lock = new object();
        private int counter = 0;
        private readonly BitcoinFallingPriceWarner.Settings settings;

        public BitcoinFallingPriceWarnerWorker(ILogger<BitcoinFallingPriceWarnerWorker> logger)
        {
            _logger = logger;
            var config = new ConfigurationBuilder()   
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            var section = config.GetSection(nameof(Settings));
            settings = section.Get<Settings>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            double timerInMinutes = settings.TimerInMinutes;
            _timer = new Timer(executeBitcoinFallingPriceWarner, null, TimeSpan.Zero, TimeSpan.FromMinutes(timerInMinutes));
        
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            this.Dispose();
            return base.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }


        private void executeBitcoinFallingPriceWarner(object state)
        {
            _logger.LogInformation($"{counter + 1} Abfrage wird gestartet");

            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    BitcoinFallingPriceWarner.Program.BitcoinFallingPriceWarner(settings);
                }
                finally
                {
                    counter++;
                    Monitor.Exit(_lock);
                }
            }
            _logger.LogInformation($"{counter} Abfrage beendet");

        }
    }
}
