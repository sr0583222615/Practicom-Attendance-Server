using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? EntryTime { get; set; }
        public TimeSpan? ExitTime { get; set; }
        public TimeSpan? DurationOfAttendance { get; set; }
    }
}
