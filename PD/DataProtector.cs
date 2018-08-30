using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD
{
    public class DataProtector
    {
        private IDataProtector _protector;
        public DataProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

        public string Encrypt(string value)
        {
            return _protector.Protect(value);
        }

        public string Decrypt(string value)
        {
            return _protector.Unprotect(value);
        }
    }
}
