using System;
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

        public async Task<List<Management>> SearchKeyManagement(string keyword)
        {
            var result = await _elasticClient.SearchAsync<Management>(
                 s => s.Query(q => q.QueryString(d => d.Query('*' + keyword + '*'))));

            if (!result.IsValid)
            {
                //Request notnvalid due to an error
                _logger.LogError("Unable to carry out Elastic Search");
                return new List<Management>();
            }
            return result.Documents.Where(x=>x.mgmt!=null).ToList();
        }

        public async Task<ElasticPostResponse> SaveSingleManagementAsync(Management mgt)
        {
            ElasticPostResponse response;
            try
            {
                if (_cache.Any(p => p.mgmt.mgmtID == mgt.mgmt.mgmtID))
                {
                    await _elasticClient.UpdateAsync<Management>(mgt, u => u.Doc(mgt));
                    response = new ElasticPostResponse {
                        responseDescription = "Record Exists. Update performed",
                        responseCode = 0
                    };
                }
                else
                {
                    _cache.Add(mgt);
                    var resp = await _elasticClient.IndexDocumentAsync<Management>(mgt);
                    response = new ElasticPostResponse {
                        responseDescription = resp.Id,
                        responseCode = 0
                    };
                }

            }
            catch(Exception ex)
            {
                response = new ElasticPostResponse {
                        responseDescription = ex.Message,
                        responseCode = 500
                    };
            }
            return response;
        }

        public async Task<ElasticPostResponse> SaveManyManagementAsync(List<Management> mgts)
        {
            ElasticPostResponse response;
            try
            {
                _cache.AddRange(mgts);
                var result = await _elasticClient.IndexManyAsync(mgts);
                //check for errors for each entry
                if (result.Errors)
                {
                    foreach (var entryWithError in result.ItemsWithErrors)
                    {
                        _logger.LogError("Failed to index management document {0}: {1}", entryWithError.Id, entryWithError.Error);
                    }
                }
                response = new ElasticPostResponse {
                    responseDescription = result.ToString(),
                    responseCode = 0
                };
            }
            catch(Exception ex)
            {
                response = new ElasticPostResponse {
                    responseDescription = ex.Message,
                    responseCode = 500
                };
            }
            return response;
            
        }
    }
}