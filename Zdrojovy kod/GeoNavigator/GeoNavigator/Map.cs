using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace GeoNavigator
{
    /// <summary>
    /// Uchovava informace o mape
    /// </summary>
    class Map
    {
        // Nazev mapy
        string name;
        public string Name { get { return name; } }

        // Nazev a cesta k souboru
        string file;
        public string File { get { return file; } }

        // Sirka mapy v pixelech
        int width;
        public int Width { get { return width; } }

        // Vyska mapy v pixelech
        int height;
        public int Height { get { return height; } }

        // Latitude leveho horniho rohu
        double lat1;
        public double Lat1 { get { return lat1; } }

        // Longitude leveho horniho rohu
        double lon1;
        public double Lon1 { get { return lon1; } }

        // Latitude praveho dolniho rohu
        double lat2;
        public double Lat2 { get { return lat2; } }

        // Longitude praveho dolniho rohu
        double lon2;
        public double Lon2 { get { return lon2; } }

        // Delka jednoho pixelu mapy v metrech
        double pixelLength;
        public double PixelLength { get { return pixelLength; } }

        // Velikost stupne na 1 pixel v latitude
        double pixelLat;
        public double PixelLat { get { return pixelLat; } }

        // Velikost stupne na 1 pixel v longitude
        double pixelLon;
        public double PixelLon { get { return pixelLon; } }

        /// <summary>
        /// Vytvori novou mapu
        /// </summary>
        public Map(string mapName, string mapFile, int mapWidth, int mapHeight, 
            double mapLat1, double mapLon1, double mapLat2, double mapLon2, double mapPixelLenght)
        {
            name = mapName;
            file = mapFile;
            width = mapWidth;
            height = mapHeight;
            lat1 = mapLat1;
            lon1 = mapLon1;
            lat2 = mapLat2;
            lon2 = mapLon2;
            pixelLength = mapPixelLenght;

            // Velikost stupne na 1 pixel
            pixelLat = Math.Abs(lat1 - lat2) / height;
            pixelLon = Math.Abs(lon1 - lon2) / width;
        }

        /// <summary>
        /// Vytvori novou mapu
        /// </summary>
        public Map(string mapName, string mapFile, int mapWidth, int mapHeight,
            double mapLat1, double mapLon1, double mapLat2, double mapLon2)
        {
            name = mapName;
            file = mapFile;
            width = mapWidth;
            height = mapHeight;
            lat1 = mapLat1;
            lon1 = mapLon1;
            lat2 = mapLat2;
            lon2 = mapLon2;

            // Velikost stupne na 1 pixel
            pixelLat = Math.Abs(lat1 - lat2) / height;
            pixelLon = Math.Abs(lon1 - lon2) / width;

            // Vypocet delky 1 pixelu v metrech
            int coef = 111120; // namorni mile (minuta sirky) * 60 = pocet metru na 1 stupen sirky
            pixelLength = ((pixelLat + pixelLon) / 2) * coef;
        }
    }
}
