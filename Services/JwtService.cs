using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace LibraryApplication.Services
{
    public class JwtService
    {
        private string secureKey = "Q&]W>Sc/)9H^XTsf";

        public string generageToken(long userId)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtHeader header = new JwtHeader(credentials);
            JwtPayload payload = new JwtPayload(userId.ToString(), null, null, null, DateTime.Today.AddDays(1));
            JwtSecurityToken securityToken = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        private JwtSecurityToken verifyToken(string jwtTooken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(secureKey);

            tokenHandler.ValidateToken(jwtTooken, new TokenValidationParameters() {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken) validatedToken;
        }

        public bool checkAuthorization(HttpRequest request)
        {
            try
            {
                var jwtToken = request.Cookies["jwt"];
                JwtSecurityToken token = verifyToken(jwtToken);
                long.Parse(token.Issuer);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
