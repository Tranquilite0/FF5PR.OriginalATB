# FF5PR.OriginalATB

A BepInEx plugin which aims to backport the original ATB formula to Final Fantasy V Pixel Remaster.

## Features

TODO: This

## Installation

1. If you previously installed BepInEx and have a "mono" folder in the game directory, remove the "mono" and "BepInEx" folders.
1. Download BepInEx 6.0.0-pre.2 IL2CPP build from [here](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip) and extract the content to the game directory.
	a. In your steam library you can right click on the game and select `Manage->Browse local files`.
	a. (It's where "FINAL FANTASY V.exe" is located).
	a. Replace the files if asked.
1. Download the mod from the [Releases](/../../releases/latest) page and extract the content to the game directory.
	a. Replace the files if asked.
1. On Steam Deck, add to the Steam launch options : `export WINEDLLOVERRIDES="winhttp=n,b"; %command%`
1. Run the game once to generate the config file, change the config in `(GAME_PATH)\BepInEx\config\FF5PR.OriginalATB.cfg` and restart the game.

## Notes

TODO: This