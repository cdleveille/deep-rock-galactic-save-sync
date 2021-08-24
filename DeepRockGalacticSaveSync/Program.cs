﻿using System;
using System.IO;

namespace DeepRockGalacticSaveSync
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileInfo settingsFile = null;
                FileInfo steamFile = null;
                FileInfo xboxFile = null;

                string path = System.IO.Directory.GetCurrentDirectory();
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("settings.txt");

                if (files.Length == 0)
                {
                    throw new Exception("No settings.txt file found in current directory! Please create a settings.txt file in the same directory " +
                        "as the app that specifies the respective paths of the Steam and Xbox save files on separate lines. For example:\n\n" +
                        "SteamSaveLocation=C:\\Program Files (x86)\\Steam\\steamapps\\common\\Deep Rock Galactic\\FSD\\Saved\\SaveGames\n" +
                        "XboxSaveLocation=C:\\Users\\cdlev\\AppData\\Local\\Packages\\CoffeeStainStudios.DeepRockGalactic\n");
                }
                else if (files.Length > 1)
                {
                    throw new Exception("More than one settings.txt file found in current directory!");
                }

                settingsFile = files[0];
                System.IO.StreamReader settingsFileReader = new System.IO.StreamReader(settingsFile.FullName);
                string line;
                string[] splitLine;
                string pathSteam = "";
                string pathXbox = "";

                while ((line = settingsFileReader.ReadLine()) != null)
                {
                    if (line.StartsWith("SteamSaveLocation"))
                    {
                        splitLine = line.Split("=");
                        pathSteam = splitLine[1].Trim();
                    }
                    else if (line.StartsWith("XboxSaveLocation"))
                    {
                        splitLine = line.Split("=");
                        pathXbox = splitLine[1].Trim();
                    }
                }

                settingsFileReader.Close();

                if (pathSteam == "" || pathXbox == "")
                {
                    throw new Exception("Steam and/or Xbox save file locations have not been set in settings.config file!");
                }

                DirectoryInfo dirSteam = new DirectoryInfo(pathSteam);
                DirectoryInfo dirXbox = new DirectoryInfo(pathXbox);

                FileInfo[] steamFiles = dirSteam.GetFiles();
                FileInfo[] xboxFiles = dirXbox.GetFiles();

                foreach (FileInfo file in steamFiles)
                {
                    if (file.Name.EndsWith("_Player.sav"))
                    {
                        steamFile = file;
                    }
                }

                foreach (FileInfo file in xboxFiles)
                {
                    if (file.Extension == "")
                    {
                        xboxFile = file;
                    }
                }

                if (steamFile == null)
                {
                    throw new Exception("Steam save file not found!");
                }
                else
                {
                    Console.WriteLine("Found Steam save file:\n" + steamFile.FullName + "\n");
                }

                if (xboxFile == null)
                {
                    throw new Exception("Xbox save file not found!");
                }
                else
                {
                    Console.WriteLine("Found Xbox save file:\n" + xboxFile.FullName + "\n");
                }

                DateTime dt = DateTime.Now;
                string timestamp = dt.Year.ToString() + "_" + dt.Month.ToString() + "_" + dt.Day.ToString() + "_" +
                    dt.Hour.ToString() + "-" + dt.Minute.ToString() + "-" + dt.Second.ToString() + "_____";

                // Backup and remove Xbox file, copy Steam file to Xbox save location and rename to match name of Xbox file
                if (steamFile.LastWriteTime > xboxFile.LastWriteTime)
                {
                    Console.WriteLine("Steam save file has more recent Last Modified Date:");
                    Console.WriteLine("Steam: " + steamFile.LastWriteTime);
                    Console.WriteLine("Xbox: " + xboxFile.LastWriteTime + "\n");

                    // Create backup directory
                    string pathXboxBackup = pathXbox + "\\DRGSaveSync_Backup";
                    Directory.CreateDirectory(pathXboxBackup);

                    string xboxFileNamePreBackup = xboxFile.Name;

                    // Move Xbox file to backup directory
                    xboxFile.MoveTo(pathXboxBackup + "\\" + timestamp + xboxFile.Name);
                    Console.WriteLine("Moved Xbox save file to backup location:\n" + pathXboxBackup + "\\" + timestamp + xboxFile.Name + "\n");

                    // Copy Steam file to Xbox save location and rename to match name of Xbox file
                    steamFile.CopyTo(pathXbox + "\\" + xboxFileNamePreBackup);
                    Console.WriteLine("Copied Steam file to Xbox save location and renamed to match name of Xbox save file:\n" + pathXbox + "\\" + xboxFileNamePreBackup + "\n");
                }

                // Backup and remove Steam file, copy Xbox file to Steam save location and rename to match name of Steam file
                else if (steamFile.LastWriteTime < xboxFile.LastWriteTime)
                {
                    Console.WriteLine("Xbox save file has more recent Last Modified Date:");
                    Console.WriteLine("Xbox: " + xboxFile.LastWriteTime);
                    Console.WriteLine("Steam: " + steamFile.LastWriteTime + "\n");

                    // Create backup directory
                    string pathSteamBackup = pathSteam + "\\DRGSaveSync_Backup";
                    Directory.CreateDirectory(pathSteamBackup);

                    string steamFileNamePreBackup = steamFile.Name;

                    // Move Steam file to backup directory
                    steamFile.MoveTo(pathSteamBackup + "\\" + timestamp + steamFile.Name);
                    Console.WriteLine("Moved Steam save file to backup location:\n" + pathSteamBackup + "\\" + timestamp + steamFile.Name + "\n");

                    // Copy Xbox file to Steam save location and rename to match name of Steam file
                    xboxFile.CopyTo(pathSteam + "\\" + steamFileNamePreBackup);
                    Console.WriteLine("Copied Xbox file to Steam save location and renamed to match name of Steam save file:\n" + pathSteam + "\\" + steamFileNamePreBackup + "\n");
                }

                else
                {
                    Console.WriteLine("Last Modified Date of Steam and Xbox save file is identical! Nothing to do.");
                    Console.WriteLine("Steam: " + steamFile.LastWriteTime);
                    Console.WriteLine("Xbox: " + xboxFile.LastWriteTime + "\n");
                }

                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR! " + e.Message);
                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }
    }
}
