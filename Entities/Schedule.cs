using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Schedule
    {
        public int Id { get; set; }
        public int Day { get; set; }
        public TimeSpan FromHour { get; set; }
        public TimeSpan ToHour { get; set; }
        public int? ScheduleGroupTypeId { get; set; }

        public virtual CdScheduleGroupType? ScheduleGroupType { get; set; }
    }
}
