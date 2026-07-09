using Polly;
using Data;

namespace WebApi.BackgroundServices
{
    public class PartsFigoBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PartsFigoBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Util.Log.Info("PartsFigoBackgroundService - CONSTRUCTOR EJECUTADO");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Util.Log.Info("PartsFigoBackgroundService - ExecuteAsync INICIADO");

            // Política simple para reintentos inmediatos ante fallos técnicos
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

                            var response = await dFigo.ExtractAndInsertParts();
                            
                            if (!response.Processed)
                                throw new Exception($"Error: {response.Message}");
                            
                            Util.Log.Info("PartsFigoBackgroundService - Éxito procesando ventas.");
                        }
                    });
                }
                catch (Exception ex)
                {
                    Util.Log.Error($"PartsFigoBackgroundService - Error crítico: {ex.Message}");
                }

                // ESPERA 2 HORAS
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
    }
}