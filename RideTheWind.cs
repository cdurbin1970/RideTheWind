using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace RideTheWind {
  [BepInPlugin("silverhawk1.RideTheWind", "RideTheWind", "0.0.3.1217")]
    public class BepInExPlugin : BaseUnityPlugin {
        private static BepInExPlugin _context;
        private static ConfigEntry<bool> _isDebug;
        private static ConfigEntry<bool> _modEnabled;
        private static ConfigEntry<bool> _useAngle;
        private static ConfigEntry<float> _angle;
        private static ConfigEntry<bool> _safety; 
        private static ConfigEntry<float> _speedFullSail;
        private static ConfigEntry<float> _speedHalfSail;
     
        private void Awake() {
        /*
            Safe settings is that the wind speed calculation will only work on the Longship and there is a MAX speed of 6
        */
            BepInExPlugin._context = this;
            BepInExPlugin._isDebug = this.Config.Bind("General", "DebugMode", false, "Print debug messages. Not normally enabled as it will spam your console and log file.");
            BepInExPlugin._modEnabled = this.Config.Bind("General", "ModEnabled", true, "Enable this mod");
            BepInExPlugin._useAngle = this.Config.Bind("General", "UseThisAngle", false, "Want to set a specific angle?");
            BepInExPlugin._angle = this.Config.Bind("General", "TheAngle", 0.0f, "Wind angle relative to ship");
            BepInExPlugin._safety = this.Config.Bind("General", "SafeSettings", true, "Use safe settings? When safe is true, will only work with Longship and 6 is max speed.");
            BepInExPlugin._speedFullSail = this.Config.Bind("General", "FullSailWindSpeed", 1.0f, "Wind speed multiplier for full sail (1-6 MAX)");
            BepInExPlugin._speedHalfSail = this.Config.Bind("General", "HalfSailWindSpeed", 1.0f, "Wind speed multiplier for half sail (1-6 MAX)");
            if (!BepInExPlugin._modEnabled.Value) {
                return;
            }
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
        
        [HarmonyPatch(typeof (EnvMan), "Awake")]
        private static class EnvManAwakePatch {
            private static void Postfix() => BepInExPlugin.PrintOut("RideTheWind Loaded");
        }

        [HarmonyPatch(typeof (EnvMan), "SetTargetWind")]
        private class TargetWindPatch {
            private static void Prefix(ref Vector3 dir) {
                var localShip = Ship.GetLocalShip();
                if (localShip == null) {
                    if (BepInExPlugin._isDebug.Value) {
                        MyDebug("No Ship Found");
                    }
                    return;
                }
                if (localShip.GetSpeedSetting() != Ship.Speed.Half && localShip.GetSpeedSetting() != Ship.Speed.Full) {
                    if (BepInExPlugin._isDebug.Value) {
                        MyDebug("Ship too slow");
                    }
                    return;
                }
                var forward = localShip.transform.forward;
                if (BepInExPlugin._useAngle.Value) {
                    var num = (float) (((double) Vector3.SignedAngle(Vector3.forward, forward, Vector3.up) + BepInExPlugin._angle.Value) * 3.1415927410125732 / 180.0);
                    dir.x = Mathf.Sin(num);
                    dir.z = Mathf.Cos(num);
                }
                else {
                    dir.x = forward.x;
                    dir.z = forward.z;
                }
            }
        }

        [HarmonyPatch(typeof (Ship), "GetSailForce")]
        private class ShipSailSize {
            private static void Prefix(ref float sailSize) {
                var localShip = Ship.GetLocalShip();
                if (localShip == null) {
                    if (BepInExPlugin._isDebug.Value) {
                        MyDebug("No ship found");
                    }
                    return;
                }
                else {
                    if (BepInExPlugin._isDebug.Value) {
                        MyDebug("Name: " + localShip.name);
                    }
                }
                if (BepInExPlugin._safety.Value) {
                    if (localShip.name == "VikingShip(Clone)") {
                        if (BepInExPlugin._isDebug.Value) {
                            MyDebug("Found Viking ship");
                        }
                        if (localShip.GetSpeedSetting() == Ship.Speed.Half) {
                            if (BepInExPlugin._speedHalfSail.Value > 6) {
                                sailSize *= 6;
                            }
                            else if (BepInExPlugin._speedHalfSail.Value < 1) {
                                sailSize *= 1;
                            }
                            else {
                                sailSize *= BepInExPlugin._speedHalfSail.Value;
                            }
                        }
                        else if (localShip.GetSpeedSetting() == Ship.Speed.Full) {
                            if (BepInExPlugin._speedFullSail.Value > 6) {
                                sailSize *= 6;
                            }
                            else if (BepInExPlugin._speedFullSail.Value < 1) {
                                sailSize *= 1;
                            }
                            else {
                                sailSize *= BepInExPlugin._speedFullSail.Value;
                            }
                        }
                    }
                }
                else {
                    if (localShip.GetSpeedSetting() == Ship.Speed.Half) {
                        sailSize *= BepInExPlugin._speedHalfSail.Value;
                    }
                    else if(localShip.GetSpeedSetting() == Ship.Speed.Full) {
                        sailSize *= BepInExPlugin._speedFullSail.Value;
                    }
                }
            }
        }
        private static void PrintOut(string log) {
            var instance = Console.instance;
            if (instance == null) {
                return;
            }
            var str = "[RideTheWind]" + ": " + log;
            instance.Print(str);
        }
        public static void MyDebug(string str = "", bool pref = true) {
            Debug.Log((pref ? typeof(BepInExPlugin).Namespace + " " : "") + str);
        }
    }
}
