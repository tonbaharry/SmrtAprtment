using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using SmartApartmentData.Model;

namespace SmartApartmentData.Extension
{
    public static class ElasticSearchExtension
    {
        public static void AddManagementElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetConnectionString("AWSElasticBaseUrl");
            var defaultIndex = configuration["managementindex"];
            var user = "tubotonbaharry";
            var pass = configuration["password"];
  
            var settings = new ConnectionSettings(new Uri(url))
            .DefaultIndex(defaultIndex);
            settings.BasicAuthentication(user, pass);
  
            //AddDefaultManagementMappings(settings);
  
            var client = new ElasticClient(settings);
  
            services.AddSingleton<IElasticClient>(client);
            CreateIndex1(client, defaultIndex); 
        }

        private static void AddDefaultManagementMappings(ConnectionSettings settings)
        {
            settings.DefaultMappingFor<Management>(m => m
                    .Ignore(p => p.mgmt.mgmtID)
                    .Ignore(p => p.mgmt.name)
                    .Ignore(p => p.mgmt.market)
                    .Ignore(p => p.mgmt.state)
                );
        }
        

        private static void CreateIndex1(IElasticClient client, string indexName)
        {
            
            var createIndexResponse2 = client.Indices.Create(indexName,
                index => index.Map<Management>(x => x.AutoMap())
            );
        }

        private static void CreateIndex3(IElasticClient client, string indexName)
        {
            var createIndexResponse2 = client.Indices.Create(indexName,
                index => index.Map<Properties>(x => x.AutoMap())
            );
        }
    }
}