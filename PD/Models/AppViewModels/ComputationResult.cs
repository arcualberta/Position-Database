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

        public int SuccessCount { get; set; } = 0;
        public int ErrorCount { get; set; } = 0;

        public object Data { get; set; }
    }
}
