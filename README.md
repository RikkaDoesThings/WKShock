```
Hella finicky btw, remove the "Shock Credentials" section of the config and only use "Shock Provider"
PiShock might not work.
```

# About
An OpenShock integration mod for White Knuckle.

Created by [Xironix/SonexFuzz](https://github.com/SonexFuzz/) (originally for PiShock), with OpenShock support added by [RikkaDoesThings](https://github.com/RikkaDoesThings).
Currently supported triggers:
* Damage
* Death

This build supports OpenShock. PiShock mode is not implemented; selecting it logs an error and no events are sent.

# Configurations
Install the mod, then edit the com.xironix.wkshock.cfg file in the config folder. Configuration in game is currently not implemented.

## OpenShock setup
Set `Use OpenShock = true`, then fill in:
* API Base URL (normally `https://api.openshock.app`)
* API Token (a valid OpenShock API token is required)
* Device IDs (comma-separated OpenShock device UUIDs; numeric IDs are not accepted)

You can find device IDs in the OpenShock web app or API response.
The mod sends commands through OpenShock's v2 shocker control endpoint. Requests include the required `User-Agent` and `Open-Shock-Token` headers.

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
