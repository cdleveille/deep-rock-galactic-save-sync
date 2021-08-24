using System;
using System.IO;

namespace DeepRockGalacticSaveSync
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileInfo settingsFile = GetSettingsFile();

                string pathSteam = GetValueForKeyInSettingsFile(settingsFile, "SteamSaveLocation");
                string pathXbox = GetValueForKeyInSettingsFile(settingsFile, "XboxSaveLocation");

                if (pathSteam == "" || pathXbox == "")
                {
                    throw new Exception("Steam and/or Xbox save file locations have not been set in settings.txt file!");
                }

                FileInfo steamFile = GetSteamSaveFile(pathSteam);
                FileInfo xboxFile = GetXboxSaveFile(pathXbox);

                CompareDatesOfSaveFilesAndReplace(steamFile, xboxFile, pathSteam, pathXbox);

                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR! " + e.Message + "\n");
                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        private static FileInfo GetSettingsFile()
        {
            Console.WriteLine("Looking for settings.txt file in current directory...");

            FileInfo[] files = GetAllFilesInCurrentDirectory();

            if (files.Length < 1)
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

            Console.WriteLine("Found settings.txt file.\n");
            return files[0];
        }

        private static FileInfo[] GetAllFilesInCurrentDirectory()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("settings.txt");
            return files;
        }

        private static string GetValueForKeyInSettingsFile(FileInfo settingsFile, string key)
        {
            System.IO.StreamReader settingsFileReader = new System.IO.StreamReader(settingsFile.FullName);
            string line, value = "";
            string[] splitLine;

            while ((line = settingsFileReader.ReadLine()) != null)
            {
                if (line.ToUpper().StartsWith(key.ToUpper()))
                {
                    splitLine = line.Split("=");
                    value = splitLine[1].Trim();
                    break;
                }
            }

            settingsFileReader.Close();
            return value;
        }

        private static FileInfo GetSteamSaveFile(string path)
        {
            Console.WriteLine("Looking for Steam save file at location specified in settings.txt...");

            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            foreach (FileInfo file in files)
            {
                if (file.Name.EndsWith("_Player.sav"))
                {
                    Console.WriteLine("Found Steam save file.\n");
                    return file;
                }
            }

            throw new Exception("Steam save file not found!");
        }

        private static FileInfo GetXboxSaveFile(string path)
        {
            Console.WriteLine("Looking for Xbox save file at location specified in settings.txt...");

            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            foreach (FileInfo file in files)
            {
                if (file.Extension == "")
                {
                    Console.WriteLine("Found Xbox save file.\n");
                    return file;
                }
            }

            throw new Exception("Xbox save file not found!");
        }

        private static void CompareDatesOfSaveFilesAndReplace(FileInfo steamFile, FileInfo xboxFile, string pathSteam, string pathXbox)
        {
            DateTime now = DateTime.Now;
            string timestamp = now.Year.ToString() + "_" + now.Month.ToString() + "_" + now.Day.ToString() + "_" +
                now.Hour.ToString() + "-" + now.Minute.ToString() + "-" + now.Second.ToString() + "_____";

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
                Console.WriteLine("Moved Xbox save file to backup location:\n" + pathXboxBackup + "\\" + xboxFile.Name + "\n");

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
                Console.WriteLine("Moved Steam save file to backup location:\n" + pathSteamBackup + "\\" + steamFile.Name + "\n");

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
        }
    }
}
