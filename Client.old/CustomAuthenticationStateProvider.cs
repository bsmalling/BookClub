using BookClub.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using BookClub.Common;

namespace Client
{

    /// <summary>
    /// This is our custom implementation of an authentication state provider.  The primary purpose of this is 
    /// to implement the overriden method, which is used internally by .NET to determine the current user identity.
    /// </summary>
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {

        private readonly JsonSerializerOptions m_serializerOptions;

        public CustomAuthenticationStateProvider()
        {
            m_serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        /// <summary>
        /// This method should be called upon a successful user login, and it will store the user's JWT token in SecureStorage.
        /// Upon saving this it will also notify .NET that the authentication state has changed which will enable authenticated views
        /// </summary>
        /// <param name="token">Our JWT to store</param>
        /// <returns></returns>
        public async Task Login(string jwtToken)
        {
            //Again, this is what I'm doing, but you could do/store/save anything as part of this process
            await SecureStorage.SetAsync("accounttoken", jwtToken);

            //Providing the current identity ifnormation
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        /// <summary>
        /// This method should be called to log-off the user from the application, which simply removed the stored token and then
        /// notifies of the change
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            try
            {
                var client = new BookClubRestClient();
                var jwtToken = await SecureStorage.GetAsync("accounttoken");
                if (jwtToken != null)
                {
                    string header, payload, signature;
                    if (!JwtUtil.GetParts(jwtToken, out header, out payload, out signature)) return;
                    var user = JsonSerializer.Deserialize<UserWithExpiration>(payload, m_serializerOptions);
                    if (user == null || !await client.Logout(user.Id)) return; // Abort, not authenticated
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logout failed:" + ex.ToString());
            }
            SecureStorage.Remove("accounttoken");
            var newState = new AuthenticationState(new ClaimsPrincipal());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        /// <summary>
        /// This is the key method that is called by .NET to accomplish our goal.
        /// It is the method that is queried to get the current AuthenticationState for the user.
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var client = new BookClubRestClient();
                var jwtToken = await SecureStorage.GetAsync("accounttoken");
                if (jwtToken != null)
                {
                    string header, payload, signature;
                    if (JwtUtil.GetParts(jwtToken, out header, out payload, out signature))
                    {
                        // TODO: The serialization fails and I cannot figure why! Enter workaround...
                        var user = JsonSerializer.Deserialize<User>(payload, m_serializerOptions);

                        //var userElement = JsonSerializer.Deserialize<JsonElement>(payload);
                        //int id = userElement.GetProperty("id").GetInt32();
                        //string username = userElement.GetProperty("username").GetString();
                        //string firstName = userElement.GetProperty("firstName").GetString();
                        //string lastName = userElement.GetProperty("lastName").GetString();
                        //string email = userElement.GetProperty("email").GetString();
                        //string? aboutMe = userElement.GetProperty("aboutMe").GetString();
                        //UserRole role = (UserRole)userElement.GetProperty("role").GetInt32();
                        //DateTime expiration = DateTime.Parse(userElement.GetProperty("expiration").GetString());

                        //var user = new UserWithExpiration(
                        //    id, username, firstName, lastName, email, aboutMe, role, expiration
                        //);
                        if (await client.TestLogin(user.Id))
                        {
                            var claims = new Claim[] {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.GivenName, user.FirstName),
                            new Claim(ClaimTypes.Surname, user.LastName),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
                        };
                            var identity = new ClaimsIdentity(claims, "Custom");
                            return new AuthenticationState(new ClaimsPrincipal(identity));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check failed:" + ex.ToString());
            }
            return new AuthenticationState(new ClaimsPrincipal());
        }

    }

}