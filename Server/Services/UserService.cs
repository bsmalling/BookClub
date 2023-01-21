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

    public class UserService : BaseService, IUserService
    {

        public UserService(BookClubContext context)
            : base(context)
        {
            // Intentionally blank
        }

        public BM.User Create(BM.User user, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Users/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@AboutMe", user.AboutMe);
            command.Parameters.AddWithValue("@NormalizedUserName", user.NormalizedUserName);

            int id = Convert.ToInt32(command.ExecuteScalar());
            user.Status = BM.ChangeStatus.None;
            user.ServiceSetId__(id);
            return user;
        }

        public BM.User? Read(int id, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Users/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var user = new BM.User(
                        (int)reader["Id"],
                        (string)reader["FirstName"],
                        (string)reader["LastName"],
                        ConvertDBVal<string>(reader["AboutMe"]),
                        (string)reader["NormalizedUserName"]
                    );
                    return user;
                }
            }
            return null;
        }

        public bool Update(BM.User user, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Users/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@AboutMe", user.AboutMe);
            command.Parameters.AddWithValue("@NormalizedUserName", user.NormalizedUserName);

            if (command.ExecuteNonQuery() > 0)
            {
                user.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(List<BM.User> users, bool loadChildren = true)
        {
            int count = 0;
            for (int i = users.Count - 1; i >= 0; i--)
            {
                var user = users[i];
                bool updated = false;

                switch (user.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(user);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(user);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(user.Id);
                        users.RemoveAt(i);
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
            string sqlCommand = GetSqlCommand("Users/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ParentType", BM.ParentType.User);

            return command.ExecuteNonQuery() > 0;
        }

        public List<BM.User> SelectAll(bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Users/SelectAll");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());

            var users = new List<BM.User>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new BM.User(
                        (int)reader["Id"],
                        (string)reader["FirstName"],
                        (string)reader["LastName"],
                        ConvertDBVal<string>(reader["AboutMe"]),
                        (string)reader["NormalizedUserName"]
                    );
                    users.Add(user);
                }
            }
            return users;
        }

    }

}
