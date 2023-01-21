using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookClub.Models
{

    public class Book : Commentable
    {

        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(500), MinLength(1)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100), MinLength(1)]
        public string Author { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? Pages { get; set; }

        [MaxLength(24), MinLength(13)]
        public string? ISBN { get; set; }

        [MaxLength(20), MinLength(10)]
        public string? ASIN { get; set; }

        public int? ThumbnailId { get; set; }

        [Required]
        public DateTime Published { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChangeStatus Status { get; set; }

        // Referenced objects

        [NotMapped]
        public virtual Thumbnail? Thumbnail { get; set; }

        [NotMapped]
        public virtual List<Comment>? Comments { get; set; }

        // Methods

        public Book(
            int id,
            string title,
            string author,
            string? description,
            int? pages,
            string? isbn,
            string? asin,
            int? thumbnailId,
            DateTime published)
        {
            Id = id;
            Title = title;
            Author = author;
            Description = description;
            Pages = pages;
            ISBN = isbn;
            ASIN = asin;
            ThumbnailId = thumbnailId;
            Published = published;
        }

        public override bool Equals(object? obj)
        {
            var item = obj as Book;
            if (item == null) return false;

            return Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return (Id.ToString() + Title + Author).GetHashCode();
        }

        public override string ToString()
        {
            return Title;
        }

        ///<summary>
        ///This method should only be called by the BookService class!
        ///</summary>
        public void ServiceSetId__(int id)
        {
            Id = id;
        }

    }

}
