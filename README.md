# RoadToMastery

http://roadtomastery.azurewebsites.net/

### Homepage
![alt tag](https://raw.githubusercontent.com/jieshan/RoadToMastery/master/RoadToMastery/Content/img/homeScreenshot.png)

### Champion page
![alt tag](https://raw.githubusercontent.com/jieshan/RoadToMastery/master/RoadToMastery/Content/img/championScreenshot.png)

### Overview
There are two projects in this repo:
1. LoLData - responsible for collecting the data from Riot API
2. RoadToMastery - the web app itself
	
### LoLData 

LoLData is a C# console application that pulls the data from Riot API. It uses HttpClient to make web calls, and once response is returned JSON.Net is applied to unbox the json. Although it is currently only tried on NA server and only to get players and masteries, I tried to make the program general enough for all servers and other kinds of tasks. My original intention of LoLData was to make it customizable as a multi-purpose data collection standalone for all my future experiments with Riot API, but it's not there yet at the moment and I will continue on it.

The app features some important features for resilient and efficient data collection:

1. Throughout adoption of async threads: whenever a web call is made or a data processing/recording task is needed, a new async thread is spawned, picking up the work without hindering the main thread moving on with subsequent queued jobs.

2. Thread pool sharing: To prevent from overwhelming number of threading bringing down the machine, and considering the different specs of machines, a thread pool of customizable limited size (default maxServersThreads is 400) is implemented. Once this limit is hit, the app will wait until new threads are allowed.

3. Web calls management: when a 429 is returned by Riot, all threads globally will honor that timeout and wait for the designated amount of time. A web call request failure is also retried two more times before being discarded.

#### Workflow

a. Open the .csproj in Visual Studio

b. Create a folder PrivateData under the project, and put an api key in a file ```ApiKey.txt``` in this folder

c. In the Main() method of ```DataCollection.cs```, comment out all Suites except for ```Suite 1: Scan a server from scratch```. This will get Plat+ player Ids in a ```NAplayers.txt``` file.

d. Now comment out all Suites except for ```Suite 4: Collecting champion mastery data from a list of players```. This will read the players id txt file, find their ranked data, and put together an ```NAChampionMasteryplayers.txt``` in the format of:
```
String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                    playerId, championId, champPoints, champLevel, rankedPlayed, rankedWon, rankedLost,
                    totalChampKills, totalDeaths, totalAssists, totalDamage, totalPhysicalDamage, totalMagicDamageDealt,
                    totalCS, totalGold, totalTurrets)
```

The other "Suites" in ```DataCollection.cs``` are for my experiment purpose only and are not related to this project.

In step (c) above, what's happenning behind the scene is:

1. Retrieve all Chanllenger league player Ids, put them in a queue (one api call)

2. In parallel, for each player id in the queue, get all his/her recent ranked game data (one api call per player)

3. In parallel, get all player id of all teammates and opponents in each ranked game (one api call per game)

4. In parallel, if the teammate/opponent is in a "qualified" league (Plat+) and has not been encountered in the system, append the id to the queue in (1) (one api call per teammate/opponent)		

5. Repeat until queue is repleted

The whole process will give around 180K+ players.

In step (d) of the workflow, for each "qualified" player Id, record the ranked stats and for each champion that's been played at least twice, check the mastery data. The end result on the record is like a "join" between player ranked stats and champion mastery data. There is some replication in this step and step (c), namely getting ranked stats, but because I think get playerId is an important procedure that could be needed in other future workflows as well, I wanted to keep it as a standalone suite/module for clearer logic. There could be some refactoring I can do to improve the performance.

Because of time limit, I couldn't go through the entire process, only processed 60K+ players leading to about 350K+ mastery data.


### RoadToMastery

RoadToMastery is an ASP.NET MVC 5 web app. To run it:

1. Open it in Visual Studio
2. Put the ```ApiKey.txt``` and ```NAChampionMasteryplayers.txt``` from LoLData in the folder DataCollection
3. Click Run

The first time it might take 20-30s to bootstrap because of data processing. But this only happens on the very first web call to the app after deployment is done. All subsequent calls to the same server should get response immediately, as can be shown by ```http://roadtomastery.azurewebsites.net/``` as an example.

For mastery progression, I used linear regression on all the mastery data points of a given champion, instead of simply taking the average of each tier respectively, and then look at the delta between these averages. This is because I believe taking the average 1) will condense the data in an unnecessary way, losing a lot of information 2) could be biased since the number of data points are not evenly distributed across different tiers. In contrast, doing linear regression on all data points regardless of the tier could capture the trend across the board, maximizing utilization of data.

Also because I found that Tier 1 and 2 data points tend to be scarce, on the champion page radar chart I only included Tier 3-5, so that comparison of averages make more sense.

As mentioned on the About page, the web app used bootstrap for layout, and d3.js for svg charts. If I had more time, on homepage I would like to animate the bars' transition when switching rankings.
