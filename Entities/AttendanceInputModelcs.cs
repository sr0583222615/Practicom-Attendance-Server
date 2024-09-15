namespace PracticomAttendance.Helpers
{
    public class AttendanceInputModel
    {
        public int StudentId { get; set; }
        public string  Date { get; set; }
        public string? EntryTime { get; set; }
        public string? ExitTime { get; set; }
    }
}