using System.ComponentModel.DataAnnotations;

using BookClub.Models;

namespace Client
{

    public class LoginViewModel
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool LoginFailureHidden { get; set; } = true;

        public User TryLogin(out string jwtToken)
        {
            BookClubRestClient client = new BookClubRestClient();
            JwtUserToken token = Task.Run(() => client.TryLogin(Username, Password)).Result;
            if (token != null)
            {
                if (token.User.Expiration > DateTime.Now)
                {
                    LoginFailureHidden = true;
                    jwtToken = token.JwtToken;
                    return token.User;
                }
            }
            LoginFailureHidden = false;
            jwtToken = null;
            return null;
        }

    }

}