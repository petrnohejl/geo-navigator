using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Vycet typu cache
/// </summary>
enum cacheType { TRADITIONAL, MULTI, UNKNOWN, LETTERBOX, WHEREIGO, EARTHCACHE, VIRTUAL, WEBCAM, EVENT, CACHEINTRASHOUT, MEGAEVENT, OTHER };

/// <summary>
/// Vycet velikosti schranky
/// </summary>
enum cacheContainer { MICRO, SMALL, REGULAR, LARGE, OTHER };

namespace GeoNavigator
{
    /// <summary>
    /// Uchovava zakladni informace o cache
    /// </summary>
    class Cache
    {
        // Nazev cache
        string name;
        public string Name { get { return name; } }

        // Zemepisna sirka
        double latitude;
        public double Latitude { get { return latitude; } }

        // Zemepisna delka
        double longitude;
        public double Longitude { get { return longitude; } }

        // Kod cache
        string code;
        public string Code { get { return code; } }

        // Dostupnost
        bool available;
        public bool Available { get { return available; } }

        // Archivace
        bool archived;
        public bool Archived { get { return archived; } }

        // Typ cache
        int type;
        public int Type { get { return type; } }

        // Velikost schranky
        int container;
        public int Container { get { return container; } }

        // Obtiznost
        double difficulty;
        public double Difficulty { get { return difficulty; } }

        // Teren
        double terrain;
        public double Terrain { get { return terrain; } }

        // Filtr
        bool view;
        public bool View { get { return view; } set { view = value; } }

        /// <summary>
        /// Vytvori novou cache
        /// </summary>
        public Cache(string cacheName, double cacheLat, double cacheLon, string cacheCode, 
            bool cacheAvail, bool cacheArch, int cacheType, int cacheCont, double cacheDiff, double cacheTerr, bool cacheView)
        {
            name = cacheName;
            latitude = cacheLat;
            longitude = cacheLon;
            code = cacheCode;
            available = cacheAvail;
            archived = cacheArch;
            type = cacheType;
            container = cacheCont;
            difficulty = cacheDiff;
            terrain = cacheTerr;
            view = cacheView;
        }

        /// <summary>
        /// Vrati nazev typu cache podle ID typu
        /// </summary>
        public static string TypeStr(int id)
        {
            switch (id)
            {
                case 0:
                    return "Traditional Cache";
                case 1:
                    return "Multi-cache";
                case 2:
                    return "Unknown Cache";
                case 3:
                    return "Letterbox Hybrid";
                case 4:
                    return "Wherigo Cache";
                case 5:
                    return "Earthcache";
                case 6:
                    return "Virtual Cache";
                case 7:
                    return "Webcam Cache";
                case 8:
                    return "Event Cache";
                case 9:
                    return "Cache In Trash Out Event";
                case 10:
                    return "Mega-Event Cache";
                default:
                    return "Other";
            }

        }

        /// <summary>
        /// Vrati nazev velikosti schranky podle ID typu velikosti
        /// </summary>
        public static string ContainerStr(int id)
        {
            switch (id)
            {
                case 0:
                    return "Micro";
                case 1:
                    return "Small";
                case 2:
                    return "Regular";
                case 3:
                    return "Large";
                default:
                    return "Other";
            }
        }
    }
}
