using System.Collections.Generic;
using OpenBveApi.Runtime;

namespace Plugin.Managers {
    /// <summary>The interface to be implemented by the plugin.</summary>
    public static class PAManager {
        private static List<int> QueuedPA = new List<int>();
        private static int PlayingPA;

        /// <summary>Is called when the plugin should initialize, reinitialize or jumping stations.</summary>
        internal static void Initialize() {
            QueuedPA.Clear();
            SoundManager.Stop(PlayingPA);
            
            PlayingPA = 0;
        }

        /// <summary>This is called every frame. </summary>
        internal static void Elapse(ElapseData data) {
            if (!SoundManager.IsPlaying(PlayingPA) && QueuedPA.Count > 0) {
                SoundManager.PlayAllCar(QueuedPA[0], 1.0, 1.0, false);
                PlayingPA = QueuedPA[0];
                QueuedPA.RemoveAt(0);
            }
        }

        /// <summary>Is called when a virtual key is pressed.</summary>
        internal static void KeyDown() {
/*            if (!doorOpened) {
               queuedPA.Add(120 + currentRoute);
            }*/
        }

        internal static void QueuePA(int atsIndex) {
            QueuedPA.Add(atsIndex);
        }
    }
}