using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class TempCode
    {
        public int Code { get; set; }
        public int StudentId { get; set; }
        public DateTime DateAndTime { get; set; }

        public virtual Student Student { get; set; } = null!;
    }
}
