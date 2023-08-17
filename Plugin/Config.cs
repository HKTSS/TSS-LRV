﻿using System.IO;
using OpenBveApi.Runtime;
using Plugin.Managers;
using System.Globalization;
using System.Text;

namespace Plugin {
	internal class Config {
		internal static string configPath { get; private set; }
		private static string reporterCustomMsgFile;
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
			reporterCustomMsgFile = Path.Combine(prop.PluginFolder, "SpecialMsgLED.ini");
			LoadReporterMessage();
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

		private static void LoadReporterMessage() {
			string[] lines;
			Data.ReporterMessage currentMessage = null;

			if (File.Exists(reporterCustomMsgFile)) {
				lines = File.ReadAllLines(reporterCustomMsgFile);

				for (int i = 0; i < lines.Length; i++) {
					string ln = lines[i];

					if (ln.ToLowerInvariant().Equals("[end]")) {
						StopReporterManager.SpecialMessages.Add(currentMessage);
						continue;
					}

					if (ln.ToLowerInvariant().Equals("[start]")) {
						currentMessage = new Data.ReporterMessage();
					} else {
						if(currentMessage == null) currentMessage = new Data.ReporterMessage();

						if (ln.Split('=').Length < 2) continue;
						string key = ln.Split('=')[0].Trim();
						string val = ln.Split('=')[1].Trim();
						switch (key) {
							case "duration":
								currentMessage.Duration = int.Parse(val);
								break;
							case "states":
								currentMessage.State = int.Parse(val);
								break;
							case "maxY":
								currentMessage.MaxY = int.Parse(val);
								break;
							case "incrementY":
								currentMessage.IncrementY = int.Parse(val);
								break;
							default:
								break;
						}
					}
				}
			}
		}

		internal static void WriteConfig(string targetKey, string targetValue) {
			try {
				int lineCount = 0;
				string[] line = File.ReadAllLines(configPath);
				foreach (string eachLine in line) {
					string[] cfg = eachLine.Split('=');
					string key = cfg[0].Trim().ToLowerInvariant();
					if (key == targetKey.ToLowerInvariant()) {
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
			sb.AppendLine("TrainStatus = 0");
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
