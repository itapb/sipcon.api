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

            //var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
            //     .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(5));

            while (!stoppingToken.IsCancellationRequested)
            {
                bool success = false;

                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var client = _httpClientFactory.CreateClient();
                        //var client = _httpClientFactory.CreateClient();
                        //client.Timeout = TimeSpan.FromSeconds(10);

                        // 1. Intentamos consultar la API
                        var response = await client.GetAsync("https://ve.dolarapi.com/v1/dolares/oficial", stoppingToken);

                        // Si la API responde con un error (ej. 500, 503), esto lanzará una excepción 
                        // que Polly atrapará para reintentar.
                        response.EnsureSuccessStatusCode();

                        // 2. Intentamos leer el JSON
                        var rateData = await response.Content.ReadFromJsonAsync<Rate>(cancellationToken: stoppingToken);

                        // Si llegamos aquí, la API respondió y el JSON es válido.
                        if (rateData != null)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var dRate = scope.ServiceProvider.GetRequiredService<dRate>();
                                var result = await dRate.Insert_Rate(rateData);

                                if (result.Processed)
                                    Util.Log.Info(result.Message);
                                else
                                    Util.Log.Error(result.Message);
                            }
                            success = true; // Todo salió bien
                        }

                        return response;
                    });
                }
                catch (Exception ex)
                {
                    // Si llega aquí, significa que tras los 3 intentos, la API siguió fallando
                    Util.Log.Error($"API unreachable after retries: {ex.Message}");
                    success = false;
                }

                // 3. Lógica de respaldo: solo si 'success' sigue siendo false, insertamos 0
                if (!success)
                {
                    Util.Log.Error("Applying fallback: Inserting 0 due to API failure.");

                    var fallbackRate = new Rate
                    {
                        DDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        NRate = 0
                    };

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dRate = scope.ServiceProvider.GetRequiredService<dRate>();
                        await dRate.Insert_Rate(fallbackRate);
                    }
                }

                // 4. Esperar hasta la próxima ejecución (4:30 PM)
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