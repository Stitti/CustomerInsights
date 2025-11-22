using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Repositories;

namespace CustomerInsights.ApiService.Services
{
    public sealed class SignalService
    {
        private readonly SignalRepository _repository;
        private readonly ILogger<SignalService> _logger;

        public SignalService(SignalRepository repository, ILogger<SignalService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<SignalDto>> GetAll(CancellationToken cancellation = default)
        {
            return await _repository.GetAllAsync(cancellation);
        }

        public async Task<SignalDto?> GetById(Guid id, CancellationToken cancellation = default)
        {
            return await _repository.GetById(id, cancellation);
        }

        internal async Task<bool> PatchAsync(Guid id, UpdateSignalRequest request,  CancellationToken cancellation = default)
        {
            return await _repository.PatchAsync(id, request, cancellation);
        }
    }
}
