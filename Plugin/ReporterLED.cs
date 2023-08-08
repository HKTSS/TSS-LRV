using OpenBveApi.Runtime;
using Plugin.Managers;
using System;
using System.Collections.Generic;

namespace Plugin {
	internal class ReporterLED {
		public static List<Data.ReporterMessage> SpecialMessages = new List<Data.ReporterMessage>();
		public static int lastFrameDest;
		private const double ShiftYInterval = 3.5;
		private const int SlideSpeedX = 300;
		private const int SlideSpeedY = 140;
		private static Data.ReporterMessage currentMessages = new Data.ReporterMessage();
		private static States currentState;
		private static bool showingSpecialMessage;
		private static double elapsedTime;
		private static double nextStateTime = 0;
		private static double deltaTime;
		private static double nextSwipeTickX;
		private static double nextSwipeTickY;
		private static int clampedX = 0;
		private static int clampedY = 0;
		private static int textureShiftX;
		private static int textureShiftY;
		private static bool reporterWillBeHidden;
		private static bool reporterHidden;
		private static bool resetTextureShift;
		private static Random rnd = new Random();

		internal static void Update(ElapseData data) {
			elapsedTime += data.ElapsedTime.Seconds;
			deltaTime = data.ElapsedTime.Seconds;
			
			/* If the destination is different from last frame (Changed) */
			if (lastFrameDest != data.Destination) {
				/* Unhide the reporter */
				reporterHidden = false;
				/* Set it to Special Message and cycle it, so it rolls over to display Chinese LED */
				currentState = States.SpecialMessage;
				CycleStates();
			}

			lastFrameDest = data.Destination;

			if (nextStateTime < elapsedTime) {
				CycleStates();
			}

			if (resetTextureShift) {
				resetTextureShift = false;
				clampedX = 0;
				clampedY = 0;
				textureShiftX = 0;
				textureShiftY = 0;
			}

			/* If now is the time to start swiping, expand the maximum textureShiftX to 500 (0.5) */
			if (nextSwipeTickX < elapsedTime) {
				clampedX = 500;
			}

			if (nextSwipeTickY < elapsedTime) {
				nextSwipeTickY = elapsedTime + ShiftYInterval;
				if (clampedY + currentMessages.incrementY < currentMessages.maxY) {
					clampedY += currentMessages.incrementY;
				}
			}

			int nextTextureShiftX = textureShiftX + (int)(SlideSpeedX * deltaTime);
			int nextTextureShiftY = textureShiftY + (int)(SlideSpeedY * deltaTime);

			textureShiftY = Math.Min(nextTextureShiftY, clampedY);
			textureShiftX = Math.Min(nextTextureShiftX, clampedX);

			/* If the door has opened in a station, tell the reporter to hide the LED when the current state finished displaying. */
			if (StationManager.doorOpenedInStation) {
				reporterWillBeHidden = true;
			}

			/* Output the pluginstate to display nothing if the reporter is hidden */
			if (reporterHidden) {
                PanelManager.Set(PanelIndices.ReporterLEDSpecialState, 0);
                PanelManager.Set(PanelIndices.ReporterLEDState, 2);
            } else {
                PanelManager.Set(PanelIndices.ReporterLEDSpecialState, showingSpecialMessage || !reporterHidden ? currentMessages.states : 0);
                PanelManager.Set(PanelIndices.ReporterLEDState, (int)currentState);
			}

            PanelManager.Set(PanelIndices.ReporterLEDTextureX, textureShiftX);
            PanelManager.Set(PanelIndices.ReporterLEDTextureY, textureShiftY);
		}

		private static void CycleStates() {
			if (reporterWillBeHidden) {
				reporterWillBeHidden = false;
				reporterHidden = true;
			}

			/* Roll back to Chinese after displaying Special Message */
			if (currentState == States.SpecialMessage) {
				currentState = States.Chinese;
			} else {
				/* Increment to the next state */
				currentState++;
			}

			/* Set the time it waits before switching to the next state */
			if (currentState == States.SpecialMessage) {
				/* Randomize the current message */
				currentMessages = SpecialMessages[rnd.Next(SpecialMessages.Count)];
				showingSpecialMessage = true;
			} else {
				showingSpecialMessage = false;
				currentMessages = new Data.ReporterMessage(6, currentState == States.Chinese ? 0 : 1, 0, 0);
			}
			nextStateTime = elapsedTime + (currentMessages.duration + 3);
			nextSwipeTickX = elapsedTime + currentMessages.duration;

			/* Reset the texture shifting so it can be animated again next time */
			resetTextureShift = true;
		}

		private enum States {
			Chinese = 0,
			English = 1,
			SpecialMessage = 2
		}
	}
}
