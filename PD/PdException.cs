using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD
{
    public class PdException : Exception
    {
        public PdException(string message)
            :base(message)
        {
        }
    }
}
