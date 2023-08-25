using OpenBveApi.Runtime;
using System;

namespace Plugin.Managers {
	internal partial class StopReporterManager {
        private static readonly double LangSlideDuration = 2.5;
        private static readonly double LangSwitchDelay = 1;
        private static readonly double LangStartSlideTime = LangSwitchTime - LangSlideDuration - LangSwitchDelay;
        private static readonly double SpecialMessageIncrementPause = 4;
        private static readonly double SpecialMessageScrollSpeed = 0.14;
        private static double TextureShiftX;
        private static double TextureShiftY;
        private static double CappedY;

        private static void UpdateTextureShift(ElapseData data) { 
            if(LangState == 1 || LangState == 2) {
                double elapsedShiftTime = ElapsedTime - LangStartSlideTime - ((LangSwitchTime * LangState) - LangSwitchTime);
                TextureShiftX = Util.ClampNumber(0, (elapsedShiftTime / LangSlideDuration), 1);
            } else {
                TextureShiftX = 0;

                if(LangState == 3 && CurrentSpecialMessage != null) {
                    double elapsedSpecialMsgTime = ElapsedTime - LangSwitchTime * 2;
                    CappedY = Math.Ceiling(elapsedSpecialMsgTime / SpecialMessageIncrementPause) * 0.1;
                    TextureShiftY = Math.Min(TextureShiftY + (data.ElapsedTime.Seconds) * SpecialMessageScrollSpeed, CappedY);
                }
            }
        }
    }
}
