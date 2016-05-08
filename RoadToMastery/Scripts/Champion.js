function bootstrap() {
    initializeRadarChart();
    document.getElementById("getStatsButton").onclick = function () { getUserData(); };
    var imgUrlsComps = championImg.split("/");
    document.getElementById("champLoadingScreen").src = "http://ddragon.leagueoflegends.com/cdn/img/champion/splash/"
        + imgUrlsComps[imgUrlsComps.length - 1].replace(".png", "_0.jpg");
}

bootstrap();

function initializeRadarChart() {
    var chartData = [];
    for (var i = 0; i < relativeLevelStats.length; i++) {
        chartData.push({
            className: "Tier"+(i+3),
            axes: [
                { axis: "Win Rate", value: relativeLevelStats[i][0] },
                { axis: "Avg Kills", value: relativeLevelStats[i][1] },
                { axis: "Avg Deaths", value: relativeLevelStats[i][2] },
                { axis: "Avg Assists", value: relativeLevelStats[i][3] },
                { axis: "Avg Physical Dmg", value: relativeLevelStats[i][4] },
                { axis: "Avg Magic Dmg", value: relativeLevelStats[i][5] },
                { axis: "Avg Gold(K)", value: relativeLevelStats[i][6] },
                { axis: "Avg CS", value: relativeLevelStats[i][7] },
                { axis: "Avg Turrets", value: relativeLevelStats[i][8] }
            ]
        });
    }
    var cfg = {
        factor: 0.9,
        w: 500,
        h: 500,
        tooltipValue: function (d, i) {
            var axis = d.axis;
            var val = d.value;
            val = (Math.log(val) / Math.LN10) / 2;
            for (var j = 0; j < chartData[0].axes.length; j++) {
                if (chartData[0].axes[j].axis === axis) {
                    return Math.round(absoluteLevelStats[i][j]*1000)/1000;
                }
            }
            return Math.round(val*1000)/1000;
        },
        legendText: ['Tier3', 'Tier4', 'Tier5']
    }
    RadarChart.draw("#radarChart", chartData, cfg);
}

var errorMsg = "<span class=\"glyphicon glyphicon-remove-sign\"></span>Oops...Invalid summoner name, or champion not yet played by this summoner this season."

function getUserData() {
    var userName = document.getElementById("getStatsInput").value;
    $.ajax({
        url: "/Home/GetUserStats",
        data: {
            userName: userName,
            championId: championId
        },
        dataType: "json",
        success: function (data) {
            if (data[0] === -1) {
                document.getElementById("getStatsMessage").innerHTML = errorMsg;
                populateInvalidUserData();
            } else {
                document.getElementById("getStatsMessage").innerHTML = "<span class=\"glyphicon glyphicon-ok-sign\"></span>Summoner data retrieved successfully.";
                var userStats = document.getElementsByClassName("summonerStats");
                var userComparisons = document.getElementsByClassName("userComparison");
                var lvl5Stats = document.getElementsByClassName("lvl5Stats");
                var lvl5Ranking = document.getElementsByClassName("lvl5Ranking");
                for (var i = 0; i < userStats.length; i++) {
                    userStats[i].innerHTML = Math.round(data[i] * 1000) / 1000;
                    var betterStats = data[i] < absoluteLevelStats[absoluteLevelStats.length - 1][i];
                    if (i === 2) {
                        betterStats = !betterStats;
                    }
                    if (betterStats) {
                        userComparisons[i].className = "userComparison " + " glyphicon glyphicon-arrow-down";
                        lvl5Stats[i].style.color = "#430046";
                        lvl5Ranking[i].style.color = "#430046";
                    } else {
                        userComparisons[i].className = "userComparison " + " glyphicon glyphicon-arrow-up";
                        lvl5Stats[i].style.color = "#c43235";
                        lvl5Ranking[i].style.color = "#c43235";
                    }
                }
            }
        },
        error: function () {
            document.getElementById("getStatsMessage").innerHTML = errorMsg;
            populateInvalidUserData();
        }
    });
}

function populateInvalidUserData() {
    var userStats = document.getElementsByClassName("summonerStats");
    var userComparisons = document.getElementsByClassName("userComparison");
    var lvl5Stats = document.getElementsByClassName("lvl5Stats");
    var lvl5Ranking = document.getElementsByClassName("lvl5Ranking");
    for (var i = 0; i < userStats.length; i++) {
        userStats[i].innerHTML = "--";
        userComparisons[i].className = "userComparison " + " glyphicon glyphicon-question-sign";
        lvl5Stats[i].style.color = "#06000a";
        lvl5Ranking[i].style.color = "#06000a";
    }
}