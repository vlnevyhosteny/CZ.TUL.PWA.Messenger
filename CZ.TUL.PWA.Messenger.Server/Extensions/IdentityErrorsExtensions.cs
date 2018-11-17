using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;
namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class IdentityErrorsExtensions
    {
        public static string ToResponseString(this IEnumerable<IdentityError> enumerable) 
        {
            return string.Join(',', enumerable.Select(x => x.ToResponseString()));
        }

        public static string ToResponseString(this IdentityError error) 
        {
            return $"{error.Description} [{error.Code}]";
        }
    }
}
