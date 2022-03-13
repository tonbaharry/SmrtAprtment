using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using SmartApartmentData.Model;
using SmartApartmentData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartApartmentData.Controllers
{
    public class ManagementController : Controller
    {
        private IManagementService _service;

        public ManagementController(IManagementService service)
        {
            _service = service;
        }
        
        [HttpPost("InsertSingleManagementRecord")]
        public async Task<IActionResult> InsertSingleManagementRecord(Management entry)
        {
            var existing = await _service.GetManagementById(entry.mgmt.mgmtID);

            if (existing == null)
            {
                ElasticPostResponse response =  await _service.SaveSingleManagementAsync(entry);
                return Ok(response);
            }

            return NotFound();
        }

        [HttpPost("InsertMultipleManagementRecord")]
        public async Task<IActionResult> InsertMultipleManagementRecord([FromBody] List<Management> entry)
        {
            ElasticPostResponse response = await _service.SaveManyManagementAsync(entry);
            return Ok(response);
        }

        [HttpGet("SearchManagement")]
        public async Task<IActionResult> SearchManagement(string query = "")
        {
            var response = await _service.SearchKeyManagement(query);

            return Ok(response);
        }
    }
}
