using OpenBveApi.Runtime;

namespace Plugin {
    internal class CameraManager {
        private static CameraViewMode CameraMode;
        public static void Update(ElapseData data) {
            CameraMode = data.CameraViewMode;
        }

        public static bool isInCab() {
            return CameraMode == CameraViewMode.Interior || CameraMode == CameraViewMode.InteriorLookAhead;
        }

        public static bool isInF2() {
            return CameraMode == CameraViewMode.Exterior;
        }

        public static bool isInF3() {
            return CameraMode == CameraViewMode.Track;
        }

        public static bool isInF4() {
            return CameraMode == CameraViewMode.FlyBy || CameraMode == CameraViewMode.FlyByZooming;
        }

        public static CameraViewMode GetMode() {
            return CameraMode;
        }
    }
}
