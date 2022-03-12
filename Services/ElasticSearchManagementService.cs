using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using SmartApartmentData.Model;

namespace SmartApartmentData.Services
{
    public class ElasticSearchManagementService : IManagementService
    {
        private List<Management> _cache = new List<Management>();
        private readonly IElasticClient _elasticClient;
        private readonly ILogger _logger;

        public ElasticSearchManagementService(IElasticClient elasticClient, ILogger<ElasticSearchManagementService> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        
        public virtual Task<IEnumerable<Management>> GetManagementListCount(int count, int skip = 0)
        {
            var mgt = _cache
                .Skip(skip)
                .Take(count);

            return Task.FromResult(mgt );
        }
        

        public virtual Task<Management> GetManagementById(int id)
        {
            var mgt = _cache
            .FirstOrDefault(p => p.mgmt.mgmtID == id);

            return Task.FromResult(mgt);
        }

        public Task<List<Management>> GetAllManagement()
        {
            var mgt = _cache;

            return Task.FromResult(mgt);
        }


        public async Task DeleteManagementAsync(Management mgt)
        {
            await _elasticClient.DeleteAsync<Management>(mgt);

            if (_cache.Contains(mgt))
            {
                _cache.Remove(mgt);
            }
        }

        public async Task SaveSingleManagementAsync(Management mgt)
        {
            if (_cache.Any(p => p.mgmt.mgmtID == mgt.mgmt.mgmtID))
            {
                var resp = await _elasticClient.UpdateAsync<Management>(mgt, u => u.Doc(mgt));
            }
            else
            {
                _cache.Add(mgt);
                var resp = await _elasticClient.IndexDocumentAsync<Management>(mgt);
            }
        }

        public async Task SaveManyManagementAsync(Management[] mgts)
        {
            _cache.AddRange(mgts);
            var result = await _elasticClient.IndexManyAsync(mgts);
            //check for errors for each entry
            if (result.Errors)
            {
                foreach (var entryWithError in result.ItemsWithErrors)
                {
                    _logger.LogError("Failed to index management document {0}: {1}",
                        entryWithError.Id, entryWithError.Error);
                }
            }
        }

        public async Task SaveBulkAsync(Management[] mgts)
        {
            _cache.AddRange(mgts);
            var result = await _elasticClient.BulkAsync(b => b.Index("management").IndexMany(mgts));
            //check for errors for each entry
            if (result.Errors)
            {
                foreach (var entryWithError in result.ItemsWithErrors)
                {
                    _logger.LogError("Failed to index management document {0}: {1}",
                        entryWithError.Id, entryWithError.Error);
                }
            }
        }
    }
}