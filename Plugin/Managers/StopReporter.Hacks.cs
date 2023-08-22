using System;
using System.Collections.Generic;
using OpenBveApi.Runtime;
namespace Plugin.Managers
{
    internal static partial class StopReporterManager
    {
        public static List<Data.StationHacksEntry> StationHacksEntries = new List<Data.StationHacksEntry>();
        private static readonly double CheckFrequency = 1;
        private static double Elapsed;

        private static void UpdateStationHacks(ElapseData data) {
            Elapsed += data.ElapsedTime.Seconds;
            if(Elapsed >= CheckFrequency) {
                Elapsed = 0;

                if (StationManager.nextStationInternal != null) {
                    foreach (Data.StationHacksEntry hackEntry in StationHacksEntries) {
                        if(hackEntry.matches(StationManager.nextStationInternal.Name)) {
                            PanelManager.Set(PanelIndices.StopReporterStationHacks, hackEntry.destination);
                            return;
                        }
                    }
                }
                PanelManager.Set(PanelIndices.StopReporterStationHacks, -1);
            }
        }
    }
}
