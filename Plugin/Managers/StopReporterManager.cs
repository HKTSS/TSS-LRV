using OpenBveApi.Runtime;

namespace Plugin.Managers {
	internal partial class StopReporterManager {
        private static readonly double LangSwitchTime = 8;
        private static double ElapsedTime;
        private static int LastLangState;
        private static int LangState;
        private static bool ReporterHidden;

        internal static void Elapse(ElapseData data) {
			ElapsedTime += data.ElapsedTime.Seconds;
            UpdateState(data);
            UpdateTextureShift(data);

            if (ReporterHidden) {
                LangState = 0;
            }
            PanelManager.Set(PanelIndices.StopReporterLangState, LangState);
            PanelManager.Set(PanelIndices.StopReporterTextureX, (int)(TextureShiftX * 1000));
            PanelManager.Set(PanelIndices.StopReporterTextureY, (int)(TextureShiftY * 1000));
            PanelManager.Set(PanelIndices.StopReporterSpecialState, CurrentSpecialMessage == null ? 0 : CurrentSpecialMessage.State);
        }

        private static void UpdateState(ElapseData data) {
            if (ElapsedTime > LangSwitchTime * 2) {
                // Special
                UpdateSpecialMessage(data);
            } else if (ElapsedTime > LangSwitchTime) {
                // English
                LangState = 2;
            } else if (ElapsedTime > 0) {
                // Chinese
                LangState = 1;
            }

            // State changed
            if (LastLangState != LangState) {
                if(LangState == 3) {
                    SetRandomSpecialMessage();
                }

                if (StationManager.doorOpenedInStation) {
                    ReporterHidden = true;
                }
            }

            // Passed 100m from last station, resets the reporter
            double prevStnDistance = data.Vehicle.Location - StationManager.prevStation.StopPosition;
            if (prevStnDistance > 100 && ReporterHidden) {
                ResetReporter();
            }

            LastLangState = LangState;
        }

        private static void ResetReporter() {
            ReporterHidden = false;
            ResetReporterAnimation();
        }

        private static void ResetReporterAnimation() {
            ElapsedTime = 0;
            TextureShiftX = 0;
            TextureShiftY = 0;
            LangState = 1;
        }
    }
}
