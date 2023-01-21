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

    public class MeetingService : BaseService, IMeetingService
    {

        public MeetingService(BookClubContext context)
            : base(context)
        {
            // Intentionally blank
        }

        public BM.Meeting Create(BM.Meeting meeting, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Meetings/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@BookId", meeting.BookId);
            command.Parameters.AddWithValue("@HostId", meeting.HostId);
            command.Parameters.AddWithValue("@LocationId", meeting.LocationId);
            command.Parameters.AddWithValue("@MeetTime", meeting.MeetTime);
            command.Parameters.AddWithValue("@Description", meeting.Description);
            if (loadChildren) LoadObjects(meeting);

            int id = Convert.ToInt32(command.ExecuteScalar());
            meeting.Status = BM.ChangeStatus.None;
            meeting.ServiceSetId__(id);
            return meeting;
        }

        public BM.Meeting? Read(int id, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Meetings/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            BM.Meeting? meeting = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    meeting = new BM.Meeting(
                        (int)reader["Id"],
                        (int)reader["BookId"],
                        (int)reader["HostId"],
                        (int)reader["LocationId"],
                        (DateTime)reader["MeetTime"],
                        ConvertDBVal<string>(reader["Description"])
                    );
                }
            }
            if (loadChildren && meeting != null)
                LoadObjects(meeting);
            return meeting;
        }

        public BM.Meeting? NextMeeting(DateTime? nowTime = null, bool loadChildren = true)
        {
            if (nowTime == null) nowTime = DateTime.Now;

            string sqlCommand = GetSqlCommand("Meetings/NextMeeting");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@NowTime", nowTime);

            BM.Meeting? meeting = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    meeting = new BM.Meeting(
                        (int)reader["Id"],
                        (int)reader["BookId"],
                        (int)reader["HostId"],
                        (int)reader["LocationId"],
                        (DateTime)reader["MeetTime"],
                        ConvertDBVal<string>(reader["Description"])
                    );
                }
            }
            if (loadChildren && meeting != null)
                LoadObjects(meeting);
            return meeting;
        }

        public bool Update(BM.Meeting meeting, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Meetings/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", meeting.Id);
            command.Parameters.AddWithValue("@BookId", meeting.BookId);
            command.Parameters.AddWithValue("@HostId", meeting.HostId);
            command.Parameters.AddWithValue("@LocationId", meeting.LocationId);
            command.Parameters.AddWithValue("@MeetTime", meeting.MeetTime);
            command.Parameters.AddWithValue("@Description", meeting.Description);

            if (command.ExecuteNonQuery() > 0)
            {
                if (loadChildren) LoadObjects(meeting);
                meeting.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(List<BM.Meeting> meetings, bool loadChildren = true)
        {
            int count = 0;
            for (int i = meetings.Count - 1; i >= 0; i--)
            {
                var meeting = meetings[i];
                bool updated = false;

                switch (meeting.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(meeting, loadChildren);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(meeting, loadChildren);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(meeting.Id);
                        meetings.RemoveAt(i);
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
            string sqlCommand = GetSqlCommand("Meetings/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ParentType", BM.ParentType.Meeting);

            return command.ExecuteNonQuery() > 0;
        }

        public List<BM.Meeting> SelectAll(bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Meetings/SelectAll");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());

            var meetings = new List<BM.Meeting>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var meeting = new BM.Meeting(
                        (int)reader["Id"],
                        (int)reader["BookId"],
                        (int)reader["HostId"],
                        (int)reader["LocationId"],
                        (DateTime)reader["MeetTime"],
                        ConvertDBVal<string>(reader["Description"])
                    );
                    meetings.Add(meeting);
                }
            }
            if (loadChildren)
                foreach (var meeting in meetings)
                    LoadObjects(meeting);
            return meetings;
        }

        private void LoadObjects(BM.Meeting meeting)
        {
            if (meeting.Book == null)
            {
                var bookService = new BookService(m_context);
                meeting.Book = bookService.Read(meeting.BookId, false);
            }
            if (meeting.Host == null)
            {
                var userService = new UserService(m_context);
                meeting.Host = userService.Read(meeting.HostId, false);
            }
            if (meeting.Location == null)
            {
                var locationService = new LocationService(m_context);
                meeting.Location = locationService.Read(meeting.LocationId);
            }
            if (meeting.Comments == null || meeting.Comments.Count == 0)
            {
                meeting.Comments = LoadComments(BM.ParentType.Meeting, meeting.Id, false);
            }
            if (meeting.Recommendations == null || meeting.Recommendations.Count == 0)
            {
                meeting.Recommendations = LoadRecommendations(meeting.Id, false);
            }
        }

        private List<BM.Recommendation> LoadRecommendations(int meetingId, bool loadChildren = true)
        {
            var recommendationService = new RecommendationService(m_context);
            return recommendationService.ReadByMeeting(meetingId, loadChildren);
        }

        private void SaveRecommendations(int meetingId, List<BM.Recommendation> recommendations)
        {
            var recommendationService = new RecommendationService(m_context);
            if (recommendations == null || recommendations.Count == 0)
                recommendationService.DeleteByMeeting(meetingId);
            else
                recommendationService.UpdateAll(meetingId, recommendations);
        }

    }

}
