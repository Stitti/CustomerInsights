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

        public async Task<IEnumerable<SignalDto>> GetAll(CancellationToken ct = default)
        {
            return await _repository.GetAllAsync();
        }

        public async Task<SignalDto?> GetById(Guid id, CancellationToken ct = default)
        {
            return await _repository.GetById(id);
        }
    }
}
