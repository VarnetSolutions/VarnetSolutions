using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VarnetFileService.Entity;


namespace WebApplication3.Auth
{
    public class BasicAuthnicationAttribute : AuthorizationFilterAttribute
    {
         

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
        // AuthenticatedUserEntity authenticatedUserEntity = UserSecurityBO.AuthenticateUserAndGetDetails(authenticationToken);

        // string username = UserValidateToken.OpenToken(authenticationToken);
        string username =UserValidateTokenV1.Open_Tokens(authenticationToken);
                if (username != null)
                {

                    //var identity = new GenericIdentity(authenticatedUserEntity.user_id.ToString());
                    //var principal = new CustomPrincipal(identity, roles);
                    //HttpContext.Current.User = principal;
                    //System.Threading.Thread.CurrentPrincipal = principal;

                    var claims = new List<Claim>
                  {
                      //new Claim(ClaimTypes.NameIdentifier, authenticatedUserEntity.user_id.ToString()),
                     new Claim(ClaimTypes.Name, username),
                      //new Claim(ClaimTypes.Email, authenticatedUserEntity.email.ToString()),
                     //new Claim(ClaimTypes.Role, authenticatedUserEntity..ToString())
                      // Add more claims as needed
                  };

                    // Create a ClaimsIdentity
                    var claimsIdentity = new ClaimsIdentity(claims, "custom");

                    // Create a ClaimsPrincipal
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Set the ClaimsPrincipal to the HttpContext
                    HttpContext.Current.User = claimsPrincipal;

                    // Your API logic here...

                    //int i = 0;
                    //string[] roles = new string[userRoles.Count];
                    //foreach (var item in userRoles)
                    //{
                    //    roles[i] = item.Role.Role_Name;
                    //    i++;
                    //}

                    /*var principal = new GenericPrincipal(new GenericIdentity(username), null);
                    System.Threading.Thread.CurrentPrincipal = principal;
                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.User = principal;
                    }*/
                }
                else
                {
                    HttpResponseEntity responseEntity = new HttpResponseEntity();
                    responseEntity.Message = "Unauthorized user!";
                    responseEntity.Status = HttpStatusCode.Unauthorized;
                    responseEntity.Data = null;


                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized, responseEntity);
                }
            }
        }
    }
}