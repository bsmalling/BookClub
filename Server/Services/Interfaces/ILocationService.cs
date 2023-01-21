using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
    public interface ILocationService : IDisposable
    {

        BM.Location Create(BM.Location location);
        BM.Location? Read(int id);
        bool Update(BM.Location location);
        int UpdateAll(List<BM.Location> locations);
        bool Delete(int id);
        List<BM.Location> SelectAll();

    }
}
