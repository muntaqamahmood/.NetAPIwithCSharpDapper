using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DotNetAPI.Helpers
{
    public partial class AuthHelpers
    {
        private readonly IConfiguration _configuration;

        public AuthHelpers(IConfiguration config)
        {
            _configuration = config;
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            // get our PasswordKey from Config
            string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);
            // hashing using above salt
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]{
                new Claim("userId", userId.ToString())
            };
            string? tokenKeyString = _configuration.GetSection("AppSettings:TokenKey").Value;
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : ""
                    )
                );
            // create sign creds to sign tokenKey
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            // create the token with the jwtTokenHandler
            SecurityToken token = tokenHandler.CreateToken(descriptor);
            // returns the created token as a string
            return tokenHandler.WriteToken(token);
        }
    }
}