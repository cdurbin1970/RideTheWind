RideTheWind by SilverHawk1

This is my first publicly released mod for Valheim. It allows you to travel
by boat very quickly. It was inspired by a couple of different mods that were available.

It serves two functions: <br>
    1) The wind is always at your back when you are on a boat and at the helm. <br>
    2) You can increase the maximum speed of your boat at half sail and full sail.<br>

Like most mods, this mod requires BepInEx in order to run.

Config options

Turn Debug on (default false). Leave this on false as it will spam your log and console.<br>
DebugMode = false

Enable the mod (default true)<br> 
ModEnabled = true

Set a specific angle for the wind. Normally it is directly behind you.<br>
UseThisAngle = false

If the above item is true, set the angle you wish to use.<br>
TheAngle = 0

When safe settings are enabled, the wind speed is only increased for Longships up
to a maximum of 6. When false, it will work with any ship or raft and no max value. At 10,
your ship is VERY difficult to steer. REMEMBER: the faster you go, the more damage your boat 
will take when you hit something. At high values, it is quite likely your boat will be destroyed
in the middle of the ocean.<br>
SafeSettings = true

This mod works by patching the sailsize calculation used by Valheim. It does not change the
visible sail size, just the calculation. You can set the multiplier for both half sail and 
full sail. If SafeSettings is true, then it only works for the Longship<br>
FullSailWindSpeed = 1<br>
HalfSailWindSpeed = 1

