using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookClub.Models
{

    public class Comment
    {

        [Key]
        public int Id { get; private set; }

        [Required]
        public ParentType ParentType { get; set; }

        [Required]
        public int ParentId { get; set; }

        [Required]
        [MaxLength(500), MinLength(1)]
        public string Text { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChangeStatus Status { get; set; }

        // Referenced objects

        [NotMapped]
        public virtual User? User { get; set; }

        // Methods

        public Comment(
            int id,
            ParentType parentType,
            int parentId,
            string text,
            int userId,
            DateTime created)
        {
            Id = id;
            ParentType = parentType;
            ParentId = parentId;
            Text = text;
            UserId = userId;
            Created = created;
        }

        public override bool Equals(object? obj)
        {
            var item = obj as Comment;
            if (item == null) return false;

            return Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return (Id.ToString()).GetHashCode();
        }

        ///<summary>
        ///This method should only be called by the CommentService class!
        ///</summary>
        public void ServiceSetId__(int id)
        {
            Id = id;
        }

    }

}
