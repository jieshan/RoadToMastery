using Newtonsoft.Json.Linq;
using RoadToMastery.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RoadToMastery.Analysis
{
    public class Analyzer
    {
        public static Dictionary<int, Champion> championDict = new Dictionary<int, Champion>();
        public static Dictionary<string, List<int>> rankings = new Dictionary<string, List<int>>();
        public static Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
        public static double[] maxLevelStats = new double[9]{ 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static List<int> alphabeticalIds = new List<int>();

        private static string apiKey = null;
        private static string summonerNameQuery = "https://na.api.pvp.net/api/lol/na/v1.4/summoner/by-name/{0}?api_key={1}";
        private static string rankedQuery = "https://na.api.pvp.net/api/lol/na/v1.3/stats/by-summoner/{0}/ranked?season=SEASON2016&api_key={1}";

        public static List<Champion> champions = new List<Champion>();
        public static void ProcessData()
        {
            StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Data/NAChampionMasteryplayers.txt"));
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] fields = line.Split(',');
                int champId = int.Parse(fields[1]);
                int index = Analyzer.ChampionRegistered(champId);
                if (index >= 0)
                {
                    champions.ElementAt(index).AddMastery(new ChampionMastery(fields));
                }
                else
                {
                    Champion champion = new Champion(champId, StaticData.championDict[champId.ToString()]);
                    champion.masteries.Add(new ChampionMastery(fields));
                    champions.Add(champion);
                }
            }
            ProcessAllChampions();

            foreach (Champion champion in Analyzer.champions)
            {
                championDict.Add(champion.championId, champion);
                for (int i = 2; i < champion.championLevels.Length; i++) 
                {
                    ChampionLevel level = champion.championLevels[i];
                    Analyzer.maxLevelStats[0] = Math.Max(level.winRate, Analyzer.maxLevelStats[0]);
                    Analyzer.maxLevelStats[1] = Math.Max(level.avgChampKills, Analyzer.maxLevelStats[1]);
                    Analyzer.maxLevelStats[2] = Math.Max(level.avgDeaths, Analyzer.maxLevelStats[2]);
                    Analyzer.maxLevelStats[3] = Math.Max(level.avgAssists, Analyzer.maxLevelStats[3]);
                    Analyzer.maxLevelStats[4] = Math.Max(level.avgPhysicalDamage, Analyzer.maxLevelStats[4]);
                    Analyzer.maxLevelStats[5] = Math.Max(level.avgMagicDamage, Analyzer.maxLevelStats[5]);
                    Analyzer.maxLevelStats[6] = Math.Max(level.avgGold, Analyzer.maxLevelStats[6]);
                    Analyzer.maxLevelStats[7] = Math.Max(level.avgCS, Analyzer.maxLevelStats[7]);
                    Analyzer.maxLevelStats[8] = Math.Max(level.avgTurrets, Analyzer.maxLevelStats[8]);
                }
            }

            List<int> ranking = new List<int>();
            List<double> stats = new List<double>();
            // Sort by total mastery count descending
            champions.Sort((x, y) => (x.masteryCount * -1).CompareTo(y.masteryCount * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.masteryCount);
            });
            rankings.Add("totalCount", ranking);
            data.Add("totalCount", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by level 5 mastery count descending
            champions.Sort((x, y) => (x.championLevels[4].count * -1).CompareTo(y.championLevels[4].count * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].count);
            });
            rankings.Add("lvl5Count", ranking);
            data.Add("lvl5Count", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by level 5 mastery percentage descending
            champions.Sort((x, y) => (x.lvl5Percentage * -1).CompareTo(y.lvl5Percentage * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.lvl5Percentage);
            });
            rankings.Add("lvl5Percentage", ranking);
            data.Add("lvl5Percentage", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by champion points per game descending
            champions.Sort((x, y) => (x.championLevels[4].totalGamePlayed * -1).CompareTo(y.championLevels[4].totalGamePlayed * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].totalGamePlayed);
            });
            rankings.Add("lvl5TotalGames", ranking);
            data.Add("lvl5TotalGames", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by improvement of win rate descending
            champions.Sort((x, y) => (x.improvementWinRate * -1).CompareTo(y.improvementWinRate * -1));
            champions.ForEach((champion) => { 
                ranking.Add(champion.championId);
                stats.Add(champion.improvementWinRate);
            });
            rankings.Add("improvementWinRate", ranking);
            data.Add("improvementWinRate", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by win rate of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].winRate * -1).CompareTo(y.championLevels[4].winRate * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].winRate);
            });
            rankings.Add("lvl5WinRate", ranking);
            data.Add("lvl5WinRate", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by improvement of kills descending
            champions.Sort((x, y) => (x.improvementKills * -1).CompareTo(y.improvementKills * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.improvementKills);
            });
            rankings.Add("improvementKills", ranking);
            data.Add("improvementKills", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average kills of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgChampKills * -1).CompareTo(y.championLevels[4].avgChampKills * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgChampKills);
            });
            rankings.Add("lvl5AvgKills", ranking);
            data.Add("lvl5AvgKills", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by improvement of deaths ascending
            champions.Sort((x, y) => (x.improvementDeaths).CompareTo(y.improvementDeaths));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.improvementDeaths);
            });
            rankings.Add("improvementDeaths", ranking);
            data.Add("improvementDeaths", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average deaths of level 5 ascending
            champions.Sort((x, y) => (x.championLevels[4].avgDeaths).CompareTo(y.championLevels[4].avgDeaths));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgDeaths);
            });
            rankings.Add("lvl5AvgDeaths", ranking);
            data.Add("lvl5AvgDeaths", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by improvement of assists descending
            champions.Sort((x, y) => (x.improvementAssists * -1).CompareTo(y.improvementAssists * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.improvementAssists);
            });
            rankings.Add("improvementAssists", ranking);
            data.Add("improvementAssists", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average assists of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgAssists * -1).CompareTo(y.championLevels[4].avgAssists * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgAssists);
            });
            rankings.Add("lvl5AvgAssists", ranking);
            data.Add("lvl5AvgAssists", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by improvement of damage descending
            champions.Sort((x, y) => (x.improvementDamage * -1).CompareTo(y.improvementDamage * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.improvementDamage);
            });
            rankings.Add("improvementDamage", ranking);
            data.Add("improvementDamage", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average damage of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgDamage * -1).CompareTo(y.championLevels[4].avgDamage * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgDamage);
            });
            rankings.Add("lvl5AvgDamage", ranking);
            data.Add("lvl5AvgDamage", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average damage of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgPhysicalDamage * -1).CompareTo(y.championLevels[4].avgPhysicalDamage * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgPhysicalDamage);
            });
            rankings.Add("lvl5AvgPhysicalDmg", ranking);
            data.Add("lvl5AvgPhysicalDmg", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average damage of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgMagicDamage * -1).CompareTo(y.championLevels[4].avgMagicDamage * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgMagicDamage);
            });
            rankings.Add("lvl5AvgMagicDmg", ranking);
            data.Add("lvl5AvgMagicDmg", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average damage of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgGold * -1).CompareTo(y.championLevels[4].avgGold * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgGold);
            });
            rankings.Add("lvl5AvgGold", ranking);
            data.Add("lvl5AvgGold", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average damage of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgCS * -1).CompareTo(y.championLevels[4].avgCS * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgCS);
            });
            rankings.Add("lvl5AvgCS", ranking);
            data.Add("lvl5AvgCS", stats);

            ranking = new List<int>();
            stats = new List<double>();
            // Sort by average damage of level 5 descending
            champions.Sort((x, y) => (x.championLevels[4].avgTurrets * -1).CompareTo(y.championLevels[4].avgTurrets * -1));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
                stats.Add(champion.championLevels[4].avgTurrets);
            });
            rankings.Add("lvl5AvgTurrets", ranking);
            data.Add("lvl5AvgTurrets", stats);

            ranking = new List<int>();
            // Sort by name descending
            champions.Sort((x, y) => (x.championName).CompareTo(y.championName));
            champions.ForEach((champion) =>
            {
                ranking.Add(champion.championId);
            });
            Analyzer.alphabeticalIds = ranking;

            foreach (Champion champion in Analyzer.champions)
            {
                string printOut = "";
                for (int i = 0; i < 5; i++)
                {
                    printOut += String.Format("Level {0}: {1}; ", i, champion.championLevels[i].avgDamage);
                }
                System.Diagnostics.Debug.WriteLine("{0}({1}): {2} {3}", champion.championName, champion.championId, champion.improvementWinRate, printOut);
                //System.Diagnostics.Debug.WriteLine("{0}({1}): {2} {3}", champion.championName, champion.championId, champion.learningCurve, champion.rSquared);
            }           
        }

        public static async Task<double[]> GetSummonerStats(string summonerName, string championId) 
        {
            double[] invalidData = new double[9]{ -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            if (Analyzer.apiKey == null)
            {
                StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Data/ApiKey.txt"));
                Analyzer.apiKey = reader.ReadLine();
                reader.Close();
            }
            // TODO: Separate class for making api calls
            HttpClient client = new HttpClient();
            HttpResponseMessage httpResponse = await client.GetAsync(String.Format(Analyzer.summonerNameQuery, summonerName, Analyzer.apiKey)).ConfigureAwait(continueOnCapturedContext: false);
            int statusCode = (int)httpResponse.StatusCode;
            // TODO: Handle failed responses
            if (statusCode != 200) {
                return invalidData;
            }
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(responseContent);
            string summonerId = (string)result[summonerName.ToLower()]["id"];

            httpResponse = await client.GetAsync(String.Format(Analyzer.rankedQuery, summonerId, Analyzer.apiKey)).ConfigureAwait(continueOnCapturedContext: false);
            statusCode = (int)httpResponse.StatusCode;
            // TODO: Handle failed responses
            if (statusCode != 200)
            {
                return invalidData;
            }
            responseContent = await httpResponse.Content.ReadAsStringAsync();
            result = JObject.Parse(responseContent);
            JArray championArray = (JArray)result["champions"];
            JObject championRankedStats = null;
            for (int i = 0; i < championArray.Count; i++) {
                if (((string)((JObject)championArray[i])["id"]).Equals(championId)) {
                    championRankedStats = (JObject)championArray[i]["stats"];
                    break;
                }
            }
            if (championRankedStats != null) {
                double gameCount = Double.Parse((string)championRankedStats["totalSessionsPlayed"]);
                double gameWon = Double.Parse((string)championRankedStats["totalSessionsWon"]);
                double totalKills = Double.Parse((string)championRankedStats["totalChampionKills"]);
                double totalDeaths = Double.Parse((string)championRankedStats["totalDeathsPerSession"]);
                double totalAssists = Double.Parse((string)championRankedStats["totalAssists"]);
                double totalPhysicalDmg = Double.Parse((string)championRankedStats["totalPhysicalDamageDealt"]);
                double totalMagicDmg = Double.Parse((string)championRankedStats["totalMagicDamageDealt"]);
                double totalGold = Double.Parse((string)championRankedStats["totalGoldEarned"]);
                double totalCS = Double.Parse((string)championRankedStats["totalMinionKills"]);
                double totalTurrets = Double.Parse((string)championRankedStats["totalTurretsKilled"]);

                return new double[9] {
                    gameWon / gameCount,
                    totalKills / gameCount,
                    totalDeaths / gameCount,
                    totalAssists / gameCount,
                    totalPhysicalDmg / gameCount,
                    totalMagicDmg / gameCount,
                    totalGold / (1000 * gameCount),
                    totalCS / gameCount,
                    totalTurrets / gameCount
                };
            }
            return invalidData;
        }
  
        private static int ChampionRegistered(int champId)
        {
            for (int i = 0; i < Analyzer.champions.Count; i++)
            {
                if (Analyzer.champions.ElementAt(i).championId == champId)
                {
                    return i;
                }
            }
            return -1;
        }

        private static void ProcessAllChampions() 
        {
            foreach (Champion champion in Analyzer.champions)
            {
                champion.ProcessChampion();
            }
        }
    }
}
