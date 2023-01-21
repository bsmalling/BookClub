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

    public class RecommendationService : BaseService, IRecommendationService
    {

        public RecommendationService(BookClubContext context)
        : base(context)
        {
            // Intentionally blank
        }

        public BM.Recommendation Create(BM.Recommendation recommendation, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Recommendations/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@MeetingId", recommendation.MeetingId);
            command.Parameters.AddWithValue("@BookId", recommendation.BookId);
            command.Parameters.AddWithValue("@UserId", recommendation.UserId);
            command.Parameters.AddWithValue("@Created", recommendation.Created);
            if (loadChildren) LoadObjects(recommendation);

            int id = Convert.ToInt32(command.ExecuteScalar());
            recommendation.Status = BM.ChangeStatus.None;
            recommendation.ServiceSetId__(id);
            return recommendation;
        }

        public BM.Recommendation? Read(int id, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Recommendations/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            BM.Recommendation? recommendation = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    recommendation = new BM.Recommendation(
                        (int)reader["Id"],
                        (int)reader["MeetingId"],
                        (int)reader["BookId"],
                        (int)reader["UserId"],
                        (DateTime)reader["Created"]
                    );
                }
            }
            if (loadChildren && recommendation != null)
                LoadObjects(recommendation);
            return recommendation;
        }

        public List<BM.Recommendation> ReadByMeeting(int parentId, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Recommendations/ReadByMeeting");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@MeetingId", parentId);

            var recommendations = new List<BM.Recommendation>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var recommendation = new BM.Recommendation(
                        (int)reader["Id"],
                        (int)reader["MeetingId"],
                        (int)reader["BookId"],
                        (int)reader["UserId"],
                        (DateTime)reader["Created"]
                    );
                    recommendations.Add(recommendation);
                }
            }
            if (loadChildren)
                foreach (var recommendation in recommendations)
                    LoadObjects(recommendation);
            return recommendations;
        }

        public bool Update(BM.Recommendation recommendation, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Recommendations/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", recommendation.Id);
            command.Parameters.AddWithValue("@MeetingId", recommendation.MeetingId);
            command.Parameters.AddWithValue("@BookId", recommendation.BookId);
            command.Parameters.AddWithValue("@UserId", recommendation.UserId);
            command.Parameters.AddWithValue("@Created", recommendation.Created);

            if (command.ExecuteNonQuery() > 0)
            {
                if (loadChildren) LoadObjects(recommendation);
                recommendation.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(int meetingId, List<BM.Recommendation> recommendations, bool loadChildren = true)
        {
            int count = 0;
            for (int i = recommendations.Count - 1; i >= 0; i--)
            {
                var recommendation = recommendations[i];
                recommendation.MeetingId = meetingId;
                bool updated = false;

                switch (recommendation.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(recommendation, loadChildren);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(recommendation, loadChildren);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(recommendation.Id);
                        recommendations.RemoveAt(i);
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
            string sqlCommand = GetSqlCommand("Recommendations/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ParentType", BM.ParentType.Recommendation);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteByMeeting(int meetingId)
        {
            string sqlCommand = GetSqlCommand("Recommendations/DeleteByMeeting");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@MeetingId", meetingId);

            return command.ExecuteNonQuery() > 0;
        }

        public List<BM.Recommendation> SelectAll(bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Recommendations/SelectAll");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());

            var recommendations = new List<BM.Recommendation>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var recommendation = new BM.Recommendation(
                        (int)reader["Id"],
                        (int)reader["MeetingId"],
                        (int)reader["BookId"],
                        (int)reader["UserId"],
                        (DateTime)reader["Created"]
                    );
                    recommendations.Add(recommendation);
                }
            }
            if (loadChildren)
                foreach (var recommendation in recommendations)
                    LoadObjects(recommendation);
            return recommendations;
        }

        private void LoadObjects(BM.Recommendation recommendation)
        {
            if (recommendation.Meeting == null)
            {
                var meetingService = new MeetingService(m_context);
                recommendation.Meeting = meetingService.Read(recommendation.MeetingId, false);
            }
            if (recommendation.Book == null)
            {
                var bookService = new BookService(m_context);
                recommendation.Book = bookService.Read(recommendation.BookId, false);
            }
            if (recommendation.User == null)
            {
                var userService = new UserService(m_context);
                recommendation.User = userService.Read(recommendation.UserId);
            }
            if (recommendation.Comments == null || recommendation.Comments.Count == 0)
            {
                recommendation.Comments = LoadComments(BM.ParentType.Recommendation, recommendation.Id, false);
            }
        }

    }

}
