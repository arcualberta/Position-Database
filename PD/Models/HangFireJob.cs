using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class HangFireJob
    {
        [Key]
        public int Id { get; set; }
        public string JobKey { get; set; }
        public string JobId { get; set; }
    }
}
