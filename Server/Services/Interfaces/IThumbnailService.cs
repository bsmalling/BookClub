using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
    public interface IThumbnailService : IDisposable
    {

        BM.Thumbnail Create(BM.Thumbnail thumbnail);
        BM.Thumbnail? Read(int id);
        bool Update(BM.Thumbnail thumbnail);
        int UpdateAll(List<BM.Thumbnail> thumbnails);
        bool Delete(int id);

    }
}

