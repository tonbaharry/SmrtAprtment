using SmartApartmentData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartApartmentData.Services
{
    public interface IPropertyService
    {
        public Task<IEnumerable<Properties>> GetPropertyListCount(int count, int skip = 0);
        public Task<Properties> GetPropertyById(int id);
        public Task DeletePropertyAsync(Properties prop);
        public Task<ElasticPostResponse> SaveSinglePropertyAsync(Properties prop);
        public Task<ElasticPostResponse> SaveManyPropertiesAsync(List<Properties> props);
        public Task<List<Properties>> SearchKeyProperties(string keyword);
    }
}
