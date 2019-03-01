using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD
{
    public interface IPdDataProtector
    {
        string Encrypt(string value);

        string Decrypt(string value);

        string Hash(string key);
    }
}
