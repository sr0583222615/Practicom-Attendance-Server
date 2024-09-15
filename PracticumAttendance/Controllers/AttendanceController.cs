using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using DataRepository;
using Entities;
using Service;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticomAttendance.Controllers
{
    [Route("attendance/[action]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {

        private readonly IAttendanceService _attendanceService;

        public AttendanceController( IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService; 

        }

        [HttpGet]
        public string? GetUserName()
        {
            return this.HttpContext.User.Identities.FirstOrDefault()?.Name;
            //WindowsIdentity identity = WindowsIdentity.GetCurrent();
            //if(identity == null)
            //{
            //    return "machine: " + name;
            //}
            //return "windows: " + identity.Name;
        }

        [HttpGet("{identityNumber}")]
        public async Task<ActionResult<string>> Subscribe(int identityNumber)
        {
            return Ok(_attendanceService.SubscribeAsync(identityNumber).Result);
        }

        [HttpGet("{identityNumber}")]
        public async Task<ActionResult<string>> CheckinHome(int identityNumber)
        {
            return Ok(_attendanceService.CheckinHome(identityNumber).Result);
        }


        [HttpGet("{temporaryCode}")]
        public async Task<IActionResult> Checkin(int temporaryCode)
        {
            return Ok(_attendanceService.CheckInAsync(temporaryCode).Result);
        }


        [HttpGet("{tz}")]
        public async Task<IActionResult> Checkout(int tz)
        {
            return Ok(_attendanceService.CheckOutAsync(tz).Result);
        }

    }
}

