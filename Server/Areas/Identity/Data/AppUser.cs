using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Server.Areas.Identity
{

	public class AppUser : IdentityUser
	{

        //[MaxLength(48)]
        //public string? FirstName { get; set; }

        //[MaxLength(64)]
        //public string? LastName { get; set; }

        //[MaxLength(512)]
        //public string? AboutMe { get; set; }

    }

}
