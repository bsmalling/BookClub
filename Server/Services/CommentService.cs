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

    public class CommentService : BaseService, ICommentService
    {

        public CommentService(BookClubContext context)
            : base(context)
        {
            // Intentionally blank
        }

        public BM.Comment Create(BM.Comment comment, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Comments/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@ParentType", comment.ParentType);
            command.Parameters.AddWithValue("@ParentId", comment.ParentId);
            command.Parameters.AddWithValue("@Text", comment.Text);
            command.Parameters.AddWithValue("@UserId", comment.UserId);
            command.Parameters.AddWithValue("@Created", comment.Created);
            if (loadChildren) LoadObjects(comment);

            int id = Convert.ToInt32(command.ExecuteScalar());
            comment.Status = BM.ChangeStatus.None;
            comment.ServiceSetId__(id);
            return comment;
        }

        public BM.Comment? Read(int id, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Comments/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            BM.Comment? comment = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    comment = new BM.Comment(
                        (int)reader["Id"],
                        (BM.ParentType)reader["ParentType"],
                        (int)reader["ParentId"],
                        (string)reader["Text"],
                        (int)reader["UserId"],
                        (DateTime)reader["Created"]
                    );
                }
            }
            if (loadChildren && comment != null)
                LoadObjects(comment);
            return comment;
        }

        public List<BM.Comment> ReadByParent(BM.ParentType parentType, int parentId, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Comments/ReadByParent");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@ParentType", parentType);
            command.Parameters.AddWithValue("@ParentId", parentId);

            var comments = new List<BM.Comment>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var comment = new BM.Comment(
                        (int)reader["Id"],
                        (BM.ParentType)reader["ParentType"],
                        (int)reader["ParentId"],
                        (string)reader["Text"],
                        (int)reader["UserId"],
                        (DateTime)reader["Created"]
                    );
                    comments.Add(comment);
                }
            }
            if (loadChildren)
                foreach (var comment in comments)
                    LoadObjects(comment);
            return comments;
        }

        public bool Update(BM.Comment comment, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Comments/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", comment.Id);
            command.Parameters.AddWithValue("@ParentType", comment.ParentType);
            command.Parameters.AddWithValue("@ParentId", comment.ParentId);
            command.Parameters.AddWithValue("@Text", comment.Text);
            command.Parameters.AddWithValue("@UserId", comment.UserId);
            command.Parameters.AddWithValue("@Created", comment.Created);

            if (command.ExecuteNonQuery() > 0)
            {
               if (loadChildren) LoadObjects(comment);
                comment.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(BM.ParentType parentType, int parentId, List<BM.Comment> comments, bool loadChildren = true)
        {
            int count = 0;
            for (int i = comments.Count - 1; i >= 0; i--)
            {
                var comment = comments[i];
                comment.ParentType = parentType;
                comment.ParentId = parentId;
                bool updated = false;

                switch (comment.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(comment, loadChildren);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(comment, loadChildren);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(comment.Id);
                        comments.RemoveAt(i);
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
            string sqlCommand = GetSqlCommand("Comments/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ParentType", BM.ParentType.Comment);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteByParent(BM.ParentType parentType, int parentId)
        {
            string sqlCommand = GetSqlCommand("Comments/DeleteByParent");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@ParentType", parentType);
            command.Parameters.AddWithValue("@ParentId", parentId);

            return command.ExecuteNonQuery() > 0;
        }

        public List<BM.Comment> SelectAll(bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Comments/SelectAll");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());

            var comments = new List<BM.Comment>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var comment = new BM.Comment(
                        (int)reader["Id"],
                        (BM.ParentType)reader["ParentType"],
                        (int)reader["ParentId"],
                        (string)reader["Text"],
                        (int)reader["UserId"],
                        (DateTime)reader["Created"]
                    );
                    comments.Add(comment);
                }
            }
            if (loadChildren)
                foreach (var comment in comments)
                    LoadObjects(comment);
            return comments;
        }

        private void LoadObjects(BM.Comment comment)
        {
            if (comment.User == null)
            {
                var userService = new UserService(m_context);
                comment.User = userService.Read(comment.UserId);
            }
        }

    }

}
