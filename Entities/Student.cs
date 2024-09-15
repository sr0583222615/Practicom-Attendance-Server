using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Student
    {
        public Student()
        {
            TempCodes = new HashSet<TempCode>();
        }

        public int Id { get; set; }
        public int IdentityNumber { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? PracticomTypeId { get; set; }

        public virtual CdPracticomType? PracticomType { get; set; }
        public virtual ICollection<TempCode> TempCodes { get; set; }
    }
}
