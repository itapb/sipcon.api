using Polly;
using Data;

namespace WebApi.BackgroundServices
{
    public class MasterSalesFigoBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MasterSalesFigoBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Util.Log.Info("MasterSalesFigoBackgroundService - CONSTRUCTOR EJECUTADO");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Util.Log.Info("MasterSalesFigoBackgroundService - ExecuteAsync INICIADO");

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
                            var response_id = await dFigo.GetMasterSaleIds();

                            var list_id = "";

                            foreach (Models.FIGO_MastersID row in response_id.Data)
                            {
                                list_id = list_id + "'" + row.Id+ "',";
                            }

                            list_id = list_id.TrimEnd(',');

                            var response = await dFigo.ExtractAndInsertMasterSales(list_id);
                            
                            if (!response.Processed)
                                throw new Exception($"Error: {response.Message}");
                            

                            Util.Log.Info("MasterSalesFigoBackgroundService - Éxito procesando ventas.");
                        }
                    });
                }
                catch (Exception ex)
                {
                    Util.Log.Error($"MasterSalesFigoBackgroundService - Error crítico: {ex.Message}");
                }

                // ESPERA 2 HORAS
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
    }
}