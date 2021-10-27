# deep-rock-galactic-save-sync
Run to sync the save game files between the Steam and Windows 10/Xbox versions of PC game Deep Rock Galactic.

<h2>Warning</h2>
This app seems to work very well, however it has not been extensively tested - please use at your own risk. I highly recommend backing up the save files for both the Steam and Windows 10/Xbox version of Deep Rock Galactic before using this app.

<h2>Instructions</h2>

1. Clone/download repo and build solution *OR* download the latest [release](https://github.com/cdleveille/DeepRockGalacticSaveSync/releases).
2. Read [this Reddit post](https://www.reddit.com/r/DeepRockGalactic/comments/e7hptr/how_to_transfer_your_steam_save_to_windows_10_and/) for instructions on how to find the save game file locations for both the Steam and Windows 10/Xbox version.
3. Copy/paste the save game locations into a **settings.txt** file in the same directory as the app. It should look something like this:
```
SteamSaveLocation=C:\Program Files (x86)\Steam\steamapps\common\Deep Rock Galactic\FSD\Saved\SaveGames
XboxSaveLocation=C:\Users\cdlev\AppData\Local\Packages\CoffeeStainStudios.DeepRockGalactic_496a1srhmar9w\SystemAppData\wgs\00090000024D69CB_882901006F2042808DB0569531F199CB\F009F38706E44AF7A4318AD729978457
```
4. Launch the version of the game (either Steam or Windows 10/Xbox) that you would like the progress to be synced for, and then close it. Generally, this should probably be whichever version has more progress. This is to ensure that the desired save file will have the more recent Last Modified Date.
5. Run **DeepRockGalacticSaveSync.exe** and review the output: the app will compare the Last Modified Date of each save file, and overwrite the older save file with the more recent one, renaming it appropriately. The older save file will be backed up to a new folder called **DRGSaveSync_Backup** in its original save location, with its filename prepended with a timestamp showing when it was backed up.
