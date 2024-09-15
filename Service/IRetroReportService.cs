using PracticomAttendance.Helpers;

namespace Service
{
    public interface IRetroReportService
    {
        Task<string> retroall(AttendanceInputModel AttendanceReport);
        Task<string> retroin(AttendanceInputModel AttendanceReport);
        Task<string?> retrout(AttendanceInputModel AttendanceReport);
    }
}