using Microsoft.AspNetCore.Mvc;
using SmartApartmentData.Model;
using SmartApartmentData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartApartmentData.Controllers
{
    public class PropertyController : Controller
    {
        private IPropertyService _service;
        public PropertyController(IPropertyService service)
        {
            _service = service;
        }

        [HttpPost("InsertSinglePropertyRecord")]
        public async Task<IActionResult> InsertSinglePropertyRecord(Properties entry)
        {
            var existing = await _service.GetPropertyById(entry.property.propertyID);

            if (existing == null)
            {
                ElasticPostResponse response = await _service.SaveSinglePropertyAsync(entry);
                return Ok(response);
            }

            return NotFound();
        }

        [HttpPost("InsertMultiplePropertiesRecord")]
        public async Task<IActionResult> InsertMultiplePropertiesRecord([FromBody] List<Properties> entry)
        {
            ElasticPostResponse response = await _service.SaveManyPropertiesAsync(entry);
            return Ok(response);
        }

        [HttpGet("SearchProperty")]
        public async Task<IActionResult> SearchManagement(string query = "")
        {
            var response = await _service.SearchKeyProperties(query);

            return Ok(response);
        }
    }
}
