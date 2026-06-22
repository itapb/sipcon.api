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
    public class TransitFigoBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TransitFigoBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Util.Log.Info("TransitFigoBackgroundService - CONSTRUCTOR EJECUTADO");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Util.Log.Info("TransitFigoBackgroundService - ExecuteAsync INICIADO");

            // Política de reintentos: 3 intentos, cada 5 minutos (igual que SalesFigoBackgroundService)
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

                            Util.Log.Info("TransitFigoBackgroundService - Iniciando extracción de tránsito de repuestos");

                            // Ejecutar la extracción e inserción de tránsito
                            var response = await dFigo.ExtractAndInsertTransit();

                            // En TransitFigoBackgroundService.ExecuteAsync
                            if (response.Processed)
                            {
                                if (response.Status == 200 && response.Data != null)
                                {
                                    if (response.Data.UpdatedRows > 0)
                                    {
                                        Util.Log.Info($"TransitFigoBackgroundService - Éxito: {response.Message}");
                                        Util.Log.Info($"TransitFigoBackgroundService - Repuestos actualizados: {response.Data.UpdatedRows}");
                                    }
                                    else
                                    {
                                        Util.Log.Info($"TransitFigoBackgroundService - {response.Message ?? "No se encontraron repuestos para actualizar"}");
                                    }
                                }
                                else if (response.Status == 200 && response.Data == null)
                                {
                                    Util.Log.Info($"TransitFigoBackgroundService - {response.Message ?? "No se encontraron datos de tránsito para procesar"}");
                                }
                            }
                            else
                            {
                                throw new Exception($"Error en ExtractAndInsertTransit: {response.Message}");
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    // Si llega aquí, significa que tras los 3 intentos, el proceso siguió fallando
                    Util.Log.Error($"TransitFigoBackgroundService - Fallaron los 3 intentos. Error final: {ex.Message}");
                    Util.Log.Error($"TransitFigoBackgroundService - Stack trace: {ex.StackTrace}");
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
            // var nextExecution = now.AddMinutes(3); //prueba de cada 3 minutos (descomentar para pruebas)

            var delay = nextExecution - now;

            Util.Log.Info($"TransitFigoBackgroundService - Próxima ejecución programada para: {nextExecution:yyyy-MM-dd HH:mm:ss}");

            await Task.Delay(delay, stoppingToken);
        }
    }
}