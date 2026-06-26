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
    public class SalesFigoBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SalesFigoBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Util.Log.Info("SalesFigoBackgroundService - CONSTRUCTOR EJECUTADO");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Util.Log.Info("SalesFigoBackgroundService - ExecuteAsync INICIADO");

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

                            // Calcular la fecha del día anterior
                            DateTime previousDay = DateTime.Today.AddDays(-1);
                            //DateTime previousDay = new DateTime(2026, 6, 5);

                            Util.Log.Info($"SalesFigoBackgroundService - Iniciando extracción de ventas para la fecha: {previousDay:yyyy-MM-dd}");

                            // Ejecutar la extracción e inserción
                            var response = await dFigo.ExtractAndInsertSales(previousDay);

                            if (response.Processed)
                            {
                                if (response.Status == 200 && response.Data != null)
                                {
                                    Util.Log.Info($"SalesFigoBackgroundService - Éxito: {response.Message ?? "Ventas procesadas correctamente"}");
                                }
                                else if (response.Status == 200 && response.Data == null)
                                {
                                    Util.Log.Info($"SalesFigoBackgroundService - {response.Message ?? "No se encontraron ventas para la fecha especificada"}");
                                }
                            }
                            else
                            {
                                // Si no fue procesado correctamente, lanzamos excepción para que Polly reintente
                                throw new Exception($"Error en ExtractAndInsertSales: {response.Message}");
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    // Si llega aquí, significa que tras los 3 intentos, el proceso siguió fallando
                    Util.Log.Error($"SalesFigoBackgroundService - Fallaron los 3 intentos para la fecha {DateTime.Today.AddDays(-1):yyyy-MM-dd}. Error final: {ex.Message}");
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
        //  var nextExecution = now.AddMinutes(3); //prueba de cada 3 minutos

            var delay = nextExecution - now;

            Util.Log.Info($"SalesFigoBackgroundService - Próxima ejecución programada para: {nextExecution:yyyy-MM-dd HH:mm:ss}");

            await Task.Delay(delay, stoppingToken);
        }

    }
}