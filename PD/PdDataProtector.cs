using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //////Reference: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/dangerous-unprotect?view=aspnetcore-2.2
            ////var persistedProtector = _protector as IPersistedDataProtector;
            ////string protectedValue = persistedProtector.Protect(value);
            ////return protectedValue;
            ///
            return _protector.Protect(value);
        }

        public string Decrypt(string value)
        {
            //////Reference: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/dangerous-unprotect?view=aspnetcore-2.2
            ////var persistedProtector = _protector as IPersistedDataProtector;
            ////byte[] protectedBytes = Encoding.ASCII.GetBytes(value);
            ////bool requiresMigration, wasRevoked;
            ////var unprotectedBytes = persistedProtector.DangerousUnprotect(
            ////    protectedData: protectedBytes,
            ////    ignoreRevocationErrors: true,
            ////    requiresMigration: out requiresMigration,
            ////    wasRevoked: out wasRevoked);

            ////string unprotectedData = Encoding.ASCII.GetString(unprotectedBytes);
            ////return unprotectedData;

            return _protector.Unprotect(value);

        }

    }
}
