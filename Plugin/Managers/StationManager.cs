using OpenBveApi.Runtime;

namespace Plugin.Managers {
	internal static class StationManager {
		private static int stationIndex;
		private static double lastFramePosition;
		internal static Station nextStation { get; private set; }
		internal static Station prevStation { get; private set; }
        internal static Station nextStationInternal;
        private static Station prevStationInternal;
        internal static bool approachingStation = true;
        internal static bool offsetToNextStation;
		internal static bool doorOpenedInStation;

		internal static void Update(ElapseData data) {
			while (stationIndex < data.Stations.Count - 1 && data.Stations[stationIndex].StopPosition + 5 < data.Vehicle.Location) stationIndex++;
			while (stationIndex > 1 && data.Stations[stationIndex - 1].StopPosition > data.Vehicle.Location) stationIndex--;

            int offset = 0;

            bool trainApproachedStation = nextStationInternal != null && data.Vehicle.Location >= nextStationInternal.DefaultTrackPosition;
            bool trainWithinStation = nextStationInternal != null && (trainApproachedStation && data.Vehicle.Location < nextStationInternal.StopPosition + 5);

            if (offsetToNextStation) offset++;

            nextStation = Util.GetClampedListItem(data.Stations, stationIndex + offset);
            prevStation = Util.GetClampedListItem(data.Stations, (stationIndex + offset) - 1);

            nextStationInternal = Util.GetClampedListItem(data.Stations, stationIndex);
            prevStationInternal = Util.GetClampedListItem(data.Stations, stationIndex - 1);

            if (!trainWithinStation) {
                offsetToNextStation = false;
            }

            if (Plugin.DoorOpened && trainApproachedStation) {
                offsetToNextStation = true;
				doorOpenedInStation = true;
            } else {
                doorOpenedInStation = false;
			}

			/* If last frame the train has not yet reached the start of the station
			and now the train ran past the start of the station. Trigger something once */
			if (lastFramePosition < nextStation.DefaultTrackPosition && data.Vehicle.Location >= nextStation.DefaultTrackPosition)
			{
				Plugin.DoorOpened2 = false;
				approachingStation = true;
			}

			lastFramePosition = data.Vehicle.Location;
		}
	}
}
