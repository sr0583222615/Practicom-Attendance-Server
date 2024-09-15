
namespace Service
{
    public interface IAttendanceService
    {
        Task<string> CheckInAsync(int temporaryCode);
        Task<string?> CheckinHome(int identityNumber);
        Task<string> CheckOutAsync(int tz);
        Task<string?> GetUserNameAsync();
        Task<string?> SubscribeAsync(int identityNumber);
    }
}