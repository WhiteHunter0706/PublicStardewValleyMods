﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public abstract class Items
    {
        protected Dictionary<int, SObject> items;
        protected ConfigOptions Config;
        public enum FarmTypes
        {
            Normal = 0,
            River = 1,
            Forest = 2,
            Hilltop = 3,
            Wilderness = 4
        };

        abstract protected void AddItems();

        public Items()
        {
            Config = ModEntry.Config;
            items = new Dictionary<int, SObject>();

            AddItems();
        }

        protected void AddFish(int parentSheetIndex)
        {
            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            switch (data[parentSheetIndex].Split('/')[7])
            {
                case "sunny":
                    if (!Game1.isRaining || Config.ShowAllFishFromCurrentSeason)
                    {
                        AddOneCommonObject(parentSheetIndex);
                    }
                    break;
                case "rainy":
                    if (Game1.isRaining || Config.ShowAllFishFromCurrentSeason)
                    {
                        AddOneCommonObject(parentSheetIndex);
                    }
                    break;
                case "both":
                    AddOneCommonObject(parentSheetIndex);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Adds a crop if there is sufficient time to grow it before the 
        /// season ends.
        /// </summary>
        /// <param name="key"></param>
        protected void AddCrop(int key)
        {
            CropConversion cropConverter = new CropConversion();
            if (Utilities.IsThereEnoughTimeToGrowSeeds(cropConverter.CropToSeedIndex(key)))
            {
                AddOneCommonObject(key);
            }
        }

        protected void AddOneCommonObject(int parentSheetIndex)
        {
            AddCommonObject(parentSheetIndex, 1);
        }

        protected void AddCommonObject(int parentSheetIndex, int stackSize)
        {
            if (!items.ContainsKey(parentSheetIndex))
            {
                items.Add(parentSheetIndex, new SObject(parentSheetIndex, stackSize));
            }
        }

        private enum SeasonIndex
        {
            Spring = 4,
            Summer = 5,
            Fall = 6,
            Winter = 7
        };

        private int SeasonNameToIndex(string season)
        {
            switch (season)
            {
                case "spring":
                    return (int)SeasonIndex.Spring;
                case "summer":
                    return (int)SeasonIndex.Summer;
                case "fall":
                    return (int)SeasonIndex.Fall;
                case "winter":
                    return (int)SeasonIndex.Winter;
                default:
                    return -1;
            }
        }

        private void ParseLocationData()
        {
            Dictionary<string, string> LocationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            int seasonIndex = SeasonNameToIndex(Game1.currentSeason);

            foreach (KeyValuePair<string, string> data in LocationData)
            {
                string[] seasonalFish = data.Value.Split('/')[seasonIndex].Split(' ');
                for(int i = 0; i < seasonalFish.Length; ++i)
                {
                    if(i % 2 == 0)
                    {
                        //Its a parent sheet index
                        AddFish(int.Parse(seasonalFish[i]));
                    }
                }
            }
        }



    }
}
