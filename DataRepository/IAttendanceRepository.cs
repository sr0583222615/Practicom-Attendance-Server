
namespace DataRepository
{
    public interface IAttendanceRepository
    {
        Task<string> CheckinAsync(int temporaryCode);
        Task<string> CheckinHome(int identityNumber);
        Task<string> CheckOutAsync(int tz);
        Task<string?> GetUserNameAsync();
        Task<string?> SubscribeAsync(int identityNumber);
    }
}