using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Drawing;
using MMG = Microsoft.Maui.Graphics;
namespace BookClub.Models
{

    public class Thumbnail
    {

        [Key]
        public int Id { get; private set; }

        [NotMapped]
        public byte[]? Image { get; set; }

        [NotMapped]
        public MMG.IImage? ImageObj { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ChangeStatus Status { get; set; }

        // Methods

        public Thumbnail(
            int id,
            byte[] image)
        {
            Id = id;
            Image = image;
        }

        public override bool Equals(object? obj)
        {
            var item = obj as Thumbnail;
            if (item == null) return false;

            return Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return (Id.ToString()).GetHashCode();
        }

        ///<summary>
        ///This method should only be called by the ThumbnailService class!
        ///</summary>
        public void ServiceSetId__(int id)
        {
            Id = id;
        }

    }

}
