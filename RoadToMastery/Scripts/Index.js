var currentSelected = data[rankingName] ? rankingName : "totalCount";

var chartSpecs = {
    width: d3.select("#chartContainer")[0][0].getBoundingClientRect().width,
    height: 4500,
    barPadding: 5,
    marginLeft: 250,
    marginRight: 120,
    iconDim: 30,
    championTextWidth: 100
};

var stats;
var ranking;
var rankingNames;
var svg;

var bootstrapDone = false;
var allChampionsStr = "Show All Champions";
var topBotChampionsStr = "Show Top & Bottom 10";

function bootstrap() {
    document.getElementById(currentSelected).checked = true;
    var radioButtons = document.getElementsByName("rankingLists");
    for (var i = 0; i < radioButtons.length; i++) {
        radioButtons[i].onclick = function() {
            currentSelected = document.querySelector('input[name="rankingLists"]:checked').value;
            loadExplanations();
            refreshChart();
        };
    }

    document.getElementById("toggleRankingsDisplayButton").onclick = function () { toggleRankingListDisplay(); };

    refreshChart();
    toggleRankingListDisplay();

    loadExplanations();

    bootstrapDone = true;
}

function refreshChart(reuseData) {
    if (!reuseData) {
        stats = data[currentSelected];
        ranking = rankings[currentSelected];
        if (bootstrapDone && document.getElementById("toggleRankingsDisplayButton").innerHTML === allChampionsStr) {
            getShortRankingList();
        }
    }

    rankingNames = [];
    for (var i = 0; i < ranking.length; i++) {
        rankingNames.push(names[ranking[i]]);
    }

    document.getElementById("rankingChart").innerHTML = "";

    //Create SVG element
    svg = d3.select("#rankingChart")
                .append("svg")
                .attr("id", "chartSvg")
                .attr("width", chartSpecs.width)
                .attr("height", chartSpecs.height);

    loadChart();
}

function loadChart() {
    svg.selectAll("rect")
        .data(stats)
        .enter()
        .append("a")
        .attr("xlink:href", function (d, i) { return "/Home/Champion/" + ranking[i]; })
        .append("rect")
        .attr("x", function (d) {
            var xOffset;
            if (d3.min(stats) > 0) {
                xOffset = 0;
            } else if (d3.max(stats) > 0) {
                var maxOffset = getBarWidth(d3.min(stats));
                xOffset = d > 0 ? maxOffset : maxOffset - getBarWidth(d);
            } else {
                xOffset = getBarWidth(d3.min(stats)) - getBarWidth(d);
            }
            return xOffset + chartSpecs.marginLeft;
        })
        .attr("y", function (d, i) {
            return i * (chartSpecs.height / stats.length);
        })
        .attr("width", function (d) {
            return getBarWidth(d);
        })
        .attr("height", chartSpecs.height / stats.length - chartSpecs.barPadding)
        .attr("class", function (d, i) { return i > 9 && i < stats.length - 10 ? "bar" : "extremeBar" });

    svg.selectAll("barText")
	    .data(stats)
        .enter()
        .append("a")
        .attr("xlink:href", function (d, i) { return "/Home/Champion/" + ranking[i]; })
	    .append("text")
	    .text(function(d) {
	        return getText(d);
	    })
	    .attr("x", function (d, i) {
	        var textPadding = (getText(d).toString().length) * 8 + 5;
	        var xTextOffset;
	        if (d3.min(stats) > 0) {
	            xTextOffset = getBarWidth(d) - textPadding;
	        } else if (d3.max(stats) > 0) {
	            var maxOffset = getBarWidth(d3.min(stats));

	            xTextOffset = d > 0 ? maxOffset + getBarWidth(d) - textPadding : maxOffset - getBarWidth(d) + 5;
	        } else {
	            xTextOffset = getBarWidth(d3.min(stats)) - getBarWidth(d) + 5;
	        }
	        return xTextOffset + chartSpecs.marginLeft;
	    })
	    .attr("y", function(d, i) {
	        return i * chartSpecs.height / stats.length + 18;
	    })
        .attr("class", "barText");

    svg.selectAll("championText")
	    .data(rankingNames)
	    .enter()
        .append("a")
        .attr("xlink:href", function (d, i) { return "/Home/Champion/" + ranking[i]; })
	    .append("text")
	    .text(function (d, i) {
	        return i <= 9 || rankingNames.length > 20 ? i + 1 : data[currentSelected].length - 19 + i;
	    })
	    .attr("x", 0)
	    .attr("y", function (d, i) {
	        return i * chartSpecs.height / rankingNames.length + chartSpecs.height / (rankingNames.length * 2);
	    })
        .attr("class", function (d, i) { return i > 9 && i < stats.length - 10 ? "bar" : "extremeBar" });

    svg.selectAll("championText")
	    .data(rankingNames)
	    .enter()
        .append("a")
        .attr("xlink:href", function (d, i) { return "/Home/Champion/" + ranking[i]; })
	    .append("text")
	    .text(function (d) {
	        return d;
	    })
	    .attr("x", chartSpecs.iconDim + 45)
	    .attr("y", function (d, i) {
	        return i * chartSpecs.height / rankingNames.length + chartSpecs.height / (rankingNames.length * 2);
	    })
        .attr("class", "championText");

    svg.selectAll("championText")
        .data(rankingNames)
	    .enter()
        .append("a")
        .attr("xlink:href", function (d, i) { return "/Home/Champion/" + ranking[i]; })
	    .append("image")
	    .attr("xlink:href", function (d, i) { return imgEndpoint + images[ranking[i]]; })
        .attr("width", chartSpecs.iconDim)
        .attr("height", chartSpecs.iconDim)
	    .attr("x", 40)
	    .attr("y", function (d, i) {
	        return i * chartSpecs.height / rankingNames.length;
	    })
        .attr("class", "championImage");
}

function getBarWidth(d) {
    var maxVal = getOverallMaxVal();
    return Math.abs(d) / maxVal * (chartSpecs.width - chartSpecs.marginLeft - chartSpecs.marginRight);
}

function getText(d) {
    var text = (currentSelected !== "improvementWinRate" && currentSelected !== "lvl5Percentage") ?
        Math.round(d * 1000) / 1000 : Math.round(d * 10000) / 100 + "%";
    return (currentSelected.indexOf("improvement") >= 0 && d > 0 ? "+" : "") + text;
}

function getOverallMaxVal() {
    var maxVal;
    if (d3.min(stats) < 0 && d3.max(stats) < 0) {
        maxVal = Math.abs(d3.min(stats));
    } else if (d3.min(stats) < 0) {
        maxVal = Math.abs(d3.min(stats)) + d3.max(stats);
    } else {
        maxVal = d3.max(stats);
    }
    return maxVal;
}

function toggleRankingListDisplay() {
    var button = document.getElementById("toggleRankingsDisplayButton");
    var currentButtonStr = button.innerHTML;
    var expandList = currentButtonStr === allChampionsStr;
    button.innerHTML = currentButtonStr === allChampionsStr ? topBotChampionsStr : allChampionsStr;

    if (expandList) {
        chartSpecs.height = chartSpecs.height / 20 * rankings[currentSelected].length;
        button.style.backgroundColor = "#c43235";
        refreshChart();
    } else {
        chartSpecs.height = chartSpecs.height / rankings[currentSelected].length * 20;
        button.style.backgroundColor = "#430046";
        getShortRankingList();
        refreshChart(true);
    }
    button.innerHTML = currentButtonStr === allChampionsStr ? topBotChampionsStr : allChampionsStr;
}

function getShortRankingList() {
    var shortStats = [];
    var shortRanking = [];
    for (var i = 0; i < ranking.length; i++) {
        if (i <= 9 || i >= ranking.length - 10) {
            shortStats.push(stats[i]);
            shortRanking.push(ranking[i]);
        }
    }
    stats = shortStats;
    ranking = shortRanking;
}

function loadExplanations() {
    document.getElementById("rankingTitle").innerHTML = rankingTitles[currentSelected];
    document.getElementById("rankingDescription").innerHTML = rankingDescriptions[currentSelected];
}

var rankingTitles = {
    "totalCount": "Everyone loves to try",
    "lvl5Count": "Mastered by most people",
    "lvl5TotalGames": "Another game?",
    "lvl5Percentage": "Kept motivated",
    "improvementWinRate": "More LPs",
    "improvementKills": "Blood thirsty",
    "improvementDeaths": "Gets out alive!",
    "improvementAssists": "Teamwork++",
    "improvementDamage": "Getting the hang of it",
    "lvl5WinRate": "(Probably) free ELO",
    "lvl5AvgKills": "Someone stop them!",
    "lvl5AvgDeaths": "Hardest to hunt down",
    "lvl5AvgAssists": "Everyone honor them!",
    "lvl5AvgDamage": "Tons of damage"
}

var rankingDescriptions = {
    "totalCount": "You have to try a champion first before you master her/him. Here are the counts of players who have at least played the given champion twice in ranked. Ezreal and Lucian are the most popular (by a lot!), whereas Yorick, Urgot, and Mordekaiser would need some love.",
    "lvl5Count": "Now how many players have actually managed to level up to Tier 5 on each champion? This time Lucian is the clear winner, followed by Ezreal, Lee Sin, and Thresh, who also have lots of people giving a try in the first place (see Total Mastery Count). At the bottom of the pack we see the League newcomer Aurelion Sol - hopefully he is on the rise on this metric.",
    "lvl5TotalGames": "As Tier 5 players on a given champion, how many games have they played in total? - Lucian wins again!",
    "lvl5Percentage": "Of all the players who have played the champion (at least twice in ranked), what percentage of them made it to Tier 5? Surprisingly, neither Lucian nor Ezreal is at the top (Ezreal drops out of top 10 even). Instead, we see Riven, Vayne, Lee Sin, and Yasuo - common traits: #SOFUNTOPLAY #BIGPLAYS! Meanwhile, if you already reached Tier 5 with latest champion Aurelion Sol, you definitely earned yourself bragging rights.",
    "improvementWinRate": "Yasuo, Gangplank, Azir, Zed...We would all agree that for these champions great ones can make a huge difference compared to mere average. It's not surprising to see that on average win rate improves more on them when a summoner achieves a higher mastery level. However, note that because we don't have too many data points on Mordekaiser (see Total Mastery Count), his ranking should be viewed with discretion.",
    "improvementKills": "As their masteries grow, players of Rengar, Zed, and Akali learn how to pocket more kills - sounds fairly reasonable. Aurelion Sol players are also learning a few tricks on their way up. At bottom of the list, some supports on a higher mastery level typically choose to yield more kills.",
    "improvementDeaths": "Azir, Talon, Riven are harder to kill as a summoner gains deeper understanding. You bet - for Azir and Talon, a better cast ult can mean saving their lives if not their teammates' as well. Xin Zhao and Galio though, die more often as mastery tier levels up. Maybe players with better skills on them have the confidence to go (too) deep?",
    "improvementAssists": "At the top we have some champions on which players get better at securing kills for the team as they climb the mastery tiers. Akali players: assists are good, you can keep them yourselves (see #Avg. Kills in Mastery Progression). Just don't have enough data on Yorick.",
    "improvementDamage": "Gangplank's barrels are usually all or nothing. When summoners get the hang of barrel timing and position, chances are they suddenly deal a lot more damage than in their previous games. Much higher damage comes with higher ganking success rate and better Rengar players know how to improve that.",
    "lvl5WinRate": "If you are REALLY good (like, Tier 5 good) with the champions placed at the top of this list, you should consider playing them in ranked. Apparently the League hasn't figure out exactly how to stop a great Aurelion Sol yet.",
    "lvl5AvgKills": "Fed Katarina, Talon, and Fizz reaping kills like they don't even care. Supports at the bottom: honor me pls...",
    "lvl5AvgDeaths": "Janna, Rek'Sai, Udyr: Bye! Karthus: Death is when the real game starts. Yasuo: Death is like the wind, always by my side.",
    "lvl5AvgAssists": "Great supports carry the game! We should all be thankful to them. Tryndamere, Master Yi, and Jax not contributing many assists - maybe they are too busy splitting.",
    "lvl5AvgDamage": "Gangplank leading the board by a large margin with his chained barrels, which definitely could be the key to a teamfight or even a game in Tier 5 summoners' hands. Trydamere's extra five seconds helps him placed at second. Sivir with her latest change also makes a strong showing. Sorry Soraka, next time we will do a ranking for healing. :)"
}

bootstrap();