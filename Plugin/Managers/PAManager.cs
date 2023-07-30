using System.Collections.Generic;
using OpenBveApi.Runtime;
using static Plugin.Plugin;

namespace Plugin {
    /// <summary>The interface to be implemented by the plugin.</summary>
    public class PAManager {

        private static List<int> QueuedPA = new List<int>();
        private static int PlayingPA;

        /// <summary>Is called when the plugin should initialize, reinitialize or jumping stations.</summary>
        public void Load() {
            QueuedPA.Clear();
            SoundManager.Stop(PlayingPA);
            
            PlayingPA = 0;
        }

        /// <summary>This is called every frame. If you have 60fps, then this method is called 60 times in 1 second</summary>
        public void Elapse(ElapseData data) {
            if (!SoundManager.IsPlaying(PlayingPA) && QueuedPA.Count > 0) {
                SoundManager.PlayAllCar(QueuedPA[0], 1.0, 1.0, false);
                PlayingPA = QueuedPA[0];
                QueuedPA.RemoveAt(0);
            }
        }

        /// <summary>Is called when a virtual key is pressed.</summary>
        public static void KeyDown() {
/*            if (!doorOpened) {
               queuedPA.Add(120 + currentRoute);
            }*/
        }

        public void queuePA(int atsIndex) {
            QueuedPA.Add(atsIndex);
        }
    }
}