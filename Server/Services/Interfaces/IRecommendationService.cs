using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
    public interface IRecommendationService : IDisposable
    {

        BM.Recommendation Create(BM.Recommendation recommendation, bool loadChildren = true);
        BM.Recommendation? Read(int id, bool loadChildren = true);
        List<BM.Recommendation> ReadByMeeting(int parentId, bool loadChildren = true);
        bool Update(BM.Recommendation recommendation, bool loadChildren = true);
        int UpdateAll(int meetingId, List<BM.Recommendation> recommendations, bool loadChildren = true);
        bool Delete(int id);
        bool DeleteByMeeting(int meetingId);
        List<BM.Recommendation> SelectAll(bool loadChildren = true);

    }
}
