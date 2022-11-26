using System;
using System.Windows.Forms;
using System.Xml;

namespace Plugin {
    public partial class Update : Form {
        private string downloadURL;

        internal static void checkUpdate(string lang, Version currentVersion) {
            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(Config.updateURL);
                string fetchedVersion = doc.SelectSingleNode("train/version").InnerText;
                string downloadURL = doc.SelectSingleNode("train/url").InnerText;
                string releaseDate = doc.SelectSingleNode("train/release").InnerText;
                int comparasion = currentVersion.CompareTo(Version.Parse(fetchedVersion));

                if (comparasion < 0) {
                    Update form = new Update(downloadURL);
                    form.updateLabel.Text = string.Format(Messages.getTranslation("updateForm.UpdateAvail"), fetchedVersion, releaseDate);
                    form.DLLabel.Links.Add(0, form.DLLabel.Text.Length, downloadURL);
                    form.ShowDialog();
                }
            } catch (Exception) {
            }
        }

        public Update(string downloadURL) {
            this.downloadURL = downloadURL;
            InitializeComponent();
        }

        private void DLLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(downloadURL);
        }

        private void OKButton_Click(object sender, EventArgs e) {
            Config.ignoreUpdate = IgnoreUpdateCheckbox.Checked;
            Config.WriteConfig("ignoreupdate", IgnoreUpdateCheckbox.Checked.ToString());
            this.Close();
        }

        private void Update_Load(object sender, EventArgs e) {
            DLLabel.Text = Messages.getTranslation("updateForm.Download");
            IgnoreUpdateCheckbox.Text = Messages.getTranslation("updateForm.Ignore");
            OKButton.Text = Messages.getTranslation("configForm.ApplyChangeBtn");
        }
    }
}
