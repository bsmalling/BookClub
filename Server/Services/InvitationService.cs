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

    public class InvitationService : BaseService, IInvitationService
    {

        public InvitationService(BookClubContext context)
            : base(context)
        {
            // Intentionally blank
        }

        public BM.Invitation Create(BM.Invitation invitation, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Invitations/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Code", invitation.Code);
            command.Parameters.AddWithValue("@UserId", invitation.UserId);
            command.Parameters.AddWithValue("@Expiration", invitation.Expiration);
            command.Parameters.AddWithValue("@Claimed", invitation.Claimed);
            command.Parameters.AddWithValue("@Notes", invitation.Notes);
            if (loadChildren) LoadObjects(invitation);

            invitation.Status = BM.ChangeStatus.None;
            return invitation;
        }

        public BM.Invitation? Read(string code, DateTime? nowDate = null, bool loadChildren = true)
        {
            if (nowDate == null) nowDate = DateTime.Now;

            string sqlCommand = GetSqlCommand("Invitations/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Code", code);
            command.Parameters.AddWithValue("@NowDate", nowDate);

            BM.Invitation? invitation = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    invitation = new BM.Invitation(
                        (string)reader["Code"],
                        (int)reader["UserId"],
                        (DateTime)reader["Expiration"],
                        (bool)reader["Claimed"],
                        ConvertDBVal<string>(reader["Notes"])
                    );
                    return invitation;
                }
            }
            if (loadChildren && invitation != null)
                LoadObjects(invitation);
            return null;
        }

        public List<BM.Invitation> ReadByUser(int userId, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Invitations/ReadByUser");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);

            var invitations = new List<BM.Invitation>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var invitation = new BM.Invitation(
                        (string)reader["Code"],
                        (int)reader["UserId"],
                        (DateTime)reader["Expiration"],
                        (bool)reader["Claimed"],
                        ConvertDBVal<string>(reader["Notes"])
                    );
                    invitations.Add(invitation);
                }
            }
            if (loadChildren)
                foreach (var invitation in invitations)
                    LoadObjects(invitation);
            return invitations;
        }

        public bool Update(BM.Invitation invitation, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Invitations/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Code", invitation.Code);
            command.Parameters.AddWithValue("@UserId", invitation.UserId);
            command.Parameters.AddWithValue("@Expiration", invitation.Expiration);
            command.Parameters.AddWithValue("@Claimed", invitation.Claimed);
            command.Parameters.AddWithValue("@Notes", invitation.Notes);

            if (command.ExecuteNonQuery() > 0)
            {
                if (loadChildren) LoadObjects(invitation);
                invitation.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(List<BM.Invitation> invitations, bool loadChildren = true)
        {
            int count = 0;
            for (int i = invitations.Count - 1; i >= 0; i--)
            {
                var invitation = invitations[i];
                bool updated = false;

                switch (invitation.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(invitation);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(invitation);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(invitation.Code);
                        invitations.RemoveAt(i);
                        break;
                    default:
                        break;
                }
                if (updated) count += 1;
            }
            if (loadChildren)
                foreach (var invitation in invitations)
                    LoadObjects(invitation);
            return count;
        }

        public bool Delete(string code)
        {
            string sqlCommand = GetSqlCommand("Invitations/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Code", code);

            return command.ExecuteNonQuery() > 0;
        }

        public List<BM.Invitation> SelectAll(bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Invitations/SelectAll");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());

            var invitations = new List<BM.Invitation>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var invitation = new BM.Invitation(
                        (string)reader["Code"],
                        (int)reader["UserId"],
                        (DateTime)reader["Expiration"],
                        (bool)reader["Claimed"],
                        ConvertDBVal<string>(reader["Notes"])
                    );
                    invitations.Add(invitation);
                }
            }
            if (loadChildren)
                foreach (var invitation in invitations)
                    LoadObjects(invitation);
            return invitations;
        }

        private void LoadObjects(BM.Invitation invitation)
        {
            if (invitation.User == null)
            {
                var userService = new UserService(m_context);
                invitation.User = userService.Read(invitation.UserId);
            }
        }

    }

}
