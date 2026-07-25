# About
A PiShock/OpenShock integration mod for White Knuckle.\
The mod uses the [PiShockLibrary](https://github.com/MxPuffin/PiShockLibrary) and some code from [REPOShock](https://github.com/MxPuffin/REPOShock) by [MxPuffin](https://github.com/MxPuffin).
Created by [Xironix/SonexFuzz](https://github.com/SonexFuzz/), OpenShock support added by [RikkaDoesThings](https://github.com/RikkaDoesThings).

Currently supported triggers:
* Damage
* Death

The mod can send haptic events through either PiShock or OpenShock, depending on the configuration in the plugin config file.

# Configurations
Install the mod, then edit the com.xironix.wkshock.cfg file in the config folder. Configuration in game is currently not implemented.

## Choose your provider
Set the following value in the config:
* Use OpenShock = true to use OpenShock
* Use OpenShock = false to use PiShock

## PiShock setup
If you are using PiShock, fill in:
* Username
* APIKey
* ShareCodes

You can get your PiShock username and API key from the PiShock account page, and your share codes from the PiShock control page.

## OpenShock setup
If you are using OpenShock, fill in:
* API Base URL (at the time of writing: https://api.openshock.org)
* API Token (optional, only if your instance requires authentication)
* Device IDs (comma-separated list of OpenShock device IDs)

You can find device IDs in the OpenShock web app or API response.

## Gameplay settings
The rest of the settings control when the plugin sends events:
* Enable Shock
* Shock Intensity
* Shock Duration
* Enable Vibrate
* Vibrate Intensity
* Vibrate Duration
* Enable Death Shock
* Death Shock Intensity
* Death Shock Duration
