using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using Polly.Extensions.Http;
using Polly;
using Models;
using Data;

namespace WebApi.BackgroundServices
{
    public class TasaBackgroundService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;

        public TasaBackgroundService(IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
        {
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromMinutes(5));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var client = _httpClientFactory.CreateClient();

                        // Usamos "officialRate" para ser específicos
                        var response = await client.GetAsync("https://ve.dolarapi.com/v1/dolares/oficial", stoppingToken);
                        response.EnsureSuccessStatusCode();

                        var rateData = await response.Content.ReadFromJsonAsync<Rate>(cancellationToken: stoppingToken);

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            // "rateService" suena más profesional que "dRate"
                            var rateService = scope.ServiceProvider.GetRequiredService<dRate>();
                            var result = await rateService.Insert_Rate(rateData);

                            if (result.Processed)
                                Util.Log.Info(result.Message);
                            else
                                Util.Log.Error(result.Message);
                        }

                        return response;
                    });
                }
                catch (Exception ex)
                {
                    Util.Log.Error($"Error in BackgroundService: {ex.Message}");
                }

                await ScheduleNextExecution(stoppingToken);
            }
        }

        private async Task ScheduleNextExecution(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            var fiveAmToday = now.Date.AddHours(5);
            var nextExecution = now < fiveAmToday ? fiveAmToday : fiveAmToday.AddDays(1);

            var delay = nextExecution - now;

            Util.Log.Info($"Next execution scheduled for: {nextExecution}");

            await Task.Delay(delay, stoppingToken);
        }
    }
}