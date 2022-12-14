using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting event consumer hosted service");

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var eventHandler = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

                if (string.IsNullOrEmpty(topic))
                {
                    throw new ArgumentNullException(nameof(topic), "KAFKA_TOPIC environment variable is not set");
                }

                Task.Run(() => eventHandler.Consume(topic), cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer hosted service stopped");

            return Task.CompletedTask;
        }
    }
}