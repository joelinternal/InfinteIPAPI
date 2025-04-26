using InfiniteIP.Models;
using InfiniteIP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfiniteIP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IGmSheet _service;
        public AccountController(IGmSheet service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _service.GetAccounts();
            return Ok(accounts);
        }
        [HttpGet("Projects/{AccountId:int}")]
        public async Task<IActionResult> GetProjects(int AccountId)
        {
            var projects = await _service.GetProjects(AccountId);
            return Ok(projects);
        }

        [HttpPost("Sow")]
        public async Task<IActionResult> CreateSow([FromBody] Sowparams sowparams)
        {        
          

            var res = await _service.CreateSow(sowparams);
            return Ok(res);
        }


        [HttpGet("Sow/{AccountId:int}/{ProjectId:int}")]
        public async Task<IActionResult> GetSow(int AccountId,int ProjectId)
        {
            var sow = await _service.GetSow(AccountId, ProjectId);
            return Ok(sow);
        }
    }
}
