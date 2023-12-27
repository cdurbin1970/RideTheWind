using System.Globalization;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.Net;

namespace RideTheWind {
  [BepInPlugin("silverhawk1.RideTheWind", "RideTheWind", ThisVersion)]
    public class RideTheWindPlugin : BaseUnityPlugin {

        private static RideTheWindPlugin _context;
        private static ConfigEntry<bool> _isDebug;
        private static ConfigEntry<bool> _modEnabled;
        private static ConfigEntry<bool> _checkForNewVersion;
        private static ConfigEntry<bool> _windEnabled;
        private static ConfigEntry<bool> _sailEnabled;
        private static ConfigEntry<bool> _useWindIntensity;
        private static ConfigEntry<bool> _rudderEnabled;
        private static ConfigEntry<bool> _useAngle;
        private static ConfigEntry<float> _angle;
        private static ConfigEntry<bool> _safety; 
        private static ConfigEntry<float> _speedFullSail;
        private static ConfigEntry<float> _speedHalfSail;
        private static ConfigEntry<float> _windIntensity;
        private static ConfigEntry<float> _rudderBackwardForce;
        private static ConfigEntry<float> _rudderSpeed;
        private static ConfigEntry<float> _maxCameraZoom;
        private static ConfigEntry<float> _exploreSize;
        private static string _newestVersion = "";
        private const string ThisVersion = "0.0.7.1227";
        private const string Url = "https://api.github.com/repos/cdurbin1970/RideTheWind/releases/latest";
        public new static ManualLogSource Logger { get; private set; }

        private void Awake() {
            Logger = base.Logger;
            Logger.LogInfo($"RideTheWind version {ThisVersion} loaded.");
        /*
            Safe settings is that the wind speed calculation will only work on the Longship and there is a MAX speed of 6
        */
            RideTheWindPlugin._context = this;
            RideTheWindPlugin._isDebug = this.Config.Bind("General", "DebugMode", false, "Print debug messages. Not normally enabled as it will spam your console and log file.");
            RideTheWindPlugin._modEnabled = this.Config.Bind("General", "ModEnabled", true, "Enable this mod?");
            RideTheWindPlugin._checkForNewVersion = this.Config.Bind("General", "VersionCheck", false, "Automatically check for new version?");
            RideTheWindPlugin._safety = this.Config.Bind("General", "SafeSettings", true, "Use safe settings? When safe is true, will only work with Longship and 6 is max speed.");
            RideTheWindPlugin._windEnabled = this.Config.Bind("General", "WindEnabled", true, "Enable the wind behind you?");
            RideTheWindPlugin._sailEnabled = this.Config.Bind("General", "SailEnabled", true, "Enable the wind speed multiplier?");
            RideTheWindPlugin._rudderEnabled = this.Config.Bind("General", "RudderEnabled", true, "Enable the rudder changes?");
            RideTheWindPlugin._useWindIntensity = this.Config.Bind("General", "WindIntensityEnabled", false, "Enable the wind intensity?");
            RideTheWindPlugin._useAngle = this.Config.Bind("Wind", "UseThisAngle", false, "Want to set a specific angle?");
            RideTheWindPlugin._angle = this.Config.Bind("Wind", "TheAngle", 0.0f, "Wind angle relative to ship");
            RideTheWindPlugin._windIntensity = this.Config.Bind("Wind", "WindIntensity", 0.05f, "Set the wind intensity (0.00-1 MAX");
            RideTheWindPlugin._speedHalfSail = this.Config.Bind("Sail", "HalfSailWindSpeed", 2.0f, "Wind speed multiplier for half sail (1-6 MAX)");
            RideTheWindPlugin._speedFullSail = this.Config.Bind("Sail", "FullSailWindSpeed", 4.0f, "Wind speed multiplier for full sail (1-6 MAX)");
            RideTheWindPlugin._rudderBackwardForce = this.Config.Bind("Rudder", "RudderBackwardForce", 2.0f, "Rudder backward force multiplier");
            RideTheWindPlugin._rudderSpeed = this.Config.Bind("Rudder", "RudderSpeed", 2.0f, "Rudder speed multiplier");
            RideTheWindPlugin._maxCameraZoom = this.Config.Bind("Camera", "MaxCameraZoom", 6.0f, "Max camera zoom when in a boat (MAX 50)");
            RideTheWindPlugin._exploreSize = this.Config.Bind("Map", "ExploreSize", 100.0f, "Size of the map reveal when exploring new areas");

            if (!RideTheWindPlugin._modEnabled.Value) {
                Logger.LogWarning("RideTheWind not enabled via config.");
                return;
            }

            if (_checkForNewVersion.Value) {
                Logger.LogInfo("RideTheWind checking for new version.");
                switch (CheckForNewVersion()) {
                    case 1000:
                        Logger.LogWarning("There is a newer version available!");
                        break;
                    case 1001:
                        Logger.LogWarning("Problem parsing the version information");
                        break;
                    case 1002:
                        Logger.LogInfo("RideTheWind is up to date.");
                        break;
                    case 1003:
                        Logger.LogWarning("Newer version than is publicly available!?!");
                        break;
                    default:
                        Logger.LogWarning("Unknown error checking version");
                        break;
                }
            }
            else {
                Logger.LogWarning("Version checking disabled via config.");
            }
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private static int CheckForNewVersion() {
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent: RideTheWind");
            try {
                var reply = client.DownloadString(Url);
                _newestVersion = reply.Substring(reply.IndexOf("tag_name") + 10, 12).Trim('"').Trim();
            }
            catch {
                Logger.LogWarning("Problem retrieving the version information.");
                _newestVersion = "Unknown";
            }
            if (System.Version.TryParse(_newestVersion, out var newVersion)) {
                if (System.Version.TryParse(ThisVersion, out var currentVersion)) {
                    if (currentVersion < newVersion) {
                        return 1000;//"There is a newer version available!";
                    }
                    else if (currentVersion > newVersion) {
                        return 1003; //"Newer version than is publicly available"
                    }
                }
                else {
                    Logger.LogWarning("Couldn't parse current version");
                    return 1001;//"Problem parsing the version information";
                }
            }
            else {
                Logger.LogWarning("Couldn't parse newest version.");
                if (_newestVersion != ThisVersion) {
                    return 1001;//"Problem parsing the version information";
                }
            }
            return 1002;//"RideTheWind is up to date."
        }

        [HarmonyPatch(typeof(Minimap), "Start")]
        public static class RevealSizePatch {
            private static void Prefix(Minimap __instance) {
                if (!RideTheWindPlugin._modEnabled.Value) {
                    return;
                }
                __instance.m_exploreRadius = Mathf.Clamp(RideTheWindPlugin._exploreSize.Value, 1, 10000);
            }
        }

        [HarmonyPatch(typeof(GameCamera), "UpdateCamera")]
        private class MaxBoatCameraZoom {
            private static void Prefix(GameCamera __instance) {
                if (!RideTheWindPlugin._modEnabled.Value) {
                    return;
                }
                __instance.m_maxDistanceBoat = Mathf.Clamp(RideTheWindPlugin._maxCameraZoom.Value, 1f, 50f); 
            }
        }

        [HarmonyPatch(typeof (EnvMan), "SetTargetWind")]
        private class TargetWindPatch {
            private static void Prefix(ref Vector3 dir, ref float intensity) {
                if (!RideTheWindPlugin._sailEnabled.Value) {
                    return;
                }
                var localShip = Ship.GetLocalShip();
                if (localShip == null) {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Wind: No Ship Found");
                    }
                    return;
                }
                if (localShip.GetSpeedSetting() != Ship.Speed.Half && localShip.GetSpeedSetting() != Ship.Speed.Full) {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Wind: Ship too slow");
                    }
                    return;
                }
                var forward = localShip.transform.forward;
                if (RideTheWindPlugin._isDebug.Value) {
                    Logger.LogDebug(forward.ToString());
                }
                if (RideTheWindPlugin._useAngle.Value) {
                    var num = (float) (((double) Vector3.SignedAngle(Vector3.forward, forward, Vector3.up) + RideTheWindPlugin._angle.Value) * 3.1415927410125732 / 180.0);
                    dir.x = Mathf.Sin(num);
                    dir.z = Mathf.Cos(num);
                }
                else {
                    dir.x = forward.x;
                    dir.z = forward.z;
                }
                if (RideTheWindPlugin._useWindIntensity.Value) {
                    intensity = Mathf.Clamp(RideTheWindPlugin._windIntensity.Value, 0.00f, 1.00f);
                }
            }
        }

        [HarmonyPatch(typeof (Ship), "GetSailForce")]
        private class ShipSailSize {
            private static void Prefix(ref float sailSize) {
                if (!RideTheWindPlugin._windEnabled.Value) {
                    return;
                }
                var localShip = Ship.GetLocalShip();
                if (localShip == null) {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Sail: No ship found");
                    }
                    return;
                }
                else {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Sail: Name- " + localShip.name);
                    }
                }
                if (RideTheWindPlugin._safety.Value) {
                    if (localShip.name == "VikingShip(Clone)") {
                        if (RideTheWindPlugin._isDebug.Value) {
                            Logger.LogDebug("Sail: Found Viking ship");
                        }
                        if (localShip.GetSpeedSetting() == Ship.Speed.Half) {
                            sailSize *= Mathf.Clamp(RideTheWindPlugin._speedHalfSail.Value, 1f, 6f);
                        }
                        else if (localShip.GetSpeedSetting() == Ship.Speed.Full) {
                            sailSize *= Mathf.Clamp(RideTheWindPlugin._speedFullSail.Value, 1f, 6f);
                        }
                    }
                }
                else {
                    if (localShip.GetSpeedSetting() == Ship.Speed.Half) {
                        sailSize *= Mathf.Clamp(RideTheWindPlugin._speedHalfSail.Value, 1f, 99f);
                    }
                    else if(localShip.GetSpeedSetting() == Ship.Speed.Full) {
                        sailSize *= Mathf.Clamp(RideTheWindPlugin._speedFullSail.Value, 1f, 99f);
                    }
                }
                if (RideTheWindPlugin._isDebug.Value) {
                    Logger.LogDebug("Sail size: " + sailSize.ToString(CultureInfo.InvariantCulture));
                    Logger.LogDebug("Sail force: " + localShip.m_sailForceFactor.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        [HarmonyPatch(typeof(Ship), "Start")]
        private class ShipRudderBackwardForce {
            private static void Prefix(Ship __instance) {
                if (!RideTheWindPlugin._rudderEnabled.Value) {
                    return;
                }
                
                if (__instance == null) {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Backward Rudder: No ship found");
                    }
                    return;
                }
                else {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Backward Rudder: Name- " + __instance.name);
                    }
                }

                if (RideTheWindPlugin._safety.Value) {
                    if (__instance.name == "VikingShip(Clone)") {
                        __instance.m_backwardForce = RideTheWindPlugin._rudderBackwardForce.Value;
                    }
                }
                else {
                    __instance.m_backwardForce = RideTheWindPlugin._rudderBackwardForce.Value;
                }
            }
        }

        [HarmonyPatch(typeof(Ship), "Start")]
        private class ShipRudderSpeed {
            private static void Prefix(Ship __instance) {
                if (!RideTheWindPlugin._rudderEnabled.Value) {
                    return;
                }
                if (__instance == null) {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Rudder Speed: No ship found");
                    }
                    return;
                }
                else {
                    if (RideTheWindPlugin._isDebug.Value) {
                        Logger.LogDebug("Rudder Speed: Name- " + __instance.name);
                    }
                }
                if (RideTheWindPlugin._safety.Value) {
                    if (__instance.name == "VikingShip(Clone)") {
                        __instance.m_rudderSpeed = RideTheWindPlugin._rudderSpeed.Value;
                    }
                }
                else {
                    __instance.m_rudderSpeed = RideTheWindPlugin._rudderSpeed.Value;
                }
            }
        }
    }
}
