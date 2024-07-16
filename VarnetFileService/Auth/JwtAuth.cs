using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VarnetFileService.Entity;

namespace VarnetFileService.Auth
{
    public class JwtAuth : AuthorizationFilterAttribute
    {
        private string permission_nm = string.Empty;
        public JwtAuth(string permission_nm)
        {
            permission_nm = permission_nm.ToLower();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
        HttpResponseEntity httpResponseEntity = new HttpResponseEntity();

            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.
                    CreateResponse(HttpStatusCode.Unauthorized);
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
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                        };

                        // Validate the token
                        string token = authenticationToken;

                        var DecodedToken = tokenHandler.ReadJwtToken(token);


                        SecurityToken validatedToken;
                        /*var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                        string username = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub").Value;

                        List<Claim> claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.Name, UserBO.GetUserByUserName(username).ToString()));



                        // Create a ClaimsIdentity
                        var claimsIdentity = new ClaimsIdentity(claims, "custom");

                        // Create a ClaimsPrincipal
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Set the ClaimsPrincipal to the HttpContext
                        HttpContext.Current.User = claimsPrincipal;

                        // Set the principal for the current request
                        //Thread.CurrentPrincipal = principal;
                        //HttpContext.Current.User = principal;

                        */

                        var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                        /* var applicationCodeClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "ApplicationCode");
                         string[] permissions = applicationCodeClaim?.Value?.Split(',');

                         if (permissions == null || permissions.Count(permission =>
                             permission.Equals("7044441E-EA30-4337-AE0C-B01C9367FB74", StringComparison.OrdinalIgnoreCase)) != 1)
                         {
                             actionContext.Response = actionContext.Request
                                 .CreateResponse(HttpStatusCode.Unauthorized, "Unknown user, incorrect appcode!");
                         }*/

                        /*var permissions = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "ApplicationCode")?.Value.Split(',');
                        int count = 0;
                        foreach (string permission in permissions)
                        {
                            if (permission.Equals("7044441E-EA30-4337-AE0C-B01C9367FB74"))
                            {
                               count++;
                            }
                        }
                        if (count !=1)
                        {
                            actionContext.Response = actionContext.Request
                           .CreateResponse(HttpStatusCode.Unauthorized, "Unknown user, incorrect appcode!");
                        }*/

                        if (!principal.Claims.Where(x => x.Type == "ApplicationCode").First().Value.ToUpper().Contains("C4E6BA84-5F91-4770-86BC-DA49C9C6D7A9"))
                        {
                            actionContext.Response = actionContext.Request
                            .CreateResponse(HttpStatusCode.Unauthorized, "Unknown user, incorrect appcode!");

                        }
                        else
                        {
                            if (!principal.Claims.Where(x => x.Type == "scope").First().Value.ToUpper().Contains(permission_nm))
                            {
                                actionContext.Response = actionContext.Request
                                .CreateResponse(HttpStatusCode.Unauthorized, "Insufficient priviledges!");
                            }
                            else
                            {
                                // Set the principal for the current request
                                Thread.CurrentPrincipal = principal;
                                HttpContext.Current.User = principal;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // Token validation failed, return unauthorized
                        //actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        //return;
                        /*HttpResponseEntity responseEntity = new HttpResponseEntity();
                        responseEntity.Message = "Unauthorized user!";
                        responseEntity.Status = HttpStatusCode.Unauthorized;
                        responseEntity.Data = ex.Message;
                        */

                        actionContext.Response = actionContext.Request
                            .CreateResponse(HttpStatusCode.Unauthorized, ex.Message);
                    }
                }
                else
                {
                    /*HttpResponseEntity responseEntity = new HttpResponseEntity();
                    responseEntity.Message = "Unauthorized user!";
                    responseEntity.Status = HttpStatusCode.Unauthorized;
                    responseEntity.Data = null;
                    */

                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized, "Auth token is missing!");
                }
            }


        }


    }
}


