using System;
using System.Collections.Generic;
using System.Text;

namespace BattleCampusMatchServer.Models
{
    [Serializable]
    public class User
    {
        public Guid ID { get; set; }
        public string StudentID { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        //Joining match ID
        public string MatchID { get; set; }
        public bool IsHost { get; set; } = false;
        public int ConnectionID { get; set; } = -1;
    }
}
