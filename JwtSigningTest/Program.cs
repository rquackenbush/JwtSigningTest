using System;

namespace JwtSigningTest
{
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Security;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.IdentityModel.Tokens;

    class Program
    {
        static void Main(string[] args)
        {
            var token = CreateToken();

            ValidateToken(token);
        }

        private static string CreateToken()
        {
            //https://www.codeproject.com/Tips/1208535/Create-And-Consume-JWT-Tokens-in-csharp

            //Load up the pulic / private key
            var bytes = File.ReadAllBytes("JWTTest.pfx");

            X509Certificate2 cert = new X509Certificate2(bytes, new SecureString());

            var key = new X509SecurityKey(cert);

            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha512);

            var header = new JwtHeader(credentials);

            var claims = new Claim[]
            {
                new Claim("scope", "http://acme.com/"),
            };

            DateTime expires = DateTime.Now.Add(TimeSpan.FromDays(100));
            DateTime notBefore = DateTime.Now.Subtract(TimeSpan.FromMinutes(-5));
            DateTime issuedAt = DateTime.Now;

            var payload = new JwtPayload("me", "you", claims, notBefore, expires, issuedAt);

            var secToken = new JwtSecurityToken(header, payload);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(secToken);

            return token;
        }

        private static void ValidateToken(string token)
        {
            //Load up the public key
            var bytes = File.ReadAllBytes("JWTTest.cer");

            X509Certificate2 cert = new X509Certificate2(bytes, new SecureString());

            var key = new X509SecurityKey(cert);

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidAudience = "you",
                ValidIssuer = "me"
            };

            var handler = new JwtSecurityTokenHandler();

            SecurityToken validatedSecurityToken;

            var principal = handler.ValidateToken(token, validationParameters, out validatedSecurityToken);

            if (principal == null)
            {
                Console.WriteLine("no principal");
            }
            else
            {
                Console.WriteLine(principal.GetType().Name);
            }
        }
    }
}
