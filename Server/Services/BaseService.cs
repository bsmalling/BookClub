using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Xml;
using System.Xml.XPath;
using System.Data.SqlTypes;

using BookClub.Models;
using BookClub.Contexts;
using System.Resources;
using System.Collections;
using System.Reflection;

namespace BookClub.Services
{

    public abstract class BaseService : IDisposable
    {

        protected BookClubContext m_context;
        private static XmlDocument? s_sqlCommands;
        private static object s_processLock = new object();

        public BaseService(BookClubContext context)
        {
            m_context = context;
            if (s_sqlCommands == null)
            {
                lock(s_processLock)
                {
                    if (s_sqlCommands == null)
                    {
                        s_sqlCommands = LoadSqlCommands();
                    }
                }
            }
        }

        private XmlDocument LoadSqlCommands()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] rn = assembly.GetManifestResourceNames();

            using (Stream stream = assembly.GetManifestResourceStream("BookClub.Data.SqlCommands.xml"))
            using (var reader = new StreamReader(stream))
            {
                var sqlCommands = new XmlDocument();
                sqlCommands.LoadXml(reader.ReadToEnd());
                return sqlCommands;
            }
        }

        protected string? GetSqlCommand(string path)
        {
            return s_sqlCommands.DocumentElement.SelectSingleNode(path)?.InnerText;
        }

        protected List<Comment> LoadComments(ParentType parentType, int parentId, bool loadChildren = true)
        {
            var commentService = new CommentService(m_context);
            return commentService.ReadByParent(parentType, parentId, loadChildren);
        }

        protected void SaveComments(ParentType parentType, int parentId, List<Comment> comments)
        {
            var commentService = new CommentService(m_context);
            if (comments == null || comments.Count == 0)
                commentService.DeleteByParent(parentType, parentId);
            else
                commentService.UpdateAll(parentType, parentId, comments);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (m_context != null)
                {
                    m_context.Dispose();
                    m_context = null;
                }
            }
            // free native resources
        }

        public static T ConvertDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return default(T);
            else
                return (T)obj;
        }

        public static int? ConvertDBInt(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return null;
            else
                return (int)obj;
        }

    }

}
