using System;
using System.Xml;
using System.Collections.Generic;
namespace Plugin.Data
{
    public class StationHacksEntry
    {
        public readonly List<string> keywords;
        public readonly int destination;

        public StationHacksEntry(XmlNode xmlNode)
        {
            keywords = new List<string>();

            foreach (XmlNode nodes in xmlNode) {
                switch(nodes.Name.ToLowerInvariant()) {
                    case "destination":
                        int.TryParse(nodes.InnerText, out destination);
                        break;
                    case "keyword":
                        keywords.Add(nodes.InnerText.ToLowerInvariant());
                        break;
                }
            }
        }

        public bool matches(string targetStationName) {
            return keywords.Contains(targetStationName);
        }
    }
}
