using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookClub.Models
{

    public class Invitation
    {

        [Key]
        [MaxLength(9), MinLength(9)]
        public string Code { get; private set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        [Required]
        public bool Claimed { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChangeStatus Status { get; set; }

        // Referenced objects

        [NotMapped]
        public virtual User? User { get; set; }

        // Methods

        public Invitation(
            string code,
            int userId,
            DateTime expiration,
            bool claimed,
            string notes)
        {
            Code = code;
            UserId = userId;
            Expiration = expiration;
            Claimed = claimed;
            Notes = notes;
        }

        public override bool Equals(object? obj)
        {
            var item = obj as Comment;
            if (item == null) return false;

            return Code.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

    }

}
