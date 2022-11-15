using OpenBveApi.Runtime;

namespace Plugin {
	internal static class StationManager {
		private static int stationIndex;
		private static double lastFramePosition;
		private static double lastAICall;
		internal static double currentTime;
		internal static bool AIEnabled;
		internal static Station nextStation;
		internal static Station prevStation;
		internal static bool approachingStation = true;
		internal static bool doorOpenedInAStation;
		internal static void ResetAITimer() {
			lastAICall = currentTime;
			AIEnabled = true;
		}

		internal static void Update(ElapseData data) {
			currentTime = data.TotalTime.Seconds;
			while (stationIndex < data.Stations.Count - 1 && data.Stations[stationIndex].StopPosition + 5 < data.Vehicle.Location) stationIndex++;
			while (stationIndex > 1 && data.Stations[stationIndex - 1].StopPosition > data.Vehicle.Location) stationIndex--;
            nextStation = data.Stations[stationIndex];
			prevStation = data.Stations[stationIndex - 1 < 0 ? 0 : stationIndex - 1];

			if (currentTime > lastAICall + 10) {
				AIEnabled = false;
			}

			if (Plugin.DoorOpened && data.Vehicle.Location >= nextStation.DefaultTrackPosition) {
				doorOpenedInAStation = true;
			} else {
				doorOpenedInAStation = false;
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
