using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Globalization;
using GeoNavigator;

namespace GeoNavigator
{
    /// <summary>
    /// Zpracovava databazovy GPX - XML soubor
    /// </summary>
    class Gpx
    {
        // Cesta k XML souboru
        private string gpxFile;
        public string GpxFile
        {
            get { return gpxFile; }
        }

        /// <summary>
        /// Vytvori GPX objekt ktery pracuje s XML souborem
        /// </summary>
        /// <param name="file"></param>
        public Gpx(string file)
        {
            gpxFile = file;
        }

        /// <summary>
        /// Vraci ID typu cache podle retezce
        /// </summary>
        private int CacheTypeId(string str)
        {
            str = str.ToLower();
            switch (str)
            {
                case "traditional cache":
                    return (int)cacheType.TRADITIONAL;
                case "multi-cache":
                    return (int)cacheType.MULTI;
                case "unknown cache":
                    return (int)cacheType.UNKNOWN;
                case "letterbox hybrid":
                    return (int)cacheType.LETTERBOX;
                case "whereigo cache":
                    return (int)cacheType.WHEREIGO;
                case "wherigo cache":
                    return (int)cacheType.WHEREIGO;
                case "earthcache":
                    return (int)cacheType.EARTHCACHE;
                case "virtual cache":
                    return (int)cacheType.VIRTUAL;
                case "webcam cache":
                    return (int)cacheType.WEBCAM;
                case "event cache":
                    return (int)cacheType.EVENT;
                case "cache in trash out event":
                    return (int)cacheType.CACHEINTRASHOUT;
                case "mega-event cache":
                    return (int)cacheType.MEGAEVENT;
                default:
                    return (int)cacheType.OTHER;
            }
        }

        /// <summary>
        /// Vraci ID typu velikosti cache podle retezce
        /// </summary>
        private int CacheContainerId(string str)
        {
            str = str.ToLower();
            switch (str)
            {
                case "micro":
                    return (int)cacheContainer.MICRO;
                case "small":
                    return (int)cacheContainer.SMALL;
                case "regular":
                    return (int)cacheContainer.REGULAR;
                case "large":
                    return (int)cacheContainer.LARGE;
                default:
                    return (int)cacheContainer.OTHER;
            }
        }

        /// <summary>
        /// Vytvori novy GPX soubor
        /// </summary>
        public void CreateGpx()
        {
            // Vytvori XmlTextWriter
            XmlTextWriter writer = new XmlTextWriter(gpxFile, System.Text.Encoding.UTF8);

            // Povoli odsazovani
            writer.Formatting = Formatting.Indented;

            // Vypise XML deklaraci
            writer.WriteStartDocument();

            // Korenovy element
            writer.WriteStartElement("gpx");
            writer.WriteAttributeString("xmlns", null, "http://www.topografix.com/GPX/1/1");
            writer.WriteAttributeString("xmlns", "groundspeak", null, "http://www.groundspeak.com/cache/1/1");

            // Ukonci vsechny otevrene tagy
            writer.WriteEndDocument();

            // Uvolni zdroje
            writer.Close();
        }

        /// <summary>
        /// Odstrani cache z GPX souboru
        /// </summary>
        public void CacheDelete(string code)
        {
            // Nacte XML dokument
            XmlDocument lDoc = new XmlDocument();
            lDoc.Load(gpxFile);

            // Namespace
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(lDoc.NameTable);
            nsMgr.AddNamespace("def", "http://www.topografix.com/GPX/1/1");
            nsMgr.AddNamespace("groundspeak", "http://www.groundspeak.com/cache/1/1");

            // Dotaz
            string lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']";

            // Odstraneni uzlu
            XmlNodeList list = lDoc.SelectNodes(lXPathQuery, nsMgr);
            foreach (XmlNode node in list)
            {
                node.ParentNode.ParentNode.RemoveChild(node.ParentNode);
            }

            // Ulozi XML dokument
            lDoc.Save(gpxFile);
        }

        /// <summary>
        /// Prida novou cache do GPX souboru
        /// </summary>
        public void CacheAdd(string[] arr)
        {
            // Nacte XML dokument
            XmlDocument lDoc = new XmlDocument();
            lDoc.Load(gpxFile);

            // Rekurzivne prochazi uzly
            BrowseNodes(lDoc.DocumentElement, lDoc, arr);
            
            // Ulozi XML dokument
            lDoc.Save(gpxFile);
        }

        /// <summary>
        /// Rekurzivne prochazi uzly
        /// </summary>
        private void BrowseNodes(XmlNode xmlUzel, XmlDocument lDoc, string[] arr)
        {
            // Vytvori novy uzel
            if (xmlUzel.Name == "gpx")
            {
                XmlElement xmlelem1;
                XmlElement xmlelem2;
                XmlElement xmlelem3;
                XmlText xmltext;
                XmlAttribute xmlatr;
                string nsGpx = "http://www.topografix.com/GPX/1/1";
                string nsGroundspeak = "http://www.groundspeak.com/cache/1/1";
                string prefixGroundspeak = "groundspeak";

                xmlelem1 = lDoc.CreateElement("", "wpt", nsGpx);
                xmlatr = lDoc.CreateAttribute("lat");
                xmlatr.Value = arr[1];
                xmlelem1.SetAttributeNode(xmlatr);
                xmlatr = lDoc.CreateAttribute("lon");
                xmlatr.Value = arr[2];
                xmlelem1.SetAttributeNode(xmlatr);

                xmlelem2 = lDoc.CreateElement("time", nsGpx);
                xmltext = lDoc.CreateTextNode("");
                xmlelem2.AppendChild(xmltext);
                xmlelem1.AppendChild(xmlelem2);

                xmlelem2 = lDoc.CreateElement("name", nsGpx);
                xmltext = lDoc.CreateTextNode(arr[3]);
                xmlelem2.AppendChild(xmltext);
                xmlelem1.AppendChild(xmlelem2);

                xmlelem2 = lDoc.CreateElement("cmt", nsGpx);
                xmltext = lDoc.CreateTextNode("" + arr[6][0] + arr[7][0] + arr[8][0] + arr[9][0]);
                xmlelem2.AppendChild(xmltext);
                xmlelem1.AppendChild(xmlelem2);

                xmlelem2 = lDoc.CreateElement("desc", nsGpx);
                xmltext = lDoc.CreateTextNode(arr[0]);
                xmlelem2.AppendChild(xmltext);
                xmlelem1.AppendChild(xmlelem2);

                xmlelem2 = lDoc.CreateElement("sym", nsGpx);
                xmltext = lDoc.CreateTextNode("Geocache");
                xmlelem2.AppendChild(xmltext);
                xmlelem1.AppendChild(xmlelem2);

                xmlelem2 = lDoc.CreateElement("type", nsGpx);
                xmltext = lDoc.CreateTextNode("Geocache");
                xmlelem2.AppendChild(xmltext);
                xmlelem1.AppendChild(xmlelem2);

                xmlelem2 = lDoc.CreateElement(prefixGroundspeak, "cache", nsGroundspeak);
                xmlatr = lDoc.CreateAttribute("id");
                xmlatr.Value = "";
                xmlelem2.SetAttributeNode(xmlatr);
                xmlatr = lDoc.CreateAttribute("guid");
                xmlatr.Value = "";
                xmlelem2.SetAttributeNode(xmlatr);
                xmlatr = lDoc.CreateAttribute("cpWPT");
                xmlatr.Value = arr[3];
                xmlelem2.SetAttributeNode(xmlatr);
                xmlatr = lDoc.CreateAttribute("available");
                xmlatr.Value = arr[4];
                xmlelem2.SetAttributeNode(xmlatr);
                xmlatr = lDoc.CreateAttribute("archived");
                xmlatr.Value = arr[5];
                xmlelem2.SetAttributeNode(xmlatr);
                xmlatr = lDoc.CreateAttribute("cpCoords");
                xmlatr.Value = "";
                xmlelem2.SetAttributeNode(xmlatr);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "name", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(arr[0]);
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "lastUpdated", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                string month = DateTime.Now.Month.ToString();
                if (month.Length == 1) month = "0" + month;
                string day = DateTime.Now.Day.ToString();
                if (day.Length == 1) day = "0" + day;
                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "exported", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(
                    DateTime.Now.Year.ToString() + "-" +
                    month + "-" +
                    day + "T00:00:00.0000000+0200");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);
                
                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "placed_by", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "owner", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("Unknown");
                xmlelem3.AppendChild(xmltext);
                xmlatr = lDoc.CreateAttribute("guid");
                xmlatr.Value = "";
                xmlelem3.SetAttributeNode(xmlatr);
                xmlatr = lDoc.CreateAttribute("cpHidden");
                xmlatr.Value = "";
                xmlelem3.SetAttributeNode(xmlatr);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "type", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(arr[6]);
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "container", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(arr[7]);
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "attributes", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "difficulty", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(arr[8]);
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "terrain", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(arr[9]);
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "country", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("Unknown");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "state", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("Unknown");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "short_description", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(arr[11]);
                xmlelem3.AppendChild(xmltext);
                xmlatr = lDoc.CreateAttribute("html");
                xmlatr.Value = "False";
                xmlelem3.SetAttributeNode(xmlatr);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "long_description", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("");
                xmlelem3.AppendChild(xmltext);
                xmlatr = lDoc.CreateAttribute("html");
                xmlatr.Value = "False";
                xmlelem3.SetAttributeNode(xmlatr);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "encoded_hints", nsGroundspeak);
                xmltext = lDoc.CreateTextNode(arr[10]);
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "logs", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem3 = lDoc.CreateElement(prefixGroundspeak, "travelbugs", nsGroundspeak);
                xmltext = lDoc.CreateTextNode("");
                xmlelem3.AppendChild(xmltext);
                xmlelem2.AppendChild(xmlelem3);

                xmlelem1.AppendChild(xmlelem2);
                xmlUzel.AppendChild(xmlelem1);

            }

            // Prochazi dalsi poduzly
            if (xmlUzel.HasChildNodes)
            {
                foreach (XmlNode dcerinnyUzel in xmlUzel.ChildNodes)
                {
                    BrowseNodes(dcerinnyUzel, lDoc, arr);
                }
            }
        }

        /// <summary>
        /// Nacte vsechny cache z GPX databaze a zakladni informace o nich do pole objektu tridy Cache
        /// </summary>
        public void CacheLoad(ArrayList caches)
        {
            // Nacte XML dokument
            XmlDocument lDoc = new XmlDocument();
            XmlNodeList lResult;
            string lXPathQuery;
            lDoc.Load(gpxFile);

            // Namespace
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(lDoc.NameTable);
            nsMgr.AddNamespace("def", "http://www.topografix.com/GPX/1/1");
            nsMgr.AddNamespace("groundspeak", "http://www.groundspeak.com/cache/1/1");

            // Pocet cache
            lXPathQuery = "/def:gpx/def:wpt";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            
            // Definice promennych
            int count = lResult.Count + 1;
            string name;
            double latitude;
            double longitude;
            string code;
            bool available;
            bool archived;
            int type;
            int container;
            double difficulty;
            double terrain;

            // Prochazi jednotlive cache
            for (int id = 1; id < count; id++)
            {
                // name
                lXPathQuery = "/def:gpx/def:wpt[" + id + "]/groundspeak:cache/groundspeak:name";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0) name = lResult[0].InnerText;
                else continue;

                // code, available, archived
                lXPathQuery = "/def:gpx/def:wpt[" + id + "]/groundspeak:cache";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0)
                {
                    if (lResult[0].Attributes["available"].Value.ToLower() == "true") available = true;
                    else available = false;

                    if (lResult[0].Attributes["archived"].Value.ToLower() == "true") archived = true;
                    else archived = false;

                    code = lResult[0].Attributes["cpWPT"].Value;
                }
                else continue;

                // type
                lXPathQuery = "/def:gpx/def:wpt[" + id + "]/groundspeak:cache/groundspeak:type";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0) type = CacheTypeId(lResult[0].InnerText);
                else continue;

                // container
                lXPathQuery = "/def:gpx/def:wpt[" + id + "]/groundspeak:cache/groundspeak:container";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0) container = CacheContainerId(lResult[0].InnerText);
                else continue;

                // difficulty
                lXPathQuery = "/def:gpx/def:wpt[" + id + "]/groundspeak:cache/groundspeak:difficulty";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0) difficulty = MakeDouble(lResult[0].InnerText);
                else continue;

                // terrain
                lXPathQuery = "/def:gpx/def:wpt[" + id + "]/groundspeak:cache/groundspeak:terrain";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0) terrain = MakeDouble(lResult[0].InnerText);
                else continue;
                
                // lat, lon
                lXPathQuery = "/def:gpx/def:wpt[" + id + "]";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0) 
                {
                    latitude = MakeDouble(lResult[0].Attributes["lat"].Value);
                    longitude = MakeDouble(lResult[0].Attributes["lon"].Value);
                }
                else continue;

                // Prida cache do pole
                caches.Add(new Cache(name, latitude, longitude, code, available, archived, type, container, difficulty, terrain, true));
            }
        }

        /// <summary>
        /// Nacte z GPX databaze vsechny informace o dane cache a vrati je v HTML dokumentu
        /// </summary>
        public string CacheInfo(string code, string[] appSettings, bool[] appSettingsShow)
        {
            // Nacte XML dokument
            StringBuilder sb = new StringBuilder();
            XmlDocument lDoc = new XmlDocument();
            XmlNodeList lResult;
            string lXPathQuery;
            lDoc.Load(gpxFile);

            // Namespace
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(lDoc.NameTable);
            nsMgr.AddNamespace("def", "http://www.topografix.com/GPX/1/1");
            nsMgr.AddNamespace("groundspeak", "http://www.groundspeak.com/cache/1/1");

            /*
            // name
            lXPathQuery = "/def:gpx/def:wpt[" + id + "]/groundspeak:cache/groundspeak:name";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"<b>{0}</b><br/><hr/>", lResult[0].InnerText);

            // lat, lon
            lXPathQuery = "/def:gpx/def:wpt[" + id + "]";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Latitude: {0}<br/>Longitude: {1}<br/>",
                lResult[0].Attributes["lat"].Value, lResult[0].Attributes["lon"].Value);
            */

            // code, available, archived
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0)
            {
                string available = lResult[0].Attributes["available"].Value.ToLower();
                string archived = lResult[0].Attributes["archived"].Value.ToLower();
                if (available == "false") available = "<span style='color: red;'>false</span>";
                if (archived == "true") archived = "<span style='color: red;'>true</span>";
                sb.AppendFormat(@"Code: {0}<br/>Available: {1}<br/>Archived: {2}<br/>",
                    lResult[0].Attributes["cpWPT"].Value, available, archived);
            } 

            // type
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:type";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Type: {0}<br/>", lResult[0].InnerText);

            // container
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:container";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Container: {0}<br/>", lResult[0].InnerText);

            // difficulty
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:difficulty";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Difficulty: {0} / 5<br/>", lResult[0].InnerText);

            // terrain
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:terrain";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Terrain: {0} / 5<br/><hr/>", lResult[0].InnerText);

            // country, state
            string country = "";
            string state = "";
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:country";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) country = lResult[0].InnerText;
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:state";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) state = lResult[0].InnerText;
            if (country == state) sb.AppendFormat(@"Country: {0}<br/>", country);
            else sb.AppendFormat(@"Country: {0}<br/>State: {1}<br/>", country, state);

            // owner
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:owner";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Owner: {0}<br/>Hidden: {1}<br/>",
                lResult[0].InnerText, ConvertDate(lResult[0].Attributes["cpHidden"].Value, false));

            // lastUpdated
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:lastUpdated";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Last updated: {0}<br/>", ConvertDate(lResult[0].InnerText, true));

            // exported
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:exported";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0) sb.AppendFormat(@"Exported: {0}<br/><hr/>", ConvertDate(lResult[0].InnerText, true));

            // encoded_hints
            if (appSettingsShow[2])
            {
                lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:encoded_hints";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0 && lResult[0].InnerText != "") sb.AppendFormat(@"Hint: {0}<br/><hr/>", lResult[0].InnerText);
            }
            
            // short_description
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:short_description";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0 && lResult[0].InnerText != "") sb.AppendFormat(@"Short description: {0}<br/><hr/>", lResult[0].InnerText);

            // attributes
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:attributes/groundspeak:attribute";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0)
            {
                sb.AppendFormat(@"Attributes:<ul>");
                for (int i = 0; i < lResult.Count; i++)
                    sb.AppendFormat(@"<li>{0}</li>", lResult[i].InnerText);
                sb.AppendFormat(@"</ul><hr/>");
            }

            // travelbugs
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:travelbugs/groundspeak:travelbug/groundspeak:name";
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0)
            {
                sb.AppendFormat(@"Travel bugs:<ul>");
                for (int i = 0; i < lResult.Count; i++)
                    sb.AppendFormat(@"<li>{0}</li>", lResult[i].InnerText);
                sb.AppendFormat(@"</ul><hr/>");
            }

            // logs
            if (appSettingsShow[1])
            {
                lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:logs/groundspeak:log";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                int cnt = lResult.Count;
                if (cnt > 0)
                {
                    sb.AppendFormat(@"Logs:<ul>");
                    for (int i = 0; i < cnt; i++)
                    {
                        sb.AppendFormat(@"<li>");

                        lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:logs/groundspeak:log[" + (i + 1) + "]/groundspeak:date";
                        lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                        if (lResult.Count > 0) sb.AppendFormat(@"<i>{0}</i> ", ConvertDate(lResult[0].InnerText, false));

                        lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:logs/groundspeak:log[" + (i + 1) + "]/groundspeak:finder";
                        lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                        if (lResult.Count > 0) sb.AppendFormat(@"<i>{0}</i> ", lResult[0].InnerText);

                        lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:logs/groundspeak:log[" + (i + 1) + "]/groundspeak:type";
                        lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                        if (lResult.Count > 0) sb.AppendFormat(@"<i>{0}</i><br/>", lResult[0].InnerText.ToLower());

                        lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:logs/groundspeak:log[" + (i + 1) + "]/groundspeak:text";
                        lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                        if (lResult.Count > 0) sb.AppendFormat(@"{0}", lResult[0].InnerText);

                        sb.AppendFormat(@"</li>");
                    }
                    sb.AppendFormat(@"</ul><hr/>");
                }
            }
            
            // long_description
            if (appSettingsShow[3])
            {
                lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/groundspeak:long_description";
                lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
                if (lResult.Count > 0) sb.AppendFormat(@"{0}<br/>", lResult[0].InnerText);
            }
            
            // Vrati HTML
            return sb.ToString();
        }

        /// <summary>
        /// Vrati hodnotu daneho elementu dane cache
        /// </summary>
        public string GetElementContent(string code, string element)
        {
            // Nacte XML dokument
            StringBuilder sb = new StringBuilder();
            XmlDocument lDoc = new XmlDocument();
            XmlNodeList lResult;
            string lXPathQuery;
            lDoc.Load(gpxFile);

            // Namespace
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(lDoc.NameTable);
            nsMgr.AddNamespace("def", "http://www.topografix.com/GPX/1/1");
            nsMgr.AddNamespace("groundspeak", "http://www.groundspeak.com/cache/1/1");

            // Dotaz
            lXPathQuery = "/def:gpx/def:wpt/groundspeak:cache[@cpWPT='" + code + "']/" + element;
            lResult = lDoc.SelectNodes(lXPathQuery, nsMgr);
            if (lResult.Count > 0 && lResult[0].InnerText != "") sb.AppendFormat(@"{0}", lResult[0].InnerText);

            // Vraci hodnotu elementu
            return sb.ToString();
        }

        // Konvertuje datum a volitelne i cas v ISO formatu na lepe citelny format
        private string ConvertDate(string iso, bool time)
        {
            try
            {
                if (time == true) return iso.Substring(0, 10) + " " + iso.Substring(11, 5);
                else return iso.Substring(0, 10);
            }
            catch
            {
                return "";
            }
            
        }

        // Prevod desetinneho cisla v retezci na double
        private double MakeDouble(string str)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            return Double.Parse(str, provider);
        }
    }
}
