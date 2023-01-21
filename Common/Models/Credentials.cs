using System;
using System.ComponentModel.DataAnnotations;

namespace BookClub.Models
{

	public class Credentials
	{

        [Required]
        public string Username { get; private set; }

        [Required]
        public string Password { get; private set; }

        public Credentials(string username, string password)
		{
			Username = username;
			Password = password;
		}

	}

}
