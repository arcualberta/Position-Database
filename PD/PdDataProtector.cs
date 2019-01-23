using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD
{
    public class PdDataProtector : IPdDataProtector
    {
        private IDataProtector _protector;
        public PdDataProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

        //public string Encrypt(string value)
        //{
        //    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
        //    byte[] result = _protector.Protect(bytes);

        //    return Convert.ToBase64String(result);
        //}

        //public string Decrypt(string value)
        //{
        //    byte[] bytes = Convert.FromBase64String(value);
        //    byte[] result = _protector.Unprotect(bytes);

        //    return System.Text.Encoding.UTF8.GetString(result);
        //}

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
