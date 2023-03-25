using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookClub.Models
{

    public class User
    {

        [Key]
        public int Id { get; protected set; }

        [Required]
        [MaxLength(100), MinLength(1)]
        public string FirstName { get; set; }

        [MaxLength(100), MinLength(1)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(128)]
        public string AspNetId { get; set; }

        [Required]
        public bool Deleted { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChangeStatus Status { get; set; }

        // Methods

        public User(
            int id,
            string firstName,
            string? lastName,
            string aspNetId)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            AspNetId = aspNetId;
        }

        [NotMapped]
        [JsonIgnore]
        public string FullName
        {
            get
            {
                if (!Deleted)
                {
                    if (String.IsNullOrEmpty(LastName))
                        return FirstName;
                    else
                        return FirstName + " " + LastName;
                }
                else
                {
                    if (String.IsNullOrEmpty(LastName))
                        return FirstName;
                    else
                        return FirstName + " " + LastName[0] + ".";
                }
            }
        }

        public override bool Equals(object? obj)
        {
            var item = obj as User;
            if (item == null) return false;

            return Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return Id.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }

        ///<summary>
        ///This method should only be called by the UserService class!
        ///</summary>
        public void ServiceSetId__(int id)
        {
            Id = id;
        }

    }

}
