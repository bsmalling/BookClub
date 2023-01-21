using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
    public interface IMeetingService : IDisposable
    {

        BM.Meeting Create(BM.Meeting meeting, bool loadChildren = true);
        BM.Meeting? Read(int id, bool loadChildren = true);
        BM.Meeting? NextMeeting(DateTime? nowTime = null, bool loadChildren = true);
        bool Update(BM.Meeting meeting, bool loadChildren = true);
        int UpdateAll(List<BM.Meeting> meetings, bool loadChildren = true);
        bool Delete(int id);
        List<BM.Meeting> SelectAll(bool loadChildren = true);

    }
}

