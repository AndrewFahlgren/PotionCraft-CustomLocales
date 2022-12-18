# CustomLocales

CustomLocales is a mod made with BepInEx for use with the Potion Craft game. It allows you to rip localisation from the game to json files, and load custom localisation from json files (overwriting the default localisation with your own text).

## Installation

### BepInEx

- Download the latest BepInEx 5 release from [here](http:/https://github.com/BepInEx/BepInEx/releases/ "here") Note: You need the `BepInEx_64` version for use with Potion Craft
- You can read the installation for BepInEx [here](http:/https://docs.bepinex.dev/articles/user_guide/installation/index.html/ "here")
- Once installed run once to generate the config files and folders

### CustomLocales installation

- Download the latest version from the releases page
- Unzip the folder
- Copy the folder into `/Potion Craft/BepInEx/plugins/`
- Run the game

## Usage
- To rip localisation, open the dev console and run the command `CustomLocales-Rip`. This will rip the localisation to json files located in `/plugins/CustomLocales/Ripped/`.
- To load custom localisation, modify the ripped files with new text, then place them in `/plugins/CustomLocales/Custom` and they will be loaded when you next start the game.
