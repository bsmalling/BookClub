using System;
using BM = BookClub.Models;

namespace BookClub.Services.Interfaces
{
	interface IBookService : IDisposable
    {

        BM.Book Create(BM.Book book, bool loadChildren = true);
        BM.Book? Read(int id, bool loadChildren = true);
        bool Update(BM.Book book, bool loadChildren = true);
        int UpdateAll(List<BM.Book> books, bool loadChildren = true);
        bool Delete(int id);
        List<BM.Book> SelectAll(bool loadChildren = true);

    }
}

