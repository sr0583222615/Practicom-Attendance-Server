using PracticomAttendance.Helpers;

namespace DataRepository
{
    public interface IRetroReportRepository
    {
        Task<string> retroall(AttendanceInputModel AttendanceRetroin);
        Task<string?> retroin(AttendanceInputModel AttendanceRetroin);
        Task<string?> retrout(AttendanceInputModel AttendanceReport);
    }
}