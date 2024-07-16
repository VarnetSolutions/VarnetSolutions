using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace VarnetFileServiceUnitTest
{
    [TestClass]
    public class TokenValidationTests
    {
        [TestMethod]
        public void ValidateToken_ValidToken_ReturnsUsername()
        {
            try
            {
                var issuer = "https://www.varnetsolutions.com/"; // Replace with your application's URL
                var audience = "sandeshvahak_broadcast_message"; // Replace with your application's URL
                var key = "ABCDeujujsik@!!!Ashsnskajuhaaaaa"; // Replace with your secret key

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };

                string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJwaXl1c2giLCJhdWQiOlsiQWRkVXNlclRvQ2hhdCIsInNhbmRlc2h2YWhha19icm9hZGNhc3RfbWVzc2FnZSJdLCJhenAiOiIxIiwiaWF0IjoiMTcxNDYzMjg4NCIsIlNlcnZpY2VDb2RlIjoiZGU4MTgyODMtZTAzMy00OGQ1LWEzN2ItOGVmNGQ4NWY2N2IyIiwiQXBwbGljYXRpb25Db2RlIjoiODI0MWJjNzAtMjVkNy00NjJlLTkzOTUtNTJiYTdjMjVlZWIxIiwiVGVuYW50Q29kZSI6IjBlYmZjM2VlLTE5NGQtNDdlZC04ZGVmLWQ5MDFmZjAxNTIwYiIsInNjb3BlIjoiZ2V0X2NoYXRfZmlyc3RfbWVzc2FnZSIsIm5iZiI6MTcxNDYzMjg4NCwiZXhwIjoxNzE0NjM2NDg0LCJpc3MiOiJodHRwczovL3d3dy52YXJuZXRzb2x1dGlvbnMuY29tLyJ9.qGERSbI9zssvFzlmAKtdlbrxHiRLQ1qLCdNdFhNWE0Y";
                var DecodedToken = tokenHandler.ReadJwtToken(token);
                var usernameClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub");
                var username = usernameClaim?.Value;

                var applicationCodeClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "ApplicationCode");
                var applicationCode = applicationCodeClaim?.Value;
                var tenantCodeClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "TenantCode");
                var tenantCode = tenantCodeClaim?.Value;
                var userIdClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "azp");
                var userId = userIdClaim?.Value;
                

                Assert.AreEqual("piyush", username);
                Assert.AreEqual($"{applicationCode}", applicationCode);
                Assert.AreEqual($"{tenantCode}", tenantCode);
                Assert.AreEqual($"{userId}", userId);


                Console.WriteLine("username: " + (username ?? "Not provided"));
                Console.WriteLine("Application Code: " + (applicationCode ?? "Not provided"));
                Console.WriteLine("Tenant Code: " + (tenantCode ?? "Not provided"));
                Console.WriteLine("User ID: " + (userId ?? "Not provided"));

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                if (validatedToken != null)
                {
                    Console.WriteLine("Authorized user");
                }
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine("Token validation failed. Error: " + ex.Message);
               // Console.WriteLine("Unauthorized user");
            }
        }
    }
}
