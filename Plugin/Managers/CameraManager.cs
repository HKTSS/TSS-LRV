using OpenBveApi.Runtime;

namespace Plugin.Managers {
    internal class CameraManager {
        private static CameraViewMode CameraMode;
        internal static void Update(ElapseData data) {
            CameraMode = data.CameraViewMode;
        }

        internal static bool InCab() {
            return CameraMode == CameraViewMode.Interior || CameraMode == CameraViewMode.InteriorLookAhead;
        }

        internal static bool InF2() {
            return CameraMode == CameraViewMode.Exterior;
        }

        internal static bool InF3() {
            return CameraMode == CameraViewMode.Track;
        }

        internal static bool InF4() {
            return CameraMode == CameraViewMode.FlyBy || CameraMode == CameraViewMode.FlyByZooming;
        }

        internal static CameraViewMode GetMode() {
            return CameraMode;
        }
    }
}
