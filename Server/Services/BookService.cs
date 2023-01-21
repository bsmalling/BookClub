using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BM = BookClub.Models;
using BookClub.Contexts;
using BookClub.Services.Interfaces;

namespace BookClub.Services
{

    public class BookService : BaseService, IBookService
    {

        public BookService(BookClubContext context)
            : base(context)
        {
            // Intentionally blank
        }

        public BM.Book Create(BM.Book book, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Books/Create");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@Description", book.Description);
            command.Parameters.AddWithValue("@Pages", book.Pages);
            command.Parameters.AddWithValue("@ISBN", book.ISBN);
            command.Parameters.AddWithValue("@ASIN", book.ASIN);
            command.Parameters.AddWithValue("@ThumbnailId", book.ThumbnailId);
            command.Parameters.AddWithValue("@Published", book.Published);
            if (loadChildren) LoadObjects(book);

            int id = Convert.ToInt32(command.ExecuteScalar());
            book.Status = BM.ChangeStatus.None;
            book.ServiceSetId__(id);
            return book;
        }

        public BM.Book? Read(int id, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Books/Read");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            BM.Book? book = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    book = new BM.Book(
                        (int)reader["Id"],
                        (string)reader["Title"],
                        (string)reader["Author"],
                        ConvertDBVal<string>(reader["Description"]),
                        (int?)reader["Pages"],
                        ConvertDBVal<string>(reader["ISBN"]),
                        ConvertDBVal<string>(reader["ASIN"]),
                        ConvertDBInt(reader["ThumbnailId"]),
                        (DateTime)reader["Published"]
                    );
                }
            }
            if (loadChildren && book != null)
                LoadObjects(book);
            return book;
        }

        public bool Update(BM.Book book, bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Books/Update");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", book.Id);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@Description", book.Description);
            command.Parameters.AddWithValue("@Pages", book.Pages);
            command.Parameters.AddWithValue("@ISBN", book.ISBN);
            command.Parameters.AddWithValue("@ASIN", book.ASIN);
            command.Parameters.AddWithValue("@ThumbnailId", book.ThumbnailId);
            command.Parameters.AddWithValue("@Published", book.Published);

            if (command.ExecuteNonQuery() > 0)
            {
                if (loadChildren) LoadObjects(book);
                book.Status = BM.ChangeStatus.None;
                return true;
            }
            return false;
        }

        public int UpdateAll(List<BM.Book> books, bool loadChildren = true)
        {
            int count = 0;
            for (int i = books.Count - 1; i >= 0; i--)
            {
                var book = books.ElementAt<BM.Book>(i);
                bool updated = false;

                switch (book.Status)
                {
                    case BM.ChangeStatus.New:
                        Create(book, loadChildren);
                        updated = true;
                        break;
                    case BM.ChangeStatus.Changed:
                        updated = Update(book, loadChildren);
                        break;
                    case BM.ChangeStatus.Deleted:
                        updated = Delete(book.Id);
                        books.RemoveAt(i);
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
            string sqlCommand = GetSqlCommand("Books/Delete");

            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ParentType", BM.ParentType.Book);

            return command.ExecuteNonQuery() > 0;
        }

        public List<BM.Book> SelectAll(bool loadChildren = true)
        {
            string sqlCommand = GetSqlCommand("Books/SelectAll");
            SqlCommand command = new SqlCommand(sqlCommand, m_context.GetConnection());

            var books = new List<BM.Book>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var book = new BM.Book(
                        (int)reader["Id"],
                        (string)reader["Title"],
                        (string)reader["Author"],
                        ConvertDBVal<string>(reader["Description"]),
                        (int?)reader["Pages"],
                        ConvertDBVal<string>(reader["ISBN"]),
                        ConvertDBVal<string>(reader["ASIN"]),
                        ConvertDBInt(reader["ThumbnailId"]),
                        (DateTime)reader["Published"]
                    );
                    books.Add(book);
                }
            }
            if (loadChildren)
                foreach (var book in books)
                    LoadObjects(book);
            return books;
        }

        private void LoadObjects(BM.Book book)
        {
            if (book.ThumbnailId.HasValue && book.Thumbnail == null)
            {
                var thumbnailService = new ThumbnailService(m_context);
                book.Thumbnail = thumbnailService.Read(book.ThumbnailId.Value);
            }
            if (book.Comments == null || book.Comments.Count == 0)
            {
                book.Comments = LoadComments(BM.ParentType.Book, book.Id, false);
            }
        }

    }

}
