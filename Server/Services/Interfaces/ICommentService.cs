using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
    public interface ICommentService : IDisposable
    {

        BM.Comment Create(BM.Comment comment, bool loadChildren = true);
        BM.Comment? Read(int id, bool loadChildren = true);
        List<BM.Comment> ReadByParent(BM.ParentType parentType, int parentId, bool loadChildren = true);
        bool Update(BM.Comment comment, bool loadChildren = true);
        int UpdateAll(BM.ParentType parentType, int parentId, List<BM.Comment> comments, bool loadChildren = true);
        bool Delete(int id);
        bool DeleteByParent(BM.ParentType parentType, int parentId);
        List<BM.Comment> SelectAll(bool loadChildren = true);

    }
}
