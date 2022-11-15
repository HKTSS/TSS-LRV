using System.Collections.Generic;

namespace Plugin {
    internal class Messages {
        private static Dictionary<string, string> ChineseTranslations = new Dictionary<string,string>();
        private static Dictionary<string, string> EnglishTranslations = new Dictionary<string, string>();

        public static void RegisterTranslations() {
            ChineseTranslations.Clear();
            EnglishTranslations.Clear();
            /* Chinese Translations */
            ChineseTranslations.Add("configForm.Title", "列車設定");

            ChineseTranslations.Add("configForm.CarNumLabel", "車卡編號");

            ChineseTranslations.Add("configForm.CrashEffectLabel", "開啟撞車效果");
            ChineseTranslations.Add("configForm.MTRBeepingLabel", "使用港鐵化關門提示聲");
            ChineseTranslations.Add("configForm.TutorialLabel", "教學模式");
            ChineseTranslations.Add("configForm.TrainStatusLabel", "列車狀態指示牌: ");

            ChineseTranslations.Add("configForm.SafetySysLabel", "安全系統");
            ChineseTranslations.Add("configForm.OtherLabel", "其他");
            ChineseTranslations.Add("configForm.CarNum1Label", "第一卡:");
            ChineseTranslations.Add("configForm.CarNum2Label", "第二卡:");
            ChineseTranslations.Add("configForm.DoorLockLabel", "列車未停定的情況下，鎖住車門");
            ChineseTranslations.Add("configForm.DoorApplyBrakeLabel", "車門開啟時，鎖住牽引動力");
            ChineseTranslations.Add("configForm.iSPSDoorLockLabel", "駛入月台後，如車門未開啟時，抑制牽引動力");
            ChineseTranslations.Add("configForm.ReverseAtStnLabel", "允許進站後倒車");

            ChineseTranslations.Add("configForm.TrainStatus1", "沒有");
            ChineseTranslations.Add("configForm.TrainStatus2", "NOT TO GO (九鐵)");
            ChineseTranslations.Add("configForm.TrainStatus3", "NOT TO GO (港鐵)");
            ChineseTranslations.Add("configForm.TrainStatus4", "SCOTCH BLOCK (港鐵)");
            ChineseTranslations.Add("configForm.ApplyChangeBtn", "確定");

            ChineseTranslations.Add("updateForm.UpdateAvail", "此列車的最新版本為{version}, 並已在{date}推出。");
            ChineseTranslations.Add("updateForm.Download", "按此前往下載網頁");
            ChineseTranslations.Add("updateForm.Ignore", "不再顯示更新");

            ChineseTranslations.Add("gameMsg.turnOffTurnSignal", "開啟死火燈前, 請先關閉指揮燈");


            /* English Translations */
            EnglishTranslations.Add("configForm.Title", "Train Configuration");

            EnglishTranslations.Add("configForm.CarNumLabel", "Car Number");

            EnglishTranslations.Add("configForm.CrashEffectLabel", "Crash effect when hitting the trains infront");
            EnglishTranslations.Add("configForm.MTRBeepingLabel", "Use MTR Door Close Beeping");
            EnglishTranslations.Add("configForm.TutorialLabel", "Tutorial Mode");
            EnglishTranslations.Add("configForm.TrainStatusLabel", "Train Status:");

            EnglishTranslations.Add("configForm.SafetySysLabel", "Safety Systems");
            EnglishTranslations.Add("configForm.OtherLabel", "Misc");
            EnglishTranslations.Add("configForm.CarNum1Label", "First Car:");
            EnglishTranslations.Add("configForm.CarNum2Label", "Second Car:");
            EnglishTranslations.Add("configForm.DoorLockLabel", "Lock all doors when the train departs");
            EnglishTranslations.Add("configForm.doorApplyBrakeLabel", "Apply brake when the door is opened");
            EnglishTranslations.Add("configForm.iSPSDoorLockLabel", "Apply brake until driver opens the door after approaching a station");
            EnglishTranslations.Add("configForm.ReverseAtStnLabel", "Allow reversing after approaching a station");

            EnglishTranslations.Add("configForm.TrainStatus1", "None");
            EnglishTranslations.Add("configForm.TrainStatus2", "NOT TO GO (KCR)");
            EnglishTranslations.Add("configForm.TrainStatus3", "NOT TO GO (MTR)");
            EnglishTranslations.Add("configForm.TrainStatus4", "SCOTCH BLOCK (MTR)");
            EnglishTranslations.Add("configForm.ApplyChangeBtn", "OK");

            EnglishTranslations.Add("updateForm.UpdateAvail", "The latest version of LRV P4 is {0} released on {1}");
            EnglishTranslations.Add("updateForm.Download", "Click Here to Download");
            EnglishTranslations.Add("updateForm.Ignore", "Don't show update in the future");

            EnglishTranslations.Add("gameMsg.turnOffTurnSignal", "Please switch off the turn signal before activating the hazard warning light.");
        }

        internal static string getTranslation(string key) {
            string language = Plugin.Language;

            if (language.StartsWith("zh")) {
                if (ChineseTranslations.TryGetValue(key, out string result)) {
                    return result;
                }
            } else {
                if (EnglishTranslations.TryGetValue(key, out string result)) {
                    return result;
                }
            }
            return key;
        }
    }
}
