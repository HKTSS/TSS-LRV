using OpenBveApi.Runtime;
using System;
using System.Collections.Generic;

namespace Plugin.Managers {
	internal partial class StopReporterManager {
        private static readonly double LangSlideDuration = 2.5;
        private static readonly double LangSwitchDelay = 1;
        private static readonly double LangStartSlideTime = LangSwitchTime - LangSlideDuration - LangSwitchDelay;
        private static readonly double SpecialMessageIncrementPause = 2.5;
        private static double TextureShiftX;
        private static double TextureShiftY;
        private static double PauseTimer = SpecialMessageIncrementPause;

        private static void UpdateTextureShift(ElapseData data) { 
            if(LangState == 1 || LangState == 2) {
                double elapsedShiftTime = ElapsedTime - LangStartSlideTime - ((LangSwitchTime * LangState) - LangSwitchTime);
                TextureShiftX = Util.ClampNumber(0, (elapsedShiftTime / LangSlideDuration), 1);
            } else {
                TextureShiftX = 0;

                if(LangState == 3 && CurrentSpecialMessage != null) { 
                    if(PauseTimer >= SpecialMessageIncrementPause) {
                        
                    }
                }
            }
        }
    }
}
