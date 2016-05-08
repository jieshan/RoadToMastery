using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoadToMastery.Analysis
{
    public class ChampionLevel
    {
        public int level;

        public int count;

        public int totalGamePlayed;
        public int totalGameWon;
        public int totalGameLost;
        public int totalChampKills;
        public int totalDeaths;
        public int totalAssists;
        public int totalCS;
        public int totalTurrets;

        public double totalDamage;
        public double totalPhysicalDamage;
        public double totalMagicDamage;
        public double totalGold;

        public double avgGamePlayed;
        public double avgGameWon;
        public double avgGameLost;
        public double avgChampKills;
        public double avgDeaths;
        public double avgAssists;
        public double avgDamage;
        public double avgPhysicalDamage;
        public double avgMagicDamage;
        public double avgCS;
        public double avgGold;
        public double avgTurrets;

        public double winRate { get; set; }

        public ChampionLevel(int level) 
        {
            this.level = level;

            this.count = 0;
            this.totalGamePlayed = 0;
            this.totalGameWon = 0;
            this.totalGameLost = 0;
            this.totalChampKills = 0;
            this.totalDeaths = 0;
            this.totalAssists = 0;
            this.totalDamage = 0;
            this.totalPhysicalDamage = 0;
            this.totalMagicDamage = 0;
            this.totalCS = 0;
            this.totalGold = 0;
            this.totalTurrets = 0;
        }

        public void AddMastery(ChampionMastery mastery) 
        {
            this.count++;
            this.totalGamePlayed += mastery.gamePlayed;
            this.totalGameWon += mastery.gameWon;
            this.totalGameLost += mastery.gameLost;
            this.totalChampKills += mastery.totalChampKills;
            this.totalDeaths += mastery.totalDeaths;
            this.totalAssists += mastery.totalAssists;
            this.totalDamage += mastery.totalDamage;
            this.totalPhysicalDamage += mastery.totalPhysicalDamage;
            this.totalMagicDamage += mastery.totalMagicDamageDealt;
            this.totalCS += mastery.totalCS;
            this.totalGold += mastery.totalGold / 1000.0;
            this.totalTurrets += mastery.totalTurrets;
        }

        public void ProcessChampionLevel()
        {
            this.winRate = this.totalGameWon * 1.0 / this.totalGamePlayed;

            this.avgChampKills = this.totalChampKills * 1.0 / this.totalGamePlayed;
            this.avgDeaths = this.totalDeaths * 1.0 / this.totalGamePlayed;
            this.avgAssists = this.totalAssists * 1.0 / this.totalGamePlayed;
            this.avgCS = this.totalCS * 1.0 / this.totalGamePlayed;
            this.avgGold = this.totalGold * 1.0 / this.totalGamePlayed;
            this.avgTurrets = this.totalTurrets * 1.0 / this.totalGamePlayed;

            this.avgDamage = this.totalDamage * 1.0 / this.totalGamePlayed;
            this.avgPhysicalDamage = this.totalPhysicalDamage * 1.0 / this.totalGamePlayed;
            this.avgMagicDamage = this.totalMagicDamage * 1.0 / this.totalGamePlayed;
        }
    }
}