using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

using BookClub.Models;

namespace Client
{
	public class BookClubRestClient
    {

        public const string BookRoute = "books";
        public const string CommentRoute = "comments";
        public const string LocationRoute = "locations";
        public const string MeetingRoute = "meetings";
        public const string RecommendationRoute = "recommendations";
        public const string ThumbnailRoute = "thumbnails";
        public const string UserRoute = "users";

        private static string GetBaseAddress()
        {
            // dev.servicestack.com resolves to 10.0.2.2
            // local.servicestack.com resolves to localhost (127.0.0.1)
            // See https://developer.android.com/studio/run/emulator-networking for more info...
            const string localhost = "192.168.2.254";
#if DEBUG
            const string port = ":7129";
            string servername = "localhost";
            if (DeviceInfo.Platform == DevicePlatform.Android)
                servername = "10.0.2.2";
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
                servername = localhost;
#else
            const string port = "";
            const string servername = localhost;
#endif
            return "https://" + servername + port;
        }

#if DEBUG
        private static readonly string BaseAddress = GetBaseAddress();
#else
        private static string BaseAddress = "To be determined...";
#endif
        private static readonly string BaseURL = BaseAddress + "/api/v1.0/{0}";

        private readonly HttpClient m_client;
        private readonly JsonSerializerOptions m_serializerOptions;

        public BookClubRestClient()
        {
#if DEBUG
            HttpClientHandler insecureHandler = GetInsecureHandler();
            m_client = new HttpClient(insecureHandler);
#else
            m_client = new HttpClient();
#endif
            m_serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<JwtUserToken> TryLogin(string username, string password)
        {
            Uri uri = new Uri(string.Format(BaseURL, "login"));
            try
            {
                Credentials credentials = new Credentials(username, password);
                var request = JsonSerializer.Serialize<Credentials>(credentials, m_serializerOptions);
                var postContent = new StringContent(request, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await m_client.PostAsync(uri, postContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(content)) return null;
                    // TODO: The serialization fails and I cannot figure why! Enter workaround...
                    return JsonSerializer.Deserialize<JwtUserToken>(content, m_serializerOptions);

                    //var json = JsonSerializer.Deserialize<JsonElement>(content);
                    //var jwtToken = json.GetProperty("jwtToken").GetString();

                    //var userElement = json.GetProperty("user");
                    //int id = userElement.GetProperty("id").GetInt32();
                    //username = userElement.GetProperty("username").GetString();
                    //string firstName = userElement.GetProperty("firstName").GetString();
                    //string lastName = userElement.GetProperty("lastName").GetString();
                    //string email = userElement.GetProperty("email").GetString();
                    //string? aboutMe = userElement.GetProperty("aboutMe").GetString();
                    //UserRole role = (UserRole)userElement.GetProperty("role").GetInt32();
                    //DateTime expiration = DateTime.Parse(userElement.GetProperty("expiration").GetString());

                    //JwtUserToken token = new JwtUserToken(jwtToken,
                    //    new UserWithExpiration(id, username, firstName, lastName, email, aboutMe, role, expiration)
                    //);
                    //return token;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return null;
        }

        public async Task<bool> TestLogin(int id)
        {
            Uri uri = new Uri(string.Format(BaseURL, "login/id?id=" + id.ToString()));
            try
            {
                HttpResponseMessage response = await m_client.GetAsync(uri);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return false;
        }

        public async Task<bool> Logout(int id)
        {
            Uri uri = new Uri(string.Format(BaseURL, "login/id?id=" + id.ToString()));
            try
            {
                HttpResponseMessage response = await m_client.DeleteAsync(uri);
                if (response.IsSuccessStatusCode) return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return false;
        }

        public async Task<T> GetByIdAsync<T>(string model, int id)
        {
            Uri uri = new Uri(string.Format(BaseURL, model + "/id?id=" + id.ToString()));
            try
            {
                HttpResponseMessage response = await m_client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, m_serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return default(T);
        }

        public async Task<List<T>> GetListAsync<T>(string model)
        {
            var items = new List<T>();
            Uri uri = new Uri(string.Format(BaseURL, model));
            try
            {
                HttpResponseMessage response = await m_client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    items = JsonSerializer.Deserialize<List<T>>(content, m_serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return items;
        }

        public async Task<Meeting> GetNextMeetingAsync()
        {
            Uri uri = new Uri(string.Format(BaseURL, MeetingRoute + "/next"));
            try
            {
                HttpResponseMessage response = await m_client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Meeting>(content, m_serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return null;
        }

#if DEBUG
        // This is necessary if you do not have a valid, trusted cert during development
        private static HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }
#endif

    }
}
