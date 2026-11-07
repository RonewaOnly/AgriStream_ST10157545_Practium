using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AgriStream_ST10157545_Practium.API
{
    public static class ApiKeyGuard
    {
        private const string HeaderName = "x-api-key";

        public static bool IsValid(HttpRequest req, string? expected)
        {
            if (string.IsNullOrEmpty(expected) ||
                !req.Headers.TryGetValue(HeaderName, out var supplied))
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(supplied.ToString()),
                Encoding.UTF8.GetBytes(expected));
        }
    }
}
