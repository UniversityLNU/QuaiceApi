using EduRateApi.Dtos;
using EduRateApi.Dtos.FundraisingDTO;
using EduRateApi.Interfaces;
using EduRateApi.Models;
using FireSharp.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace EduRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundraisingController : ControllerBase
    {
        private readonly IFundraisingService _fundraisingService;
        public FundraisingController(IFundraisingService fundraisingService)
        {
            _fundraisingService = fundraisingService;
        }

        [HttpPost("UploadFundrasing")]
        public async Task<ActionResult<ServerResponse>> UploadFundrasing([FromBody] FundraisingDto fundraisingDto)
        {
            var response = await _fundraisingService.UploadFundrasing(fundraisingDto);
            return StatusCode((int)response.statusCode, response);
        }


        [HttpGet("GetFundraisingById/{fundraisingId}")]
        public async Task<ActionResult<Fundraising>> GetFundraisingById(string fundraisingId)
        {
            var response = await _fundraisingService.GetFundraisingById(fundraisingId);
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet("GetAllFundraisings")]
        public async Task<ActionResult<List<Fundraising>>> GetAllFundraisings()
        {
            var response = await _fundraisingService.GetAllApprovedFundraisings();
            return StatusCode((int)response.statusCode, response);
        }

        [HttpGet("GetUnapprovedFundraisings")]
        public async Task<ActionResult<List<Fundraising>>> GetUnapprovedFundraisings()
        {
            var response = await _fundraisingService.GetUnapprovedFundraisings();
            return StatusCode((int)response.statusCode, response);
        }


        [HttpPut("ApproveFundraising/{fundraisingId}")]
        public async Task<ActionResult> ApproveFundraising(string fundraisingId)
        {
            var response = await _fundraisingService.ApproveFundraising(fundraisingId);
            return StatusCode((int)response.statusCode, response);
        }
    }
}
