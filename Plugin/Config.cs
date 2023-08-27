using System.IO;
using OpenBveApi.Runtime;
using Plugin.Managers;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Plugin {
	internal class Config {
		internal static string configPath { get; private set; }
		private static string SpecialReporterMessageFile;
        private static string reporterHacksFile;
		private static string[] lines;
		private static string failureReason;
		internal static int carNum1 = 1127;
		internal static int carNum2 = 1120;
		internal static bool doorLockEnabled;
		internal static bool doorApplyBrake;
		internal static bool doorlockEnabled;
		internal static bool iSPSEnabled;
        internal static bool dsdEnabled;
		internal static bool crashEnabled;
		internal static bool mtrBeeping;
		internal static bool allowReversingInStations;
		internal static int trainStatus;

		internal static bool LoadConfig(LoadProperties prop, Util.LRVType LRVGen) {
			configPath = Path.Combine(prop.PluginFolder, "config.txt");
			SpecialReporterMessageFile = Path.Combine(prop.PluginFolder, "stopreporter_special_msg.xml");
            reporterHacksFile = Path.Combine(prop.PluginFolder, "stopreporter_hacks.xml");
            LoadReporterSpecialMessage();
            LoadStopReporterHacks();
			Messages.RegisterTranslations();
			/* Get the Plugin Folder path, then combine it with LRVSystem.ini for the full path to the config file. */
			if (File.Exists(configPath)) {
				try {
					lines = File.ReadAllLines(configPath, Encoding.UTF8);
				} catch {
					prop.FailureReason = "\n[LRV-System] Failed to read configuration file, aborting plugin initalization!";
					return false;
				}

				/* Loop through each line, from top to bottom */
				foreach (string line in lines) {
					if (!line.StartsWith("#")) {
						string[] cfg = line.Split('=');
						if (cfg.Length >= 2) {
							string key = cfg[0].Trim().ToLowerInvariant();
							string valstr = cfg[1].Trim().ToLowerInvariant();
							int val;
							string[] seperated = valstr.Split(',');
							switch (key) {
								case "carnum":
									if (int.TryParse(seperated[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out val)) {
										carNum1 = Util.CheckLRVNum(1, val, LRVGen);
									}
									if (seperated.Length > 1) {
										if (int.TryParse(seperated[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out val)) {
											carNum2 = Util.CheckLRVNum(2, val, LRVGen);
										}
									}
									break;
								case "doorlock":
									if (valstr == "true") doorlockEnabled = true;
									else doorlockEnabled = false;
									break;
								case "applybrake":
									if (valstr == "true") doorApplyBrake = true;
									else doorApplyBrake = false;
									break;
								case "ispsdoorlock":
									if (valstr == "true") iSPSEnabled = true;
									else iSPSEnabled = false;
									break;
								case "crash":
									if (valstr == "true") crashEnabled = true;
									else crashEnabled = false;
									break;
                                case "dsd":
                                    if (valstr == "true") dsdEnabled = true;
                                    else dsdEnabled = false;
                                    break;
								case "mtrbeep":
									if (valstr == "true") mtrBeeping = true;
									else mtrBeeping = false;
									break;
								case "revatstation":
									if (valstr == "true") allowReversingInStations = true;
									else allowReversingInStations = false;
									break;
                                case "lrvgeneration":
                                    switch(valstr) {
                                        case "1":
                                            Plugin.LRVGeneration = Util.LRVType.P1;
                                            break;
                                        case "1r":
                                            Plugin.LRVGeneration = Util.LRVType.P1R;
                                            break;
                                        case "2":
                                            Plugin.LRVGeneration = Util.LRVType.P2;
                                            break;
                                        case "3":
                                            Plugin.LRVGeneration = Util.LRVType.P3;
                                            break;
                                        case "4":
                                            Plugin.LRVGeneration = Util.LRVType.P4;
                                            break;
                                        case "5":
                                            Plugin.LRVGeneration = Util.LRVType.P5;
                                            break;
                                        case "6":
                                            Plugin.LRVGeneration = Util.LRVType.P6;
                                            break;
                                        case "7":
                                            Plugin.LRVGeneration = Util.LRVType.P7;
                                            break;
                                    }
                                    break;
                                case "trainstatus":
									if (int.TryParse(valstr, NumberStyles.Integer, CultureInfo.InvariantCulture, out val)) {
										if (val <= 3) trainStatus = val;
									}
									break;
							}
						}
					}
				}

                return true;
			} else {
				/* We have generated a config file without any error. Now we can apply the changes instantly in-game */
				if (GenerateConfig()) {
					carNum1 = 1127;
					carNum2 = 1120;
					doorApplyBrake = true;
					doorlockEnabled = true;
					iSPSEnabled = true;
					trainStatus = 0;
					crashEnabled = true;
					return true;
				} else {
					/* Otherwise, we set the failure reason to be the failure reason set on GenerateConfig method. Then return false to not load the plugin */
					prop.FailureReason = failureReason;
					return false;
				}
			}
		}

		private static void LoadReporterSpecialMessage() {
			if (File.Exists(SpecialReporterMessageFile)) {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(SpecialReporterMessageFile);

                XmlNodeList entries = xmlDoc.GetElementsByTagName("Entry");
                foreach (XmlNode messageNode in entries) {
                    Data.ReporterMessage reporterMessage = new Data.ReporterMessage(messageNode);
                    StopReporterManager.SpecialMessages.Add(reporterMessage);
                }
			}
		}

        private static void LoadStopReporterHacks() { 
            if(File.Exists(reporterHacksFile)) {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reporterHacksFile);

                XmlNodeList stations = xmlDoc.GetElementsByTagName("Station");
                foreach (XmlNode stationNode in stations) {
                    Data.StationHacksEntry stationHacksEntry = new Data.StationHacksEntry(stationNode);
                    StopReporterManager.StationHacksEntries.Add(stationHacksEntry);
                }
            }
        }

        internal static void WriteConfig(string targetKey, string targetValue) {
			try {
				int lineCount = 0;
				string[] line = File.ReadAllLines(configPath);
				foreach (string eachLine in line) {
					string[] cfg = eachLine.Split('=');
					string key = cfg[0].Trim();
					if (key.ToLowerInvariant() == targetKey.ToLowerInvariant()) {
						lines[lineCount] = key + " = " + targetValue;
					}
					lineCount++;
				}
				File.WriteAllLines(configPath, lines);
			} catch {
				MessageManager.PrintMessage("Fail to save the configuration file!", OpenBveApi.Colors.MessageColor.Red, 5.0);
			}
		}

		internal static bool GenerateConfig() {
			StringBuilder sb = new StringBuilder();
            // Comment
            sb.AppendLine("# 修改此檔案後，你需要重新載入OpenBVE才能令設定生效");
            sb.AppendLine("# After editing this config, please restart OpenBVE to apply changes.");
            sb.AppendLine("# true = 啟用/Enabled");
            sb.AppendLine("# false = 停用/Disabled");
            sb.AppendLine("");
			sb.AppendLine("CarNum = 1127,1120");
			sb.AppendLine("DoorLock = true");
			sb.AppendLine("ApplyBrake = true");
			sb.AppendLine("iSPSDoorLock = true");
			sb.AppendLine("Crash = true");
			sb.AppendLine("MTRbeep = false");
			sb.AppendLine("RevAtStation = false");
            sb.AppendLine("DSD = false");
            sb.AppendLine("TrainStatus = 0");
            sb.AppendLine("LRVGeneration = 4");
            try {
				File.WriteAllText(configPath, sb.ToString());
				return true;
			} catch {
				failureReason = "\n[LRV-System] Configuration file not found and failed to generate a config file.\n[LRV-System] Please check your permission.";
				return false;
			}
		}
	}
}
