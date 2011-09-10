using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

/// <summary>
/// Vycet orientace souradnice
/// </summary>
enum orientation { LAT, LON };

namespace GeoNavigator
{
    /// <summary>
    /// Prevadi souradnice ve stupnich na stupne v textovem formatu
    /// </summary>
    public class Deg2Deg
    {
        // Orientace
        int flag;
        public int Flag
        {
            get { return flag; }
        }

        // Kladna souradnice
        bool isPositive;
        public bool IsPositive
        {
            get { return isPositive; }
        }

        // Stupne
        double degrees;
        public double Degrees
        {
            get { return degrees; }
        }

        /// <summary>
        /// Vytvori novy formar souradnic
        /// </summary>
        public Deg2Deg(double decimalDegrees, int orient)
        {
            isPositive = (decimalDegrees > 0);
            flag = orient;
            degrees = Math.Abs(decimalDegrees);
        }

        /// <summary>
        /// Vraci souradnice v textovem retezci
        /// </summary>
        public override string ToString()
        {
            string str = "";
            if (flag == (int)orientation.LAT && isPositive) str = "N ";
            else if (flag == (int)orientation.LAT && !isPositive) str = "S ";
            else if (flag == (int)orientation.LON && isPositive) str = "E ";
            else if (flag == (int)orientation.LON && !isPositive) str = "W ";
            return str + Math.Round(degrees, 6) + "°";
        }
    }

    /// <summary>
    /// Prevadi souradnice ve stupnich na stupne a minuty v textovem formatu
    /// </summary>
    public class Deg2DegMin
    {
        // Orientace
        int flag;
        public int Flag
        {
            get { return flag; }
        }

        // Kladna souradnice
        bool isPositive;
        public bool IsPositive
        {
            get { return isPositive; }
        }

        // Stupne
        uint degrees;
        public uint Degrees
        {
            get { return degrees; }
        }

        // Minuty
        double minutes;
        public double Minutes
        {
            get { return minutes; }
        }

        /// <summary>
        /// Vytvori novy formar souradnic
        /// </summary>
        public Deg2DegMin(double decimalDegrees, int orient)
        {
            isPositive = (decimalDegrees > 0);
            flag = orient;
            degrees = (uint)Math.Abs(decimalDegrees);
            minutes = (Math.Abs(decimalDegrees) - Math.Abs((double)degrees)) * 60.0;
        }

        /// <summary>
        /// Vraci souradnice v textovem retezci
        /// </summary>
        public override string ToString()
        {
            string str = "";
            if (flag == (int)orientation.LAT && isPositive) str = "N ";
            else if (flag == (int)orientation.LAT && !isPositive) str = "S ";
            else if (flag == (int)orientation.LON && isPositive) str = "E ";
            else if (flag == (int)orientation.LON && !isPositive) str = "W ";
            return str + degrees + "°" + Math.Round(minutes, 4) + "'";
        }
    }

    /// <summary>
    /// Prevadi souradnice ve stupnich na stupne, minuty a vteriny v textovem formatu
    /// </summary>
    public class Deg2DegMinSec
    {
        // Orientace
        int flag;
        public int Flag
        {
            get { return flag; }
        }

        // Kladna souradnice
        bool isPositive;
        public bool IsPositive
        {
            get { return isPositive; }
        }

        // Stupne
        uint degrees;
        public uint Degrees
        {
            get { return degrees; }
        }

        // Minuty
        uint minutes;
        public uint Minutes
        {
            get { return minutes; }
        }

        // Vteriny
        double seconds;
        public double Seconds
        {
            get { return seconds; }
        }

        /// <summary>
        /// Vytvori novy formar souradnic
        /// </summary>
        public Deg2DegMinSec(double decimalDegrees, int orient)
        {
            isPositive = (decimalDegrees > 0);
            flag = orient;
            degrees = (uint)Math.Abs(decimalDegrees);
            double doubleMinutes = (Math.Abs(decimalDegrees) - Math.Abs((double)degrees)) * 60.0;
            minutes = (uint)doubleMinutes;
            seconds = (doubleMinutes - (double)minutes) * 60.0;
        }

        /// <summary>
        /// Vraci souradnice v textovem retezci
        /// </summary>
        public override string ToString()
        {
            string str = "";
            if (flag == (int)orientation.LAT && isPositive) str = "N ";
            else if (flag == (int)orientation.LAT && !isPositive) str = "S ";
            else if (flag == (int)orientation.LON && isPositive) str = "E ";
            else if (flag == (int)orientation.LON && !isPositive) str = "W ";
            return str + degrees + "°" + minutes + "'" + Math.Round(seconds, 3) + "\"";
        }
    }

    /// <summary>
    /// Prevadi souradnice ruznych formatu na stupne
    /// </summary>
    public class ToDeg
    {
        /// <summary>
        /// Prevadi stupne a minuty na stupne
        /// </summary>
        public static string DegMin2Deg(string pos)
        {
            string[] words = pos.Split(' ');
            int deg = int.Parse(words[0]);
            double min = MakeDouble(words[1]);

            double res = Math.Round(deg + min / 60, 6);
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            return res.ToString(provider);
        }

        /// <summary>
        /// Prevadi stupne, minuty a vteriny na stupne
        /// </summary>
        public static string DegMinSec2Deg(string pos)
        {
            string[] words = pos.Split(' ');
            int deg = int.Parse(words[0]);
            double min = MakeDouble(words[1]);
            double sec = MakeDouble(words[2]);

            double res = Math.Round(deg + min/60 + sec/3600, 6);
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            return res.ToString(provider);
        }

        // Prevod desetinneho cisla v retezci na double
        private static double MakeDouble(string str)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            return Double.Parse(str, provider);
        }
    }
}
