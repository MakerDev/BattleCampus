using System;
using System.Collections.Generic;
using System.Text;

namespace BattleCampusMatchServer.Models
{
    public class GuestUser : User
    {
        public GuestUser()
        {
            Name = "Guest";
            StudentID = null;
            ID = Guid.NewGuid();
        }
    }
}
