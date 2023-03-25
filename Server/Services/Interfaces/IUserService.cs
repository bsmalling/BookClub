using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
    public interface IUserService : IDisposable
    {

        BM.User Create(BM.User user, bool loadChildren = true);
        BM.User? Read(int id, bool loadChildren = true);
        BM.User? GetByAspNetId(string aspNetId, bool leadChildren = true);
        bool Update(BM.User user, bool loadChildren = true);
        int UpdateAll(List<BM.User> users, bool loadChildren = true);
        bool Delete(int id);
        List<BM.User> SelectAll(bool loadChildren = true);

    }
}
