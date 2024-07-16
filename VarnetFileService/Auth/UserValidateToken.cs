using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;




namespace WebApplication3.Auth
{
    public class UserValidateToken
    {
        public static string OpenToken(string token)
        {
            try
            {
               var issuer = " https://wp.sandeshvahak.co.in"; // Replace with your application's URL
               var audience = "sandeshvahakchathubuser"; // Replace with your application's URL
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
                //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhcGlfdXNlciIsImF1ZCI6WyJzYW5kZXNodmFoYWtfYnJvYWRjYXN0X21lc3NhZ2UiLCJzYW5kZXNodmFoYWtjaGF0aHVidXNlciJdLCJpYXQiOiIxNzEyNTY0NTExIiwibmJmIjoxNzEyNTY0NTExLCJleHAiOjE3MTI1NjgxMTEsImlzcyI6Imh0dHBzOi8vd3d3LnZhcm5ldHNvbHV0aW9ucy5jb20vIn0.ylb4wO281QX2lhNrI_n2MSIE6xlAgvkOvSLY6iW2Uww";
                //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJwaXl1c2giLCJhdWQiOlsiQWRkVXNlclRvQ2hhdCIsInNhbmRlc2h2YWhha19icm9hZGNhc3RfbWVzc2FnZSJdLCJhenAiOiIxIiwiaWF0IjoiMTcxNDEyODE0NyIsInNjb3BlIjoiZ2V0X2NoYXRfZmlyc3RfbWVzc2FnZSIsIm5iZiI6MTcxNDEyODE0NywiZXhwIjoxNzE0MTMxNzQ3LCJpc3MiOiJodHRwczovL3d3dy52YXJuZXRzb2x1dGlvbnMuY29tLyJ9.Ojrn1QetC6chbEgap8v2t375znV1QWPzUTiH7pZ-C30";
                var DecodedToken = tokenHandler.ReadJwtToken(token);
                var username = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub").Value;

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Set the principal for the current request
                //Thread.CurrentPrincipal = principal;
                //HttpContext.Current.User = principal;

                return username;
            }
            catch (SecurityTokenException ex)
            {
                // Token validation failed, return unauthorized
                //actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                //return;
                return null;
            }
        }
    }
}