using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Localbanda.Models
{
    public class Version
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string IOS_Version { get; set; }
        public string Android_Version { get; set; }
        public string Api_Version { get; set; }

        public DateTime DTS { get; set; }
    }
}
