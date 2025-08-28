using System;
using Microsoft.AspNetCore.DataProtection;

namespace ECommerce.Business.DataProtection
{
    public class DataProtection : IDataProtection
    {
        private readonly IDataProtector _protector;

        public DataProtection(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("ECommercePlatform.PasswordProtector");
        }

        public string Protect(string text) => _protector.Protect(text);
        public string UnProtect(string protectedText) => _protector.Unprotect(protectedText);
    }

}

