using Microsoft.Extensions.Logging;
using Nest;
using SmartApartmentData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 using SmartApartmentData.Interfaces;

namespace SmartApartmentData.Services
{
    public class ElasticSearchPropertiesService : IPropertyService
    {
        private List<Properties> _cache = new List<Properties>();
        private readonly IElasticClient _elasticClient;
        private readonly ILogger _logger;

        public ElasticSearchPropertiesService(IElasticClient elasticClient, ILogger<ElasticSearchManagementService> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }
        Task IPropertyService.DeletePropertyAsync(Properties prop)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Properties> GetPropertyById(int id)
        {
            var props = _cache
            .FirstOrDefault(p => p.property.propertyID == id);

            return Task.FromResult(props);
        }

        public virtual Task<IEnumerable<Properties>> GetPropertyListCount(int count, int skip = 0)
        {
            var prop = _cache
                .Skip(skip)
                .Take(count);

            return Task.FromResult(prop);
        }

        public async Task<ElasticPostResponse> SaveManyPropertiesAsync(List<Properties> props)
        {
            ElasticPostResponse response;
            try
            {
                _cache.AddRange(props);
                var result = await _elasticClient.IndexManyAsync(props);
                //check for errors for each entry
                if (result.Errors)
                {
                    foreach (var entryWithError in result.ItemsWithErrors)
                    {
                        _logger.LogError("Failed to index property document {0}: {1}", entryWithError.Id, entryWithError.Error);
                    }
                }
                response = new ElasticPostResponse
                {
                    responseDescription = result.ToString(),
                    responseCode = 0
                };
            }
            catch (Exception ex)
            {
                response = new ElasticPostResponse
                {
                    responseDescription = ex.Message,
                    responseCode = 500
                };
            }
            return response;
        }

        public async Task<ElasticPostResponse> SaveSinglePropertyAsync(Properties props)
        {
            ElasticPostResponse response;
            try
            {
                if (_cache.Any(p => p.property.propertyID == props.property.propertyID))
                {
                    await _elasticClient.UpdateAsync<Properties>(props, u => u.Doc(props));
                    response = new ElasticPostResponse
                    {
                        responseDescription = "Record Exists. Update performed",
                        responseCode = 0
                    };
                }
                else
                {
                    _cache.Add(props);
                    var resp = await _elasticClient.IndexDocumentAsync<Properties>(props);
                    response = new ElasticPostResponse
                    {
                        responseDescription = resp.Id,
                        responseCode = 0
                    };
                }

            }
            catch (Exception ex)
            {
                response = new ElasticPostResponse
                {
                    responseDescription = ex.Message,
                    responseCode = 500
                };
            }
            return response;
        }

        public async Task<List<Properties>> SearchKeyProperties(string keyword)
        {
            var result = await _elasticClient.SearchAsync<Properties>(s => s.Source()  
                                .Query(q => q  
                                .QueryString(qs => qs  
                                .AnalyzeWildcard()  
                                   .Query("*" + keyword + "*")  
                                   .Fields(fs => fs  
                                       .Fields(f1 => f1.property.market,  
                                               f2 => f2.property.name,  
                                               f3 => f3.property.formerName,  
                                               f4 => f4.property.streetAddress,
                                               f5 => f5.property.city,  
                                               f6 => f6.property.state    
                                               )  
                                   )  
                                   ))); 

            if (!result.IsValid)
            {
                //Request notnvalid due to an error
                _logger.LogError("Unable to carry out Elastic Search");
                return new List<Properties>();
            }
            return result.Documents.Where(x=>x.property!=null).ToList();
        }
    }
}
