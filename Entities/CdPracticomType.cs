using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class CdPracticomType
    {
        public CdPracticomType()
        {
            Students = new HashSet<Student>();
        }

        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? schedule_gruop_type_id { get; set; }

        public virtual CdScheduleGroupType? ScheduleGroupType { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
