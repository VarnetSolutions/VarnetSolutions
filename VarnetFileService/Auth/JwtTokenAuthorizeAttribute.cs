using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using VarnetFileService.Entity;

namespace VarnetFileService.Auth
{
    public class JwtTokenAuthorizeAttribute : AuthorizeAttribute
    {
        private DateTime expUtc;

        public override void OnAuthorization(HttpActionContext actionContext)
        {

            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string authenticationToken = actionContext.Request.Headers
                                            .Authorization.Parameter;

                /* string decodedAuthenticationToken = Encoding.UTF8.GetString(
                     Convert.FromBase64String(authenticationToken));
                 string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                 string username = usernamePasswordArray[0];
                 string token = usernamePasswordArray[1];
                */
                //var userRoles = UserSecurity.AuthenticateUser(username, password);
                //AuthenticatedUserEntity authenticatedUserEntity = UserSecurityBO.AuthenticateUserAndGetDetails(authenticationToken);
                if (authenticationToken != null && authenticationToken.Trim() != "")
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
                            IssuerSigningKey = securityKey,
                            ValidateIssuerSigningKey = true
                        };

                        // Validate the token
                        string token = authenticationToken;
  
                        var DecodedToken = tokenHandler.ReadJwtToken(token);
                        SecurityToken validatedToken;
                        var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                        var expiryClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "exp");
                        if (expiryClaim == null)
                        {
                            throw new Exception("Token does not have an expiry time.");
                        }

                        long expiryTimeUnix = long.Parse(expiryClaim.Value);
                        DateTime expiryTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expiryTimeUnix).UtcDateTime;

                        TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        DateTime expiryTimeIst = TimeZoneInfo.ConvertTimeFromUtc(expiryTimeUtc, indianZone);

                        DateTime currentTimeIst = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianZone);
                        if (currentTimeIst > expiryTimeIst)
                        {
                            throw new Exception("Token has expired.");
                        }



                        string user_id = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "ApplicationCode").Value;
                        string username = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub").Value;

                        //string permissions = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "scope").Value;                       
                        var permissions = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "scope")?.Value.Split(',');

                        // UserDtetails authenticatedUserEntity = AuthDAO.GetUserByToken(user_id);
                        if (authenticationToken != null)
                             {
                     //       var requiredPermissions = new[] { "required_permission_1", "required_permission_2" }; // Define required permissions

                                   if (permissions != null )
                                     {
                                      var claims = new List<Claim>
                                       {
                                         new Claim(ClaimTypes.NameIdentifier, user_id),
                                         new Claim(ClaimTypes.Name, username),
                                         new Claim("permissions", string.Join(",", permissions))
                                       };

                                      var claimsIdentity = new ClaimsIdentity(claims, "custom");
                                      var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                                      HttpContext.Current.User = claimsPrincipal;
                                    }
                                   else
                                    {
                                        throw new Exception("User does not have the required permission.");
                                    }
                         }
                         else
                        {
                         throw new Exception("Unauthorized user!");
                        }

                        /*UserDtetails authenticatedUserEntity = AuthDAO.GetUserByToken(user_id);
                        if (authenticatedUserEntity != null)
                        {
                            if (permissions != null && _permissions.Any(perm => permissions.Contains(perm)))
                            {
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user_id),
                                    new Claim(ClaimTypes.Name, username),
                                   //new Claim("permissions", permissions)
                                     new Claim("permissions", string.Join(",", permissions))
                                };

                                // Create a ClaimsIdentity
                                var claimsIdentity = new ClaimsIdentity(claims, "custom");

                                // Create a ClaimsPrincipal
                                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                // Set the ClaimsPrincipal to the HttpContext
                                HttpContext.Current.User = claimsPrincipal;
                            }
                            else
                            {
                                throw new Exception("User does not have the required permission.");
                            }

                        }
                        else
                        {
                            throw new Exception("Unauthorized user!");
                        }*/
                    }
                    catch (Exception ex)
                    {
                        // Token validation failed, return unauthorized
                        //actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        //return;
                        HttpResponseEntity responseEntity = new HttpResponseEntity();
                        responseEntity.Message = ex.Message;
                        responseEntity.Status = HttpStatusCode.Unauthorized;
                        responseEntity.Data = null;
                        actionContext.Response = actionContext.Request
                            .CreateResponse(HttpStatusCode.OK, responseEntity);
                    }
                }
                else
                {
                    HttpResponseEntity responseEntity = new HttpResponseEntity();
                    responseEntity.Message = "Unauthorized user!";
                    responseEntity.Status = HttpStatusCode.Unauthorized;
                    responseEntity.Data = null;
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.OK, responseEntity);
                }
            }
        }


        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
        {
            if (expires.HasValue)
            {
                var expiresUtc = Convert.ToDateTime(expires.Value);

                // Convert token's expiration time from CST to IST
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                DateTime cstExpiration = TimeZoneInfo.ConvertTimeFromUtc(expires.Value, cstZone);
                DateTime istExpiration = TimeZoneInfo.ConvertTime(cstExpiration, istZone);

                // Log the token's expiration time in IST
                Console.WriteLine($"Token Expiration Time (IST): {istExpiration}");

                // Log the current UTC time
                DateTime utcNow = DateTime.UtcNow;
                Console.WriteLine($"Current UTC Time: {utcNow}");

                // Check if the token has expired
                bool isExpired = istExpiration <= utcNow; // Compare with current UTC time
                Console.WriteLine($"Token Expired: {isExpired}");

                return !isExpired; // Return true if token has not expired
            }

            return true; // Token has no expiration time, treat as valid
        }
    }
}
