using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Models;

namespace WebApi.BackgroundServices
{
    public class ReturnsFigoBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ReturnsFigoBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Util.Log.Info("ReturnsFigoBackgroundService - CONSTRUCTOR EJECUTADO");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Util.Log.Info("ReturnsFigoBackgroundService - ExecuteAsync INICIADO");

            // Política de reintentos: 3 intentos, cada 5 minutos
            var retryPolicy = Polly.Policy
                       .Handle<Exception>()
                       .WaitAndRetryAsync(3, _ => TimeSpan.FromMinutes(5));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var dFigo = scope.ServiceProvider.GetRequiredService<dFigo>();

                            Util.Log.Info($"ReturnsFigoBackgroundService - Iniciando extracción de devoluciones");

                            // Ejecutar la extracción e inserción
                            var response = await dFigo.ExtractAndInsertReturns();

                            if (response.Processed)
                            {
                                if (response.Status == 200 && response.Data != null)
                                {
                                    Util.Log.Info($"ReturnsFigoBackgroundService - Éxito: {response.Message ?? "Devoluciones procesadas correctamente"}");
                                }
                                else if (response.Status == 200 && response.Data == null)
                                {
                                    Util.Log.Info($"ReturnsFigoBackgroundService - {response.Message ?? "No se encontraron devoluciones para la fecha especificada"}");
                                }
                            }
                            else
                            {
                                // Si no fue procesado correctamente, lanzamos excepción para que Polly reintente
                                throw new Exception($"Error en ExtractAndInsertReturns: {response.Message}");
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    // Si llega aquí, significa que tras los 3 intentos, el proceso siguió fallando
                    Util.Log.Error($"ReturnsFigoBackgroundService - Fallaron los 3 intentos. Error final: {ex.Message}");
                }

                // Esperar hasta la próxima ejecución (5:00 AM del día siguiente)
                await ScheduleNextExecution(stoppingToken);
            }
        }

        private async Task ScheduleNextExecution(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            var fiveAmToday = now.Date.AddHours(5);
            var nextExecution = now < fiveAmToday ? fiveAmToday : fiveAmToday.AddDays(1);

            Util.Log.Info($"ReturnsFigoBackgroundService - Próxima ejecución programada para: {nextExecution:yyyy-MM-dd HH:mm:ss}");

            await Task.Delay(nextExecution - now, stoppingToken);
        }
    }
}