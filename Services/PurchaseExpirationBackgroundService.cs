using LibaryManagement.Services.Interfaces;

namespace LibaryManagement.Services
{
    public class PurchaseExpirationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PurchaseExpirationBackgroundService> _logger;

        public PurchaseExpirationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<PurchaseExpirationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var purchaseService = scope.ServiceProvider
                            .GetRequiredService<IPurchaseService>();

                        await purchaseService.ExpirePurchasesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while expiring purchases.");
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
        }
    }
}
