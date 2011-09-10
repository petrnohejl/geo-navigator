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
    /// Zpracovava databazovy XML soubor s mapami
    /// </summary>
    class MapXml
    {
        // Cesta k XML souboru
        private string xmlFile;
        public string XmlFile
        {
            get { return xmlFile; }
        }

        // Vytvori XML objekt
        public MapXml(string file)
        {
            xmlFile = file;
        }

        /// <summary>
        /// Nacte vsechny mapy do pole objektu tridy Map
        /// </summary>
        public void MapLoad(ArrayList maps)
        {
            // Nacte XML dokument
            XmlDocument lDoc = new XmlDocument();
            XmlNodeList lResult;
            string lXPathQuery;
            lDoc.Load(xmlFile);

            // Pocet map
            lXPathQuery = "/maps/map";
            lResult = lDoc.SelectNodes(lXPathQuery);

            // Definice promennych
            int count = lResult.Count + 1;
            string name;
            string file;
            int width;
            int height;
            double lat1;
            double lon1;
            double lat2;
            double lon2;
            double pixelLength;
            
            // Prochazi jednotlive mapy
            for (int id = 1; id < count; id++)
            {
                // name
                lXPathQuery = "/maps/map[" + id + "]/name";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) name = lResult[0].InnerText;
                else continue;

                // file
                lXPathQuery = "/maps/map[" + id + "]/file";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) file = lResult[0].InnerText;
                else continue;

                // width
                lXPathQuery = "/maps/map[" + id + "]/width";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) width = int.Parse(lResult[0].InnerText);
                else continue;

                // height
                lXPathQuery = "/maps/map[" + id + "]/height";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) height = int.Parse(lResult[0].InnerText);
                else continue;
                
                // lat1
                lXPathQuery = "/maps/map[" + id + "]/lat1";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) lat1 = MakeDouble(lResult[0].InnerText);
                else continue;

                // lon1
                lXPathQuery = "/maps/map[" + id + "]/lon1";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) lon1 = MakeDouble(lResult[0].InnerText);
                else continue;

                // lat2
                lXPathQuery = "/maps/map[" + id + "]/lat2";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) lat2 = MakeDouble(lResult[0].InnerText);
                else continue;

                // lon2
                lXPathQuery = "/maps/map[" + id + "]/lon2";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) lon2 = MakeDouble(lResult[0].InnerText);
                else continue;

                // pixelLength
                lXPathQuery = "/maps/map[" + id + "]/pixel_length";
                lResult = lDoc.SelectNodes(lXPathQuery);
                if (lResult.Count > 0) pixelLength = MakeDouble(lResult[0].InnerText);
                else continue;

                // Prida mapu do pole
                maps.Add(new Map(name, file, width, height, lat1, lon1, lat2, lon2, pixelLength));
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
