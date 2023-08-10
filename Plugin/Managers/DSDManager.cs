using OpenBveApi.Runtime;
namespace Plugin.Managers
{
    public static class DSDManager
    {
        private const double DSD_TIMEOUT = 2;
        private static bool dsdHeld;
        private static VehicleSpecs specs;
        private static double unheldTime;

        internal static void Initialize(VehicleSpecs specs) {
            DSDManager.specs = specs;
        }

        internal static void Elapse(ElapseData data) {
            if(!Config.dsdEnabled) {
                System.Windows.Forms.MessageBox.Show("S");
                dsdHeld = true;
            }

            PanelManager.Set(PanelIndices.DSDHeld, dsdHeld ? 1 : 0);

            if(!dsdHeld) {
                unheldTime += data.ElapsedTime.Seconds;
            } else {
                unheldTime = 0;
            }

            if (unheldTime > DSD_TIMEOUT) {
                data.Handles.BrakeNotch = specs.BrakeNotches;
            }
        }

        internal static void setDSDHeld(bool newDSDHeld) {
            if(newDSDHeld != dsdHeld) {
                // Click sound
                SoundManager.Play(SoundIndices.powerHandleClick, 1.0, 1.0, false);
            }

            if (Config.dsdEnabled) {
                dsdHeld = newDSDHeld;
            }
        }
    }
}
