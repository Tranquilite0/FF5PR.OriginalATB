# FF5PR.OriginalATB

A BepInEx plugin which aims to backport the original ATB formula used in the SNES/GBA/PS1 versions of Final Fantasy V to the Pixel Remaster.

## Features

This mod offers multiple options to mix and match ATB behaviors.

- ATB Formula Variants:
    - Original: The original formula where all units' ATB fills at the same rate with the minimum ATB value based on Agility/Weight/Haste/Slow.
    - Original (/w Fill Rate): The original formula, but Haste/Slow changes the rate at which the ATB fills instead of being baked into the minimum value.
    - PixelRemaster: The unchanged ATB formula that the Pixel Remaster uses.
- Enemy Agility Variance:
    - Randomly adds 0, +1, or -1 to enemy agility at the start of battle to add a little variance to the way battles play out (like in the original).
- Advance ATB at Battle Start:
    - Makes it so that the ATB automatically advances at the start of battle until the first unit gets their turn.
- Delay Time at Turn Start:
    - Grants a short period of time after a gets their turn to select an action.
    - Amount delayed is based on battle speed setting.
    - Happens in wait and active mode (also just like in the original).

## Installation

1. If you previously installed BepInEx and have a "mono" folder in the game directory, remove the "mono" and "BepInEx" folders.
1. Download BepInEx 6.0.0-pre.2 IL2CPP build from [here](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip) and extract the content to the game directory.
    - In your steam library you can right click on the game and select `Manage->Browse local files`.
    - (It's where "FINAL FANTASY V.exe" is located).
    - Replace the files if asked.
1. Download the mod from the [Releases](/../../releases/latest) page and extract the content to the game directory.
    - Replace the files if asked.
1. On Steam Deck, add to the Steam launch options : `export WINEDLLOVERRIDES="winhttp=n,b"; %command%`
1. Run the game once to generate the config file, change the config in `(GAME_PATH)\BepInEx\config\FF5PR.OriginalATB.cfg` and restart the game.

## Notes

- This plugin is intended to be used in FF5 PR, but may work in 4 and possibly 6 too (untested, and even if it does work there will be game balance implications).
- ATB behavior is completely deterministic. The original Handled speed ties by randomizing the order it checks units for their turn being ready.
    - I might add in a small randomization factor to start of combat ATB calcs to emulate this behavior.
- I might also eventualy update the ATB calculation to work with discrete ATB "chunks" that tick every 16 frames.