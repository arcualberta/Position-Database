using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels
{
    public class ComputationResult
    {
        public List<string> Successes { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
