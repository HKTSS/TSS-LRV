namespace Plugin {
	static class Util {
		internal static int CarNumPanel(int carNum) {
			/* The plugin states is just Car No. - 1110 */
			/* Example: 1118 - 1110 = 8 */
			return carNum - 1110;
		}

		internal enum LRVType {
			P7,
			P6,
			P5,
			P4,
			P3,
			P1R,
			P1,
			Invalid,
		}

		/* This method are called for each car, which validates the car number depending on what phase the train is. This will also reset to the default value if the car number is invalid */
		internal static int CheckLRVNum(int carIndex, int val, LRVType LRVGen) {
			if (LRVGen == LRVType.P4) {
				if (val >= 1111 && val <= 1132) return val;

				if (carIndex == 1) return 1127;
				else return 1120;
			}

			if (LRVGen == LRVType.P3) {
				if (val >= 1091 && val <= 1110) return val;
				if (carIndex == 1) return 1106;
				else return 1091;
			}

			if (LRVGen == LRVType.P1 || LRVGen == LRVType.P1R) {
				if (val >= 1001 && val <= 1070) return val;
				if (carIndex == 1) return 1043;
				else return 1033;
			}
			return 1001;
		}
	}
}
