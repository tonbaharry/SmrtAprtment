using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;

namespace SmartApartmentData.Services
{
    public class DotNetService
    {
        public class ElasticSearchDotNetService : IDotNetService
        {
            private List<DotNet> _cache = new List<DotNet>();

            private readonly IElasticClient _elasticClient;
            private readonly ILogger _logger;

            public ElasticSearchDotNetService(IElasticClient elasticClient, ILogger<ElasticSearchDotNetService> logger)
            {
                _elasticClient = elasticClient;
                _logger = logger;
            }

            
            public virtual Task<IEnumerable<DotNet>> GetProducts(int count, int skip = 0)
            {
                var products = _cache
                    .Skip(skip)
                    .Take(count);

                return Task.FromResult(products);
            }
            

            public virtual Task<DotNet> GetProductById(string id)
            {
                var product = _cache
                .FirstOrDefault(p => p.index._id == id);

                return Task.FromResult(product);
            }
            

            public async Task DeleteAsync(DotNet product)
            {
                await _elasticClient.DeleteAsync<DotNet>(product);

                if (_cache.Contains(product))
                {
                    _cache.Remove(product);
                }
            }

            public async Task SaveSingleAsync(DotNet product)
            {
                if (_cache.Any(p => p.index._id == product.index._id))
                {
                    await _elasticClient.UpdateAsync<DotNet>(product, u => u.Doc(product));
                }
                else
                {
                    _cache.Add(product);
                    await _elasticClient.IndexDocumentAsync<DotNet>(product);
                }
            }

            public async Task SaveManyAsync(DotNet[] products)
            {
                _cache.AddRange(products);
                var result = await _elasticClient.IndexManyAsync(products);
                if (result.Errors)
                {
                    // the response can be inspected for errors
                    foreach (var itemWithError in result.ItemsWithErrors)
                    {
                        _logger.LogError("Failed to index document {0}: {1}",
                            itemWithError.Id, itemWithError.Error);
                    }
                }
            }

            public async Task SaveBulkAsync(DotNet[] products)
            {
                _cache.AddRange(products);
                var result = await _elasticClient.BulkAsync(b => b.Index("products").IndexMany(products));
                if (result.Errors)
                {
                    // the response can be inspected for errors
                    foreach (var itemWithError in result.ItemsWithErrors)
                    {
                        _logger.LogError("Failed to index document {0}: {1}",
                            itemWithError.Id, itemWithError.Error);
                    }
                }
            }
        }
    }
}