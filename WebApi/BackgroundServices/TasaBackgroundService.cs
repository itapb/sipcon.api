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
            // Configuramos la política para manejar errores de red, 5xx y 404
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2));

            while (!stoppingToken.IsCancellationRequested)
            {
                bool success = false;
                // Definimos las fechas a consultar: Hoy, Mañana, Pasado Mañana
                var daysToTry = new[] { DateTime.Today, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2) };

                foreach (var date in daysToTry)
                {
                    try
                    {
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            var client = _httpClientFactory.CreateClient();
                            string formattedDate = date.ToString("yyyy/MM/dd");
                            string url = $"https://ve.dolarapi.com/v1/historicos/dolares/oficial/{formattedDate}";

                            var response = await client.GetAsync(url, stoppingToken);

                            // Si es 404, lanzamos una excepción para que Polly la capture y reintente 
                            // o para que el flujo pase a la siguiente fecha en el foreach
                            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                                throw new HttpRequestException($"No data found for {formattedDate}", null, System.Net.HttpStatusCode.NotFound);

                            response.EnsureSuccessStatusCode();

                            var rateData = await response.Content.ReadFromJsonAsync<InsertRate>(cancellationToken: stoppingToken);

                            if (rateData != null)
                            {
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var dRate = scope.ServiceProvider.GetRequiredService<dRate>();
                                    var result = await dRate.Insert_Rate(rateData);

                                    if (result.Processed)
                                    {
                                        Util.Log.Info($"Data processed for {formattedDate}: {result.Message}");
                                        success = true;
                                    }
                                    else
                                    {
                                        Util.Log.Error($"Error inserting data for {formattedDate}: {result.Message}");
                                    }
                                }
                            }
                            return response;
                        });

                        if (success) break; // Si ya insertamos un valor, salimos del ciclo de fechas
                    }
                    catch (Exception ex)
                    {
                        Util.Log.Error($"Failed to retrieve/process data for {date:yyyy-MM-dd}: {ex.Message}");
                    }
                }

                // Lógica de respaldo: Si ninguna fecha tuvo éxito
                if (!success)
                {
                    Util.Log.Error("All attempts failed. Applying fallback: Inserting 0 for today.");

                    var fallbackRate = new InsertRate
                    {
                        DDate = DateTime.Now,
                        NRate = 0
                    };

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dRate = scope.ServiceProvider.GetRequiredService<dRate>();
                        await dRate.Insert_Rate(fallbackRate);
                    }
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