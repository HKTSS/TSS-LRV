using System.Xml;

namespace Plugin.Data {
    internal class ReporterMessage {
        public double Duration;
        public int State;
        public ReporterMessage() {
        }

        public ReporterMessage(double duration, int state) {
            this.Duration = duration;
            this.State = state;
        }

        public ReporterMessage(XmlNode xmlNode) {
            foreach (XmlNode nodes in xmlNode) {
                switch (nodes.Name.ToLowerInvariant())
                {
                    case "duration":
                        double.TryParse(nodes.InnerText, out Duration);
                        break;
                    case "state":
                        int.TryParse(nodes.InnerText, out State);
                        break;
                }
            }
        }
    }
}
