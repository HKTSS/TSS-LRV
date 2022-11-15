using OpenBveApi.Runtime;

namespace Plugin {
    internal class CameraManager {
        private CameraViewMode CameraMode;
        public void Update(ElapseData data) {
            CameraMode = data.CameraViewMode;
        }

        public bool isInCab() {
            return CameraMode == CameraViewMode.Interior || CameraMode == CameraViewMode.InteriorLookAhead;
        }

        public bool isInF2() {
            return CameraMode == CameraViewMode.Exterior;
        }

        public bool isInF3() {
            return CameraMode == CameraViewMode.Track;
        }

        public bool isInF4() {
            return CameraMode == CameraViewMode.FlyBy || CameraMode == CameraViewMode.FlyByZooming;
        }

        public CameraViewMode GetMode() {
            return CameraMode;
        }
    }
}
