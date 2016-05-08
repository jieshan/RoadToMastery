using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadToMastery.Analysis
{
    public class ChampionMastery
    {
        public int gameWon;
        public int gameLost;
        public int masteryPoint;
        public int masteryLevel;
        public int totalPhysicalDamage;
        public int totalMagicDamageDealt;
        public int totalCS;
        public int totalGold;
        public int totalTurrets;

        public int gamePlayed { get; set; }
        public int totalChampKills { get; set; }
        public int totalDeaths { get; set; }
        public int totalAssists { get; set; }
        public int totalDamage { get; set; }

        public double winRate { get; set; }

        public ChampionMastery(string[] dataFeed) 
        {
            this.masteryPoint = int.Parse(dataFeed[2]);
            this.masteryLevel = int.Parse(dataFeed[3]);
            this.gamePlayed = int.Parse(dataFeed[4]);
            this.gameWon = int.Parse(dataFeed[5]);
            this.gameLost = int.Parse(dataFeed[6]);
            this.totalChampKills = int.Parse(dataFeed[7]);
            this.totalDeaths = int.Parse(dataFeed[8]);
            this.totalAssists = int.Parse(dataFeed[9]);
            this.totalDamage = int.Parse(dataFeed[10]);
            this.totalPhysicalDamage = int.Parse(dataFeed[11]);
            this.totalMagicDamageDealt = int.Parse(dataFeed[12]);
            this.totalCS = int.Parse(dataFeed[13]);
            this.totalGold = int.Parse(dataFeed[14]);
            this.totalTurrets = int.Parse(dataFeed[15]);

            this.winRate = this.gameWon * 1.0 / this.gamePlayed;
        }
    }
}
