using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using SmartApartmentData.Interfaces;
using SmartApartmentData.Model;
using SmartApartmentData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace SmartApartmentData.Controllers
{
    [Authorize]
    public class ManagementController : Controller
    {
        private IManagementService _service;
        private ITokenService _tokenService;
        private IMapper _mapper;

        public ManagementController(IManagementService service, ITokenService tokenService, IMapper mapper)
        {
            _service = service;
            _tokenService = tokenService;
            _mapper = mapper;
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
            var serviceresponse = await _service.SearchKeyManagement(query);

            return Ok(serviceresponse);
        }
    }
}
