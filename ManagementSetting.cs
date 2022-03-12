using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartApartmentData
{
    public class ManagementSetting
    {
        public string Name { get; set; } = "Housing Management Elastic search";
        public string Description { get; set; } = "A short description of the application";
        public string Owner { get; set; } = "Tubotonba Harry";
        public int ProductsPerPage { get; set; } = 100;
    }
}
