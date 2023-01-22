using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookClub.Models
{

    public class Meeting : Commentable
    {

        private List<Recommendation> m_recommendations = new List<Recommendation>();

        [Key]
        public int Id { get; private set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int HostId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public DateTime MeetTime { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChangeStatus Status { get; set; }

        // Referenced objects

        [NotMapped]
        public virtual Book? Book { get; set; }

        [NotMapped]
        public virtual User? Host { get; set; }

        [NotMapped]
        public virtual Location? Location { get; set; }

        [NotMapped]
        public virtual List<Recommendation>? Recommendations { get; set; }

        [NotMapped]
        public virtual List<Comment>? Comments { get; set; }

        // Methods

        public Meeting(
            int id,
            int bookId,
            int hostId,
            int locationId,
            DateTime meetTime,
            string? description)
        {
            Id = id;
            BookId = bookId;
            HostId = hostId;
            LocationId = locationId;
            MeetTime = meetTime;
            Description = description;
        }

        public override string ToString()
        {
            return MeetTime.ToString("dddd, MMMM d, yyyy @ h:mm tt");
        }

        public override bool Equals(object? obj)
        {
            var item = obj as Meeting;
            if (item == null) return false;

            return Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return (Id.ToString() + BookId.ToString()).GetHashCode();
        }

        ///<summary>
        ///This method should only be called by the MeetingService class!
        ///</summary>
        public void ServiceSetId__(int id)
        {
            Id = id;
        }

    }

}
