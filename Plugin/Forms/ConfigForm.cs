using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugin {
    public partial class ConfigForm : Form {
        public ConfigForm() {
            InitializeComponent();
            if (Plugin.Language.ToLowerInvariant().StartsWith("zh")) {
                TranslateForm();
            }
        }

        internal void TranslateForm() {
            try {
                Text = Messages.getTranslation("configForm.Title");
                TrainCarNumberLabel.Text = Messages.getTranslation("configForm.CarNumLabel");
                SafetySystemLabel.Text = Messages.getTranslation("configForm.SafetySysLabel");
                MiscLabel.Text = Messages.getTranslation("configForm.OtherLabel");
                FirstCarLabel.Text = Messages.getTranslation("configForm.CarNum1Label");
                SecondCarLabel.Text = Messages.getTranslation("configForm.CarNum2Label");
                DoorLockCheckBox.Text = Messages.getTranslation("configForm.DoorLockLabel");
                ApplyBrakeCheckBox.Text = Messages.getTranslation("configForm.DoorApplyBrakeLabel");
                iSPSDoorLockEnabled.Text = Messages.getTranslation("configForm.iSPSDoorLockLabel");
                CrashCheckBox.Text = Messages.getTranslation("configForm.CrashEffectLabel");
                MTRBeeping.Text = Messages.getTranslation("configForm.MTRBeepingLabel");
                RevAtStation.Text = Messages.getTranslation("configForm.ReverseAtStnLabel");
                tutCheckBox.Text = Messages.getTranslation("configForm.TutorialLabel");
                /* Change the font size of the TrainStatusLabel */
                TrainStatusLabel.Font = new Font("Arial", 8.75F);
                TrainStatusLabel.Text = Messages.getTranslation("configForm.TrainStatusLabel");
                /* Have to move down the TrainStatusLabel 2 pixels down when in chinese to make it looks right. */
                TrainStatusLabel.Location = new Point(TrainStatusLabel.Location.X, TrainStatusLabel.Location.Y + 2);
                TrainStatusBox.Items[0] = Messages.getTranslation("configForm.TrainStatus1");
                TrainStatusBox.Items[1] = Messages.getTranslation("configForm.TrainStatus2");
                TrainStatusBox.Items[2] = Messages.getTranslation("configForm.TrainStatus3");
                TrainStatusBox.Items[3] = Messages.getTranslation("configForm.TrainStatus4");
                ApplyChanges.Text = Messages.getTranslation("configForm.ApplyChangeBtn");
            } catch (Exception e) {
                MessageBox.Show(e.StackTrace);
            }
        }

        internal static void LaunchForm() {
            if (Application.OpenForms.OfType<ConfigForm>().Any()) {
                Application.OpenForms.OfType<ConfigForm>().First().BringToFront();
            } else {
                Task.Run(() => Application.Run(new ConfigForm()));
            }
        }

        private void ConfigForm_Load(object sender, EventArgs e) {
            DoorLockCheckBox.Checked = Config.doorlockEnabled;
            ApplyBrakeCheckBox.Checked = Config.doorApplyBrake;
            iSPSDoorLockEnabled.Checked = Config.iSPSEnabled;
            CrashCheckBox.Checked = Config.crashEnabled;
            MTRBeeping.Checked = Config.mtrBeeping;
            RevAtStation.Checked = Config.allowReversingInStations;
            TrainStatusBox.SelectedIndex = Config.trainStatus;
            tutCheckBox.Checked = Config.tutorialMode;
            if (Plugin.LRVGeneration == Util.LRVType.P1 || Plugin.LRVGeneration == Util.LRVType.P1R) {
                CarNum1Box.Minimum = 1001;
                CarNum1Box.Maximum = 1090;
                CarNum2Box.Minimum = 1001;
                CarNum2Box.Maximum = 1090;
            } else if (Plugin.LRVGeneration == Util.LRVType.P3) {
                CarNum1Box.Minimum = 1091;
                CarNum1Box.Maximum = 1110;
                CarNum2Box.Minimum = 1091;
                CarNum2Box.Maximum = 1110;
            } else if (Plugin.LRVGeneration == Util.LRVType.P4) {
                CarNum1Box.Minimum = 1111;
                CarNum1Box.Maximum = 1132;
                CarNum2Box.Minimum = 1111;
                CarNum2Box.Maximum = 1132;
            } else if (Plugin.LRVGeneration == Util.LRVType.P5) {
                CarNum1Box.Minimum = 1133;
                CarNum1Box.Maximum = 1162;
                CarNum2Box.Minimum = 1211;
                CarNum2Box.Maximum = 1220;
            }
            CarNum1Box.Value = Config.carNum1;
            CarNum2Box.Value = Config.carNum2;
            if (Plugin.specs.Cars < 2) {
                CarNum2Box.Enabled = false;
            }
        }

        private void CarNumFilter(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-')) e.Handled = true;
        }

        private void ApplyChanges_Click(object sender, EventArgs e) {
            Config.trainStatus = TrainStatusBox.SelectedIndex;
            Config.carNum1 = Convert.ToInt32(CarNum1Box.Value);
            Config.carNum2 = Convert.ToInt32(CarNum2Box.Value);
            Config.iSPSEnabled = iSPSDoorLockEnabled.Checked;
            Config.doorlockEnabled = DoorLockCheckBox.Checked;
            Config.doorApplyBrake = ApplyBrakeCheckBox.Checked;
            Config.crashEnabled = CrashCheckBox.Checked;
            Config.mtrBeeping = MTRBeeping.Checked;
            Config.allowReversingInStations = RevAtStation.Checked;
            Config.tutorialMode = tutCheckBox.Checked;
            Plugin.ChangeCarNumber(1, Util.CarNumPanel((int) CarNum1Box.Value));
            Plugin.ChangeCarNumber(2, Util.CarNumPanel((int) CarNum2Box.Value));
            Config.WriteConfig("CarNum", CarNum1Box.Value + "," + CarNum2Box.Value);
            Config.WriteConfig("DoorLock", DoorLockCheckBox.Checked.ToString().ToLowerInvariant());
            Config.WriteConfig("DoorBrake", ApplyBrakeCheckBox.Checked.ToString().ToLowerInvariant());
            Config.WriteConfig("iSPSdoorlock", iSPSDoorLockEnabled.Checked.ToString().ToLowerInvariant());
            Config.WriteConfig("Crash", CrashCheckBox.Checked.ToString().ToLowerInvariant());
			Config.WriteConfig("MTRbeep", MTRBeeping.Checked.ToString().ToLowerInvariant());
            Config.WriteConfig("RevAtStation", RevAtStation.Checked.ToString().ToLowerInvariant());
            Config.WriteConfig("TrainStatus", TrainStatusBox.SelectedIndex.ToString());
            Config.WriteConfig("Tutorial", tutCheckBox.Checked.ToString().ToLowerInvariant());
            this.Close();
            this.Invalidate();
            /* Force quit the form, as mono just freezes the form without this :/ */
            Application.Exit();
        }
	}
}
