using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LazerCraft
{
    public class SinglePlayerSave
    {
        public static int xTotalLevels = 3;
        public static int yTotalLevels = 3;
        public static int totalTapes = 5;
        public static int totalBosses = 1;
        //DateTime.Now.ToString();
        public int save;
        public string lastPlayed;
        public int directionEntered;
        public int xLocation, yLocation;
        public int difficulty;
        public bool[,] areasDiscovered;
        public bool[] tapesDiscovered;
        public bool[] bossesKilled;
        public int areasDiscoveredInt;
        public int tapesDiscoveredInt;
        public int bossesKilledInt;
        public SinglePlayerSave(int save)
        {
            this.save = save;
            areasDiscovered = new bool[xTotalLevels, yTotalLevels];
            tapesDiscovered = new bool[totalTapes];
            bossesKilled = new bool[totalBosses];
        }
        public void Load()
        {
            string path = @"Content/SinglePlayerSaves/Save"+save.ToString()+".data";
            if (!File.Exists(path))
            {
                lastPlayed = "Never";
                directionEntered = 0;
                xLocation = 0;
                yLocation = 0;
                difficulty = 0;
                for (int xIndex = 0; xIndex < xTotalLevels; xIndex += 1)
                {
                    for (int yIndex = 0; yIndex < yTotalLevels; yIndex += 1)
                    {
                        areasDiscovered[xIndex, yIndex] = false;
                    }
                }
                for (int index = 0; index < totalTapes; index += 1)
                {
                    tapesDiscovered[index] = false;
                }
                for (int index = 0; index < totalBosses; index += 1)
                {
                    bossesKilled[index] = false;
                }
                areasDiscoveredInt = 0;
                tapesDiscoveredInt = 0;
                bossesKilledInt = 0;
                Save(true);
            }
            else
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    lastPlayed = reader.ReadString();
                    directionEntered = reader.ReadInt32();
                    xLocation = reader.ReadInt32();
                    yLocation = reader.ReadInt32();
                    difficulty = reader.ReadInt32();

                    areasDiscoveredInt = 0;
                    tapesDiscoveredInt = 0;
                    bossesKilledInt = 0;

                    for (int xIndex = 0; xIndex < xTotalLevels; xIndex += 1)
                    {
                        for (int yIndex = 0; yIndex < yTotalLevels; yIndex += 1)
                        {
                            areasDiscovered[xIndex, yIndex] = reader.ReadBoolean();
                            if (areasDiscovered[xIndex, yIndex] == true)
                                areasDiscoveredInt += 1;
                        }
                    }
                    for (int index = 0; index < totalTapes; index += 1)
                    {
                        tapesDiscovered[index] = reader.ReadBoolean();
                        if (tapesDiscovered[index] == true)
                            tapesDiscoveredInt += 1;
                    }
                    for (int index = 0; index < totalBosses; index += 1)
                    {
                        bossesKilled[index] = reader.ReadBoolean();
                        if (bossesKilled[index] == true)
                            bossesKilledInt += 1;
                    }
                }
            }
        }
        public void Save(bool first)
        {
            string path = @"Content/SinglePlayerSaves/Save" + save.ToString() + ".data";
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                if (first == false)
                    lastPlayed= DateTime.Now.ToString();
                writer.Write(lastPlayed);
                writer.Write(directionEntered);
                writer.Write(xLocation);
                writer.Write(yLocation);
                writer.Write(difficulty);
                for (int xIndex = 0; xIndex < xTotalLevels; xIndex += 1)
                {
                    for (int yIndex = 0; yIndex < yTotalLevels; yIndex += 1)
                    {
                        writer.Write(areasDiscovered[xIndex, yIndex]);
                    }
                }
                for (int index = 0; index < totalTapes; index += 1)
                {
                    writer.Write(tapesDiscovered[index]);
                }
                for (int index = 0; index < totalBosses; index += 1)
                {
                    writer.Write(bossesKilled[index]);
                }
            }
        }
    }
}
