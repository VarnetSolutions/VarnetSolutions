using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;

namespace WebApplication3.Auth
{
    public class UserValidateTokenV1
    {
        
        public static string Open_Tokens(string token)
        {

            try
            {
                var issuer = "https://www.varnetsolutions.com/";
                var audience = "sandeshvahak_broadcast_message";
                var key = "ABCDeujujsik@!!!Ashsnskajuhaaaaa";
           

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var tokenHandler = new JwtSecurityTokenHandler();



                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = securityKey,
                   
                };


                // Decode the JWT token
                //var jwtToken = tokenHandler.ReadJwtToken(token);
               // Console.WriteLine(jwtToken);

                // Validate the token
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                Console.WriteLine(principal);
                // Log validated token details for debugging
                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken != null)
                {
                    Console.WriteLine("Issuer: " + jwtToken.Issuer);
                    Console.WriteLine("Audience: " + jwtToken.Audiences.FirstOrDefault());
                    Console.WriteLine("Claims:");
                    Console.WriteLine("ApplicationId" + jwtToken.Claims.FirstOrDefault(c => c.Type == "applicationId"));
                    foreach (var claim in jwtToken.Claims)
                    {
                        Console.WriteLine($"  {claim.Type}: {claim.Value}");
                    }
                }


                // Set the principal for the current request
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }

                // Extract the application ID and scopes claim from the token

                var applicationIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "ApplicationCode")?.Value;
                var scopesClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "scope")?.Value;

                Console.WriteLine(applicationIdClaim);

                if (scopesClaim == "get_chat_first_message")
                {
                    return "valid User ";
                }
                else
                {
                    return "Invalid access Permission";
                }


                // Check if the required scope is present in the token's scopes
                /*if (!string.IsNullOrEmpty(scopesClaim) && scopesClaim.Split(' ').Contains("verify user"))
                {
                    return $"Token is valid. Application ID: {applicationIdClaim}. Scopes: {scopesClaim}.";
                }*/

                //return $"Token is valid but does not have the required scope. Application ID: {applicationIdClaim}. Scopes: {scopesClaim}.";
            }
            catch (SecurityTokenException)
            {
                // Token validation failed
                return "Token validation failed.";
            }
            catch (Exception ex)
            {
                // Other exceptions
                return $"An error occurred: {ex.Message}";
            }
        }

        internal static (bool isValid, object message, object applicationId) Open_Tokens(string token, string requiredScope)
        {
            throw new NotImplementedException();
        }

       
    }

    
}
