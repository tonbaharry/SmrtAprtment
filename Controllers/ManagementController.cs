using Microsoft.AspNetCore.Mvc;
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
                await _service.SaveSingleManagementAsync(entry);
                return Ok();
            }

            return NotFound();
        }
    }
}
