RideTheWind by SilverHawk1 - https://github.com/cdurbin1970/RideTheWind

This mod allows you to tweak boat settings for better QOL. It was inspired by a couple of different mods that were available.

Like most mods, this mod requires BepInEx in order to run. 
Drop it into your {install_dir}/bepinex/plugins directory
The config file is located in {install_dir}/bepinex/config

Config options

Turn Debug on (default false). Leave this on false as it will spam your log and console.
DebugMode = false

Enable the mod (default true) 
ModEnabled = true

Automatically check for newer versions? (default false)
VersionCheck = false

When safe settings are enabled, the wind speed is only increased for Longships up
to a maximum of 6. When false, it will work with any ship or raft and no max value. At 10,
your ship is VERY difficult to steer. REMEMBER: the faster you go, the more damage your boat 
will take when you hit something. At high values, it is quite likely your boat will be destroyed
in the middle of the ocean.
SafeSettings = true

Should we enable the wind behind you portion of the mod?
WindEnabled = true

Should we enable the sail calculation portion of the mod?
SailEnabled = true

Should we enable the rudder enhancements?
RudderEnabled = true

Should we enable the wind intensity portion of the mod?
WindIntensityEnabled = false

This settings controls the amount of force applied to the rudder control when moving backwards.
RudderBackwardForce = 2

This setting controls how fast the rudder turns.
RudderSpeed = 2

This setting does not change the visible sail size, just the calculation. You can set the multiplier 
for both half sail and full sail. If SafeSettings is true, then it only works for the Longship
HalfSailWindSpeed = 2
FullSailWindSpeed = 4

Set a specific angle for the wind. Normally it is directly behind you.
UseThisAngle = false

If the above item is true, set the angle you wish to use.
TheAngle = 0

This setting allows you to set the wind intensity manually. Valid
settings are from 0.00 to 1.00. Please undserstand when you change
the wind intensity, it affects everything. At values close to 1, the waves
will become very large like in a storm and with increased sail size, your boat
can become difficult to steer. Use with caution!  
WindIntensity = 0.05 

Set the max camera zoom when in a boat (MAX 50)
MaxCameraZoom = 6 

Size of the map reveal when exploring new areas
ExploreSize = 100