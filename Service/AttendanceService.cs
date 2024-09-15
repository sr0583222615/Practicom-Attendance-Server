using System.Threading.Tasks;
using DataRepository;


namespace Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public Task<string?> SubscribeAsync(int identityNumber)
        {
            return _attendanceRepository.SubscribeAsync(identityNumber);
        }
        public Task<string?> CheckinHome(int identityNumber)
        {
            return _attendanceRepository.CheckinHome(identityNumber);
        }

        public Task<string> CheckInAsync(int temporaryCode)
        {
            return _attendanceRepository.CheckinAsync(temporaryCode);
        }

        public Task<string> CheckOutAsync(int tz)
        {
            return _attendanceRepository.CheckOutAsync(tz);
        }

        public async Task<string?> GetUserNameAsync()
        {
            return await _attendanceRepository.GetUserNameAsync();
        }

    }
}
