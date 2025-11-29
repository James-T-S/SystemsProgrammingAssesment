using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SystemsProgrammingAssesment
{
    internal class Clan

    {
        public int ClanID { get; set; }
        public string ClanName { get; set; } = "";
        public List<User> Members { get; set; } = new List<User>();
        private string Rank
        {
            get { return Rank; }
            set
            {
                if (Rank.ToLower() != "gold" || Rank.ToLower() != "silver" || Rank.ToLower() != "bronze") Rank = Rank;

                else Rank = "None";
            }
        }
        private int ClanScore
        {
            get { return ClanScore; }

            set
            {
                ClanScore = 0;

                foreach (User member in Members)
                {
                    ClanScore += member.HighScore;
                }
            }
        }

        public Clan(int ClanID, string ClanName, string Rank, int ClanScore)
        {
            this.ClanID = ClanID;
            this.ClanName = ClanName;
            this.Rank = Rank;
            this.ClanScore = ClanScore;
        }
    }
}
