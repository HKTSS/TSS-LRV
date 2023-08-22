using System;
using System.Collections.Generic;
using OpenBveApi.Runtime;

namespace Plugin.Managers
{
    internal static partial class StopReporterManager
    {
        private static readonly Random rnd = new Random();
        private static Data.ReporterMessage CurrentSpecialMessage = null;
        public static List<Data.ReporterMessage> SpecialMessages = new List<Data.ReporterMessage>();

        private static void UpdateSpecialMessage(ElapseData data) {
            // Special Message
            LangState = 3;

            if (CurrentSpecialMessage != null) {
                // Special Message is over
                if (ElapsedTime > (LangSwitchTime * 2) + CurrentSpecialMessage.Duration) {
                    ResetReporterAnimation();
                }
            }
        }

        private static void SetRandomSpecialMessage() {
            CurrentSpecialMessage = SpecialMessages[rnd.Next(SpecialMessages.Count - 1)];
        }
    }
}
