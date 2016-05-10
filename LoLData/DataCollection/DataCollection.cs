﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLData.DataCollection
{
    public class DataCollection 
    {
        public static string[] serverList = {"NA"};

        public static string apiKey;

        public static string apiKeyPath = "PrivateData\\ApiKey.txt";

        public static string gameType = "ONEFORALL_5x5";

        public static string inFilePrefixTemplate = "{0}players";

        public static string outFilePrefixTemplate = "{0}ChampionMastery";

        public static void Main() 
        {
            if (DataCollection.apiKey == null)
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                StreamReader reader = new StreamReader(@Path.Combine(currentDirectory, DataCollection.apiKeyPath));
                DataCollection.apiKey = reader.ReadLine();
                reader.Close();
            }

            System.Diagnostics.Debug.WriteLine("============================");
            System.Diagnostics.Debug.WriteLine("Data Collection Starts");

            for (int i = 0; i < DataCollection.serverList.Length; i++ )
            {
                // TODO: Query servers asynchronously
                ServerManager serverManager = new ServerManager(serverList[0], DataCollection.apiKey, 
                    String.Format(DataCollection.inFilePrefixTemplate, serverList[0]),
                    String.Format(DataCollection.outFilePrefixTemplate, serverList[0]),
                    DataCollection.gameType);
                try
                {
                    // Suite 1: Scan a server from scratch
                    // DataCollection.SeedScanSuite(serverManager);

                    // Suite 2: Scan a type of games given existing players from a file
                    // DataCollection.ScanGamesFromPlayersSuite(serverManager);

                    // Suite 3: Collecting detailed game data from existing list of gameIds
                    //DataCollection.ScanGamesFromGameIdsSuite(serverManager);

                    // Suite 4: Collecting champion mastery data from a list of players
                    DataCollection.ScanChampionMastryFromPlayersSuite(serverManager);
                }
                finally
                {
                    serverManager.CloseAllFiles();
                    System.Diagnostics.Debug.WriteLine("****************");
                    System.Diagnostics.Debug.WriteLine(String.Format("Error in collecting {0} server.", serverList[0]));
                }              
            }

            System.Diagnostics.Debug.WriteLine("============================");
        }

        // Operation Suites
        public static void SeedScanSuite(ServerManager serverManager)
        {
            serverManager.InitiateNewSeedScan();
            serverManager.ProcessAll();
        }

        public static void ScanGamesFromPlayersSuite(ServerManager serverManager) 
        {
            serverManager.loadDataFromFile();
            serverManager.ProcessAll(ServerManager.Subject.Player, false);
        }

        public static void ScanGamesFromGameIdsSuite(ServerManager serverManager)
        {
            serverManager.loadDataFromFile(ServerManager.Subject.Game);
            serverManager.ProcessAll(ServerManager.Subject.Game);
        }

        public static void ScanChampionMastryFromPlayersSuite(ServerManager serverManager)
        {
            serverManager.loadDataFromFile();
            serverManager.ProcessAll(ServerManager.Subject.PlayerMastery, false);
        }
    }
}
