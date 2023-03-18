using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MMG = Microsoft.Maui.Graphics;

using BM = BookClub.Models;
using BookClub.Services.Interfaces;
using BookClub.Contexts;
using System.Drawing;

namespace BookClub.Services
{

    public class ThumbnailService : BaseService, IThumbnailService
    {

        public ThumbnailService(BookClubContext context)
            : base(context)
        {
            // Intentionally blank
        }

        public BM.Thumbnail Create(BM.Thumbnail thumbnail)
        {
            string? sqlCommand = GetSqlCommand("Thumbnails/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            var bytes = ImageToBytes(thumbnail.ImageObj);
            command.Parameters.Add("@Image", SqlDbType.VarBinary, -1).Value = bytes;

            int id = Convert.ToInt32(command.ExecuteScalar());
            thumbnail.Status = BM.ChangeStatus.None;
            thumbnail.ServiceSetId__(id);
            return thumbnail;
        }

        public BM.Thumbnail? Read(int id)
        {
            string? sqlCommand = GetSqlCommand("Thumbnails/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var thumbnail = new BM.Thumbnail(
                        (int)reader["Id"],
                        (byte[])reader["Image"]
                    );
                    return thumbnail;
                }
            }
            return null;
        }

        public bool Update(BM.Thumbnail thumbnail)
        {
            string? sqlCommand = GetSqlCommand("Thumbnails/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", thumbnail.Id);
            var bytes = ImageToBytes(thumbnail.ImageObj);
            command.Parameters.AddWithValue("@Image", bytes);

            if (command.ExecuteNonQuery() > 0)
            {
                thumbnail.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(List<BM.Thumbnail> thumbnails)
        {
            int count = 0;
            for (int i = thumbnails.Count - 1; i >= 0; i--)
            {
                var thumbnail = thumbnails[i];
                bool updated = false;

                switch (thumbnail.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(thumbnail);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(thumbnail);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(thumbnail.Id);
                        thumbnails.RemoveAt(i);
                        break;
                    default:
                        break;
                }
                if (updated) count += 1;
            }
            return count;
        }

        public bool Delete(int id)
        {
            string? sqlCommand = GetSqlCommand("Thumbnails/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ParentType", BM.ParentType.Thumbnail);

            return command.ExecuteNonQuery() > 0;
        }

        public static byte[] ImageToBytes(MMG.IImage image)
        {
            // TODO: Should not be Windows platform only
            ImageConverter imageConverter = new ImageConverter();
            return (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
        }

        public static Image BytesToImage(byte[] bytes)
        {
            // TODO: Should not be Windows platform only
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }

    }

}
