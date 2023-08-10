using OpenBveApi.Runtime;
using System;
namespace Plugin.Managers
{
    public static class CrashManager
    {
        private static CrashState currentCrashState = CrashState.None;

        internal static void Elapse(ElapseData data) {
            switch (currentCrashState) {
                case CrashState.Minor:
                case CrashState.None:
                    PanelManager.Set(PanelIndices.GlassCracked, 0);
                    break;
                case CrashState.Medium:
                    PanelManager.Set(PanelIndices.GlassCracked, 1);
                    Plugin.SetHeadlightState(data, 2);
                    break;
                case CrashState.Severe:
                    PanelManager.Set(PanelIndices.GlassCracked, 1);
                    PanelManager.Set(PanelIndices.NoPower, 1);
                    Plugin.DirectionLight = IndicatorLight.None;
                    PanelManager.Set(PanelIndices.Indicator, 0);
                    break;
            }

            // Handle collision with front train
            if (data.PrecedingVehicle != null && Config.crashEnabled) {
                if (currentCrashState == CrashState.None && data.PrecedingVehicle.Distance < 0.1 && data.PrecedingVehicle.Distance > -4) {
                    /* Crash Sounds */
                    SoundManager.Play(SoundIndices.Crash, 1.0, 1.0, false);
                    double collisionSpeed = Math.Abs(data.PrecedingVehicle.Speed.KilometersPerHour - data.Vehicle.Speed.KilometersPerHour);

                    if (collisionSpeed > 17) {
                        currentCrashState = CrashState.Severe;
                    } else if (collisionSpeed > 10) {
                        currentCrashState = CrashState.Medium;
                    } else {
                        currentCrashState = CrashState.Minor;
                    }
                }
            }
        }

        internal static void SetCrashState(CrashState newState) {
            currentCrashState = newState;
        }

        internal static bool TrainCrashed() {
            return currentCrashState != CrashState.None;
        }
    }
}
