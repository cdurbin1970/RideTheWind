RideTheWind by SilverHawk1 - <a href="https://github.com/cdurbin1970/RideTheWind">GitHub</a>
<br><br>
This mod allows you to tweak boat and other settings for better QOL. It was inspired by a couple of different mods that were available.<br>
<br>
<br>
Like most mods, this mod requires BepInEx in order to run.<br>
Drop it into your {install_dir}/bepinex/plugins directory<br>
The config file is located in {install_dir}/bepinex/config<br>
<br>
<b>Config options</b>
<br>
Turn Debug on (default false). Leave this on false as it will spam your log and console.<br>
DebugMode = false<br>
<br>
Enable the mod (default true)<br> 
ModEnabled = true<br>
<br>
Automatically check for newer versions? (default false)<br>
VersionCheck = false<br>
<br>
When safe settings are enabled, the wind speed is only increased for Longships up<br>
to a maximum of 6. When false, it will work with any ship or raft and no max value. At 10,<br>
your ship is VERY difficult to steer. REMEMBER: the faster you go, the more damage your boat <br>
will take when you hit something. At high values, it is quite likely your boat will be destroyed<br>
in the middle of the ocean.<br>
SafeSettings = true<br>
<br>
Should we enable the wind behind you portion of the mod?<br>
WindEnabled = true<br>
<br>
Should we enable the sail calculation portion of the mod?<br>
SailEnabled = true<br>
<br>
Should we enable the rudder enhancements?<br>
RudderEnabled = true<br>
<br>
Should we enable the wind intensity portion of the mod?<br>
WindIntensityEnabled = false<br>
<br>
This settings controls the amount of force applied to the rudder control when moving backwards.<br>
RudderBackwardForce = 2<br>
<br>
This setting controls how fast the rudder turns.<br>
RudderSpeed = 2<br>
<br>
This mod works by patching the sailsize calculation used by Valheim. It does not change the<br>
visible sail size, just the calculation. You can set the multiplier for both half sail and<br> 
full sail. If SafeSettings is true, then it only works for the Longship<br>
HalfSailWindSpeed = 2<br>
FullSailWindSpeed = 4<br>
<br>
Set a specific angle for the wind. Normally it is directly behind you.<br>
UseThisAngle = false<br>
<br>
If the above item is true, set the angle you wish to use.<br>
TheAngle = 0<br>
<br>
This setting allows you to set the wind intensity manually. Valid<br>
settings are from 0.00 to 1.00. Please undserstand when you change<br>
the wind intensity, it affects everything. At values close to 1, the waves<br>
will become very large like in a storm and with increased sail size, your boat<br>
can become difficult to steer. Use with caution!<br>  
WindIntensity=0.05<br> 
<br>
Set the max camera zoom when in a boat (MAX 50)<br>
MaxCameraZoom = 6<br>
<br>
Size of the map reveal when exploring new areas<br>
ExploreSize = 100