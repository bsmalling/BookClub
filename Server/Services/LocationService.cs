using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BM = BookClub.Models;
using BookClub.Services.Interfaces;
using BookClub.Contexts;

namespace BookClub.Services
{

    public class LocationService : BaseService, ILocationService
    {

        public LocationService(BookClubContext context)
            : base(context)
        {
            // Intentionally blank
        }

        public BM.Location Create(BM.Location location)
        {
            string sqlCommand = GetSqlCommand("Locations/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Address1", location.Address1);
            command.Parameters.AddWithValue("@Address2", location.Address2);
            command.Parameters.AddWithValue("@City", location.City);
            command.Parameters.AddWithValue("@State", location.State);
            command.Parameters.AddWithValue("@Zip", location.Zip);
            command.Parameters.AddWithValue("@Description", location.Description);

            int id = Convert.ToInt32(command.ExecuteScalar());
            location.Status = BM.ChangeStatus.None;
            location.ServiceSetId__(id);
            return location;
        }

        public BM.Location? Read(int id)
        {
            string sqlCommand = GetSqlCommand("Locations/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var location = new BM.Location(
                        (int)reader["Id"],
                        (string)reader["Address1"],
                        ConvertDBVal<string>(reader["Address2"]),
                        (string)reader["City"],
                        (string)reader["State"],
                        ConvertDBVal<string>(reader["Zip"]),
                        ConvertDBVal<string>(reader["Description"])
                    );
                    return location;
                }
            }
            return null;
        }

        public bool Update(BM.Location location)
        {
            string sqlCommand = GetSqlCommand("Locations/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", location.Id);
            command.Parameters.AddWithValue("@Address1", location.Address1);
            command.Parameters.AddWithValue("@Address2", location.Address2);
            command.Parameters.AddWithValue("@City", location.City);
            command.Parameters.AddWithValue("@State", location.State);
            command.Parameters.AddWithValue("@Zip", location.Zip);
            command.Parameters.AddWithValue("@Description", location.Description);

            if (command.ExecuteNonQuery() > 0)
            {
                location.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(List<BM.Location> locations)
        {
            int count = 0;
            for (int i = locations.Count - 1; i >= 0; i--)
            {
                var location = locations[i];
                bool updated = false;

                switch (location.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(location);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(location);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(location.Id);
                        locations.RemoveAt(i);
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
            string sqlCommand = GetSqlCommand("Locations/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ParentType", BM.ParentType.Location);

            return command.ExecuteNonQuery() > 0;
        }

        public List<BM.Location> SelectAll()
        {
            string sqlCommand = GetSqlCommand("Locations/SelectAll");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());

            var locations = new List<BM.Location>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var location = new BM.Location(
                        (int)reader["Id"],
                        (string)reader["Address1"],
                        ConvertDBVal<string>(reader["Address2"]),
                        (string)reader["City"],
                        (string)reader["State"],
                        ConvertDBVal<string>(reader["Zip"]),
                        ConvertDBVal<string>(reader["Description"])
                    );
                    locations.Add(location);
                }
            }
            return locations;
        }

    }

}
