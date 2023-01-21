using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookClub.Models
{

    [Flags]
    public enum UserRole : int
    {
        None = 0,
        User = 1,
        Power = 3,
        Admin = 7
    }

    public enum ParentType : int
    {
        Book = 0,
        Comment = 1,
        Location = 2,
        Meeting = 3,
        Recommendation = 4,
        Thumbnail = 5,
        User = 6
    }

    public enum ChangeStatus : int
    {
        None = 0,
        New = 1,
        Changed = 2,
        Deleted = 3
    }

}
