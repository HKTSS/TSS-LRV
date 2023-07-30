using OpenBveApi.Runtime;
using OpenBveApi.Colors;
using System;
using System.Threading.Tasks;

namespace Plugin {
    /// <summary>The interface to be implemented by the plugin.</summary>
    public class AIManager {
        private const int AI_TIMEOUT = 4;
        private static IndicatorLight Indicator = IndicatorLight.None;
        private static bool AIEnabled;
        private static double lastAICall;
        private static double currentTime;

        /// <summary>Is called when the plugin should initialize, reinitialize or jumping stations.</summary>
        public static void ResetLRV(ResetType type) {
            AIEnabled = false;
        }

        /// <summary>This is called every frame. If you have 60fps, then this method is called 60 times in 1 second</summary>
        public static void Elapse(ElapseData data) {
            currentTime = data.TotalTime.Seconds;

            // We can't track if AI is enabled, so we have to cheat by counting down if AI didn't perform anything
            if (currentTime >= lastAICall + AI_TIMEOUT) {
                AIEnabled = false;
            }

            if(AIEnabled) {
                if (Plugin.DirectionLight != Indicator) {
                    Plugin.SetDirectionIndicator(Indicator);
                }
            }
        }

        /// <summary>Is called when a virtual key is pressed.</summary>
        public static void KeyDown(VirtualKeys key) {
        }

        /// <summary>Is called when a virtual key is released.</summary>
        public static void KeyUp(VirtualKeys key) {
        }

        /// <summary>Is called when the train passes a beacon.</summary>
        /// <param name="beacon">The beacon data.</param>
        public static void SetBeacon(BeaconData beacon) {
            switch (beacon.Type) {
                case BeaconIndices.AIIndicators:
                    switch(beacon.Optional) {
                        case -1:
                            Indicator = IndicatorLight.Left;
                            break;
                        case 0:
                            Indicator = IndicatorLight.None;
                            break;
                        case 1:
                            Indicator = IndicatorLight.Right;
                            break;
                        case 2:
                            Indicator = IndicatorLight.Both;
                            break;
                    }
                    break;
            }
        }

        // Called by OpenBVE when AI is performed
        public static void PerformAI(AIData data) {
            ResetAITimer();
        }

        private static void ResetAITimer() {
            lastAICall = currentTime;
            AIEnabled = true;
        }
    }
}