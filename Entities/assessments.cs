using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class assessments
    {
        public int Id { get; set; }
        public int Assessment_type_id { get; set; }
        public int Student_id { get; set; }
        public DateTime? Date { get; set; }
        public int? Value { get; set; }
    }
}
