using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
	public interface IInvitationService : IDisposable
    {

        BM.Invitation Create(BM.Invitation Invitation, bool loadChildren = true);
        BM.Invitation? Read(string code, DateTime? nowDate = null, bool loadChildren = true);
        List<BM.Invitation> ReadByUser(int userId, bool loadChildren = true);
        bool Update(BM.Invitation Invitation, bool loadChildren = true);
        int UpdateAll(List<BM.Invitation> invitations, bool loadChildren = true);
        bool Delete(string code);
        List<BM.Invitation> SelectAll(bool loadChildren = true);

    }
}
