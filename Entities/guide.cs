using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class guide
    {

        public int Id { get; set; }
        public int Identity_number { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
    }
}
