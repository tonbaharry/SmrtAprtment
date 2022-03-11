using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace SmartApartmentData.Extension
{
    public static class ElasticSearchExtension
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetConnectionString("AWSElasticBaseUrl");
            var defaultIndex = configuration["index1"];
  
            var settings = new ConnectionSettings(new Uri(url))
            .DefaultIndex(defaultIndex);
  
            AddDefaultMappings(settings);
  
            var client = new ElasticClient(settings);
  
            services.AddSingleton(client);
  
            CreateIndex(client, defaultIndex);
        }
  
        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings.DefaultMappingFor<DotNet>(m => m
                    .Ignore(p => p.index._id)
                    .Ignore(p => p.index._index)
                    .Ignore(p => p.index._type)
                );
        }
  
        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                index => index.Map<DotNet>(x => x.AutoMap())
            );
        }
    }
}