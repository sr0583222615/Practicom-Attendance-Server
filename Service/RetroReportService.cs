using DataRepository;
using PracticomAttendance.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RetroReportService : IRetroReportService
    {
        private readonly IRetroReportRepository _retroReportRepository;

        public RetroReportService(IRetroReportRepository retroReportRepository)
        {
            _retroReportRepository = retroReportRepository;
        }

        public Task<string?> retrout(AttendanceInputModel AttendanceReport)
        {
            return _retroReportRepository.retrout(AttendanceReport);
        }

        public Task<string> retroin(AttendanceInputModel AttendanceReport)
        {
            return _retroReportRepository.retroin(AttendanceReport);
        }

        public Task<string> retroall(AttendanceInputModel AttendanceReport)
        {
            return _retroReportRepository.retroall(AttendanceReport);
        }

    }
}
