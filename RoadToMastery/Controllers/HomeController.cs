using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RoadToMastery.Analysis;
using System.Threading.Tasks;

namespace RoadToMastery.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string rankingName)
        {
            this.ViewData["rankings"] = Analyzer.rankings;
            this.ViewData["data"] = Analyzer.data;
            this.ViewData["names"] = StaticData.championDict;
            this.ViewData["rankingName"] = rankingName;
            this.ViewData["champImages"] = StaticData.championImgDict;
            this.ViewData["alphabeticalIds"] = Analyzer.alphabeticalIds;
            return View();
        }

        public ActionResult Champion(string id)
        {
            if (id == null) 
            {
                id = "1";
            }
            this.ViewData["champId"] = id;
            this.ViewData["champName"] = StaticData.championDict[id];
            this.ViewData["champImage"] = StaticData.iconUrl + StaticData.championImgDict[id];
            this.ViewData["names"] = StaticData.championDict;
            this.ViewData["alphabeticalIds"] = Analyzer.alphabeticalIds;
            this.ViewData["champImages"] = StaticData.championImgDict;

            Dictionary<string, int> champRankings = new Dictionary<string, int>();
            Dictionary<string, double> champData = new Dictionary<string, double>();
            foreach (KeyValuePair<string, List<int>> pair in Analyzer.rankings) 
            {
                int index = pair.Value.FindIndex(i => i.ToString().Equals(id));
                champRankings.Add(pair.Key, index + 1);
                champData.Add(pair.Key, Analyzer.data[pair.Key].ElementAt(index));
            }
            this.ViewData["champRankings"] = champRankings;
            this.ViewData["champData"] = champData;

            Champion champ = Analyzer.championDict[int.Parse(id)];
            List<double[]> relativeLevelStats = new List<double[]>();
            List<double[]> absoluteLevelStats = new List<double[]>();
            for (int i = 2; i < champ.championLevels.Length; i++)
            {
                ChampionLevel level = champ.championLevels[i];
                relativeLevelStats.Add(new double[9] { 
                    level.winRate / Analyzer.maxLevelStats[0], 
                    level.avgChampKills / Analyzer.maxLevelStats[1], 
                    level.avgDeaths / Analyzer.maxLevelStats[2], 
                    level.avgAssists / Analyzer.maxLevelStats[3], 
                    level.avgPhysicalDamage / Analyzer.maxLevelStats[4], 
                    level.avgMagicDamage / Analyzer.maxLevelStats[5], 
                    level.avgGold / Analyzer.maxLevelStats[6], 
                    level.avgCS / Analyzer.maxLevelStats[7], 
                    level.avgTurrets / Analyzer.maxLevelStats[8] });
                absoluteLevelStats.Add(new double[9] { 
                    level.winRate, 
                    level.avgChampKills, 
                    level.avgDeaths, 
                    level.avgAssists, 
                    level.avgPhysicalDamage, 
                    level.avgMagicDamage, 
                    level.avgGold, 
                    level.avgCS, 
                    level.avgTurrets });
            }
            this.ViewData["relativeLevelStats"] = relativeLevelStats;
            this.ViewData["absoluteLevelStats"] = absoluteLevelStats;

            return View();
        }

        public async Task<JsonResult> GetUserStats(string userName, string championId)
        {
            return Json(await Analyzer.GetSummonerStats(userName, championId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            this.ViewData["names"] = StaticData.championDict;
            this.ViewData["alphabeticalIds"] = Analyzer.alphabeticalIds;
            this.ViewData["champImages"] = StaticData.championImgDict;

            return View();
        }
    }
}