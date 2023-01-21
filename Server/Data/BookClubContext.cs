using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using BookClub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookClub.Contexts
{

    public abstract class BookClubContext : IdentityDbContext
    {

        public const string USER_ROLE  = "USER";
        public const string POWER_ROLE = "POWER";
        public const string ADMIN_ROLE = "ADMIN";

        private readonly string? m_connectionString;
        private SqlConnection? m_connection;

        public BookClubContext(DbContextOptions options)
            : base(options)
        {
            m_connectionString = this.Database.GetConnectionString();
        }

        //public DbSet<Book> Books { get; set; }
        //public DbSet<Comment> Comments { get; set; }
        //public DbSet<Location> Locations { get; set; }
        //public DbSet<Meeting> Meetings { get; set; }
        //public DbSet<Recommendation> Recommendations { get; set; }
        //public DbSet<Thumbnail> Thumbnails { get; set; }
        //public DbSet<User> Users { get; set; }

        public SqlConnection GetConnection()
        {
            if (m_connection == null)
            {
                var connection = new SqlConnection(m_connectionString);
                connection.Open();
                m_connection = connection;
            }
            return m_connection;
        }

        public override void Dispose()
        {
            base.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (m_connection != null)
                {
                    m_connection.Dispose();
                    m_connection = null;
                }
            }
            // free native resources
        }

    }

    public class AdminBookClubContext : BookClubContext
    {

        // Created with admin connection string
        public AdminBookClubContext(DbContextOptions<AdminBookClubContext> options)
            : base(options)
        {
            // Intentionally blank
        }

    }

    public class UserBookClubContext : BookClubContext
    {

        // Created with user connection string
        public UserBookClubContext(DbContextOptions<UserBookClubContext> options)
            : base(options)
        {
            // Intentionally blank
        }

    }

}
