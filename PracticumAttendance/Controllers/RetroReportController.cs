using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using DataRepository;
using Entities;
using Service;

namespace PracticomAttendance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetroReportController : ControllerBase
    {
        private readonly IRetroReportService _retroReporService;

        public RetroReportController(IRetroReportService retroReporService)
        {
            _retroReporService = retroReporService;

        }

        [HttpPost ("retrout")]
        public IActionResult retrout([FromBody] Helpers.AttendanceInputModel AttendanceReport)
        {
            return Ok(_retroReporService.retrout(AttendanceReport).Result);
        }

        [HttpPost ("retroin")]
        public IActionResult retroin([FromBody] Helpers.AttendanceInputModel AttendanceRetroin)

        {
            return Ok(_retroReporService.retroin(AttendanceRetroin).Result);

        }

        [HttpPost("retroall")]
        public async Task<IActionResult> retroall([FromBody] Helpers.AttendanceInputModel AttendanceRetroin)
        {
            return Ok(_retroReporService.retroall(AttendanceRetroin).Result);
        }
    }
}





