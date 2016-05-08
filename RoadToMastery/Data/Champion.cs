using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadToMastery.Analysis
{
    public class Champion
    {
        public int championId;
        public string championName;
        public List<ChampionMastery> masteries;

        public int masteryCount;
        public int totalChampionPts;
        public int totalGamesPlayed;

        public double lvl5Percentage;

        public double improvementWinRate;
        public double improvementKills;
        public double improvementDeaths;
        public double improvementAssists;
        public double improvementDamage;


        public ChampionLevel[] championLevels;

        public Champion(int championId, string championName) 
        {
            this.championId = championId;
            this.championName = championName;
            this.masteries = new List<ChampionMastery>();
            this.championLevels = new ChampionLevel[5]{
                new ChampionLevel(1),
                new ChampionLevel(2),
                new ChampionLevel(3),
                new ChampionLevel(4),
                new ChampionLevel(5)
            };
            this.totalChampionPts = 0;
            this.totalGamesPlayed = 0;
        }

        public void AddMastery(ChampionMastery mastery) 
        {
            this.masteries.Add(mastery);
        }

        public void ProcessChampion() 
        {
            this.ComputeCumulativeStats();
            this.masteryCount = this.masteries.Count;
            this.lvl5Percentage = this.championLevels[4].count * 1.0 / this.masteryCount;

            this.improvementWinRate = this.ComputeImprovement("winRate", false);
            this.improvementKills = this.ComputeImprovement("totalChampKills", false);
            this.improvementDeaths = this.ComputeImprovement("totalDeaths", false);
            this.improvementAssists = this.ComputeImprovement("totalAssists", false);
            this.improvementDamage = this.ComputeImprovement("totalDamage", false);
        }

        private void ComputeCumulativeStats() 
        {
            foreach (ChampionMastery mastery in this.masteries)
            {
                int champLvl = mastery.masteryLevel;
                this.totalChampionPts += mastery.masteryPoint;
                this.totalGamesPlayed += mastery.gamePlayed;
                this.championLevels[champLvl - 1].AddMastery(mastery);
            }
            foreach(ChampionLevel champLevel in this.championLevels)
            {
                champLevel.ProcessChampionLevel();
            }
        }

        private double ComputeImprovement(string propertyName, bool useLevel = true) 
        {
            double[] masteryLevels = null;
            double[] yVals = null;
            if (!useLevel)
            {
                masteryLevels = new double[this.championLevels[2].count + this.championLevels[3].count + this.championLevels[4].count];
                yVals = new double[this.championLevels[2].count + this.championLevels[3].count + this.championLevels[4].count];
                int j = 0;
                for (int i = 0; i < this.masteries.Count; i++)
                {
                    if (this.masteries.ElementAt(i).masteryLevel >= 3)
                    {
                        masteryLevels[j] = this.masteries.ElementAt(i).masteryLevel;
                        yVals[j] = Convert.ToDouble(this.masteries.ElementAt(i).GetType().GetProperty(propertyName).GetValue(this.masteries.ElementAt(i), null));
                        if (!propertyName.Equals("winRate"))
                        {
                            yVals[j] = yVals[j] / Convert.ToDouble(this.masteries.ElementAt(i).GetType().GetProperty("gamePlayed").GetValue(this.masteries.ElementAt(i), null));
                        }
                        j++;
                    }
                }
            }
            else
            {
                masteryLevels = new double[]{ 3, 4, 5 };
                yVals = new double[3]{
                    Convert.ToDouble(this.championLevels[2].GetType().GetProperty(propertyName).GetValue(this.championLevels[2], null)),
                    Convert.ToDouble(this.championLevels[3].GetType().GetProperty(propertyName).GetValue(this.championLevels[2], null)),
                    Convert.ToDouble(this.championLevels[4].GetType().GetProperty(propertyName).GetValue(this.championLevels[2], null))
                };
            }

            double rSquared;
            double yintercept;
            double slope;
            Utils.LinearRegression(masteryLevels, yVals, 0, yVals.Length, out rSquared, out yintercept, out slope);

            //double[] predictedWinRates = (double[])masteryLevels.Clone();
            //for (int i = 0; i < predictedWinRates.Length; i++ )
            //{
            //    predictedWinRates[i] = predictedWinRates[i] * this.improvementWinRate + yintercept;
            //}
            //this.pValueWinRate = Utils.TTest(yVals, predictedWinRates);

            return slope;
        }
    }
}
