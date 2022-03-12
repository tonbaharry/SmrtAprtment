using System.Collections.Generic;
using System.Threading.Tasks;
using SmartApartmentData.Model;

namespace SmartApartmentData.Services
{
    public interface IManagementService
    {
        public Task<IEnumerable<Management>> GetManagementListCount(int count, int skip = 0);
        public Task<Management> GetManagementById(int id);
        public Task DeleteManagementAsync(Management mgt);
        public Task SaveSingleManagementAsync(Management mgt);
        public Task SaveManyManagementAsync(Management[] mgts);
        public Task SaveBulkAsync(Management[] mgts);
    }
}