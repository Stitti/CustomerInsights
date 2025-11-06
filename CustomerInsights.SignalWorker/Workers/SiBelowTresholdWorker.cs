using CustomerInsights.Base.Models;
using CustomerInsights.SignalWorker.Detectors;
using CustomerInsights.SignalWorker.Models;
using CustomerInsights.SignalWorker.Repositories;

namespace CustomerInsights.SignalWorker.Workers;

public sealed class SiBelowThresholdWorker : BackgroundService
{
    private readonly SignalRepository _signalRepository;
    private readonly OutboxRepository _outboxRepository;
    private readonly SatisfactionStateRepository _satisfactionStateRepository;
    private readonly ILogger<SiBelowThresholdWorker> _logger;
    private readonly SiBelowThresholdDetector _detector;

    public SiBelowThresholdWorker(SignalRepository signalRepository, OutboxRepository outboxRepository, SatisfactionStateRepository satisfactionStateRepository, ILogger<SiBelowThresholdWorker> logger, SiBelowTresholdSignalConfig configuration)
    {
        _signalRepository = signalRepository;
        _outboxRepository = outboxRepository;
        _satisfactionStateRepository = satisfactionStateRepository;
        _logger = logger;
        _detector = new SiBelowThresholdDetector(configuration);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SI-below-threshold worker started.");

        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                OutboxMessage? message = await _outboxRepository.FetchNextOutboxAsync("AccountSiUpdated", cancellationToken);
                if (message == null)
                {
                    await Task.Delay(500, cancellationToken);
                    continue;
                }

                double currentSi = await _satisfactionStateRepository.GetAccountSiAsync(message.TenantId, message.TargetId, cancellationToken);

                Signal? signal = _detector.Detect(message.TenantId, message.TargetId, currentSi, DateTime.UtcNow);
                if (signal != null)
                {
                    await _signalRepository.InsertSignalAsync(signal, cancellationToken);
                    _logger.LogInformation("Signal stored: tenant={Tenant} account={Account} SI={Si:F1} (< {Thr:F1})", message.TenantId, message.TargetId, currentSi, signal.Threshold);
                }

                await _outboxRepository.AckOutboxAsync(message.Id, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while processing SI-below-threshold loop.");
                await Task.Delay(1500, cancellationToken);
            }
        }
    }
}
