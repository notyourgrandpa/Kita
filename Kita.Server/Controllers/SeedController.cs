using Kita.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Path = System.IO.Path;
using Kita.Infrastructure.Persistence;
using Kita.Infrastructure.Identity;
using Kita.Application.Services;

namespace Kita.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ISeedService _seedService;

        public SeedController(ISeedService seedService)
        {
            _seedService = seedService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            await using var stream = file.OpenReadStream();
            var seedResult = await _seedService.Import(stream);

            return Ok(seedResult);
        }

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            await _seedService.CreateDefaultUser();
            return Ok();
        }
    }
}
