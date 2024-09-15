using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class CdScheduleGroupType
    {
        public CdScheduleGroupType()
        {
            CdPracticomTypes = new HashSet<CdPracticomType>();
            Schedules = new HashSet<Schedule>();
        }

        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<CdPracticomType> CdPracticomTypes { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
