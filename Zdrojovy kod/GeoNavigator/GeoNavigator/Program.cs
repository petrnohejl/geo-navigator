using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GeoNavigator
{
    static class Program
    {
        /// <summary>
        /// Hlavni trida cele aplikace
        /// </summary>
        [MTAThread]
        static void Main()
        {
            // Vytvoreni hlavniho okna
            Application.Run(new MainForm());
        }
    }
}
