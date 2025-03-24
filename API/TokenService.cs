using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using static System.Net.WebRequestMethods;

namespace API
{

    public class TokenService
    {
        public string ExtraiDadosDoToken(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                Console.WriteLine("Cabeçalho Authorization ausente ou inválido.");
                return null; // Ou lance uma exceção personalizada
            }

            string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            // Pegue o "username" do token, geralmente no campo "sub"
            string username = token.Claims.FirstOrDefault(c => c.Type == "username" || c.Type == "sub")?.Value;

            if (!string.IsNullOrEmpty(username))
            {
                Console.WriteLine($"Username extraído: {username}");
            }
            else
            {
                Console.WriteLine("Username não encontrado no token.");
            }

            return username;
        }
 


    public bool ValidaToken(string jwtToken)
        {
            var validationParameters = GetTokenValidationParameters();
            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(jwtToken, validationParameters, out var validatedToken);
                return true; // Token válido
            }
            catch
            {
                return false; // Token inválido
            }
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new JsonWebKey("https://cognito-idp.us-east-1.amazonaws.com/us-east-1_UZ3o5yiyF/.well-known/jwks.json"),
                ValidateIssuer = true,
                ValidIssuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_UZ3o5yiyF",
                ValidateAudience = true,
                ValidAudience = "7dga8lvc9054cam96i0cgparfr",
                ValidateLifetime = true
            };
        }
    }

}
