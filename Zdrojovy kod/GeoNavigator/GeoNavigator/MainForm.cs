using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.IO;
using Microsoft.WindowsMobile.Samples.Location;
using Microsoft.WindowsMobile.Status;
using GeoNavigator;
using Ini;

namespace GeoNavigator
{
    /// <summary>
    /// Hlavni okno aplikace
    /// </summary>
    public partial class MainForm : Form
    {
        // GPS
        private EventHandler updateDataHandler;
        private GpsDeviceState device = null;
        private GpsPosition position = null;
        private Gps gps = new Gps();

        // Aktualni souradnice
        private double lat = 0;
        private double lon = 0;

        // Seznam cache a map
        private ArrayList caches = new ArrayList();
        private ArrayList maps = new ArrayList();

        // Aktualni vybrana cache a mapa
        private int selectedCache;
        private int selectedMap;

        // INI
        private IniFile ini;

        // Nastaveni a filtry
        private string[] settings = new string[4];
        private bool[] settingsShow = new bool[6];
        private int[] filter = new int[17];

        // Sledovani preklopeni displeje
        private SystemState orientationWatcher;

        // Zoom
        private int zoomIndex = 2;
        private double[] zoomList = { 0.5, 0.75, 1, 1.5, 2, 3, 5, 8, 15, 30, 50 };

        // Mapovy obrazkovy podklad
        private Bitmap mapImage;

        // Definice vykreslovaci plochy, barev a fontu
        private System.Drawing.Graphics g;
        private System.Drawing.Pen pen1 = new System.Drawing.Pen(Color.Orange, 2.0F); // navadeci cara
        private System.Drawing.Pen pen2 = new System.Drawing.Pen(Color.Red, 2.0F); // meritko
        private System.Drawing.Pen pen3 = new System.Drawing.Pen(Color.Red, 3.0F); // navadeci sipka
        private System.Drawing.Pen pen4 = new System.Drawing.Pen(Color.Green, 8.0F); // bod cache
        private System.Drawing.Pen pen5 = new System.Drawing.Pen(Color.Goldenrod, 8.0F); // bod cache
        private System.Drawing.Pen pen6 = new System.Drawing.Pen(Color.Navy, 8.0F); // bod cache
        private System.Drawing.Pen pen7 = new System.Drawing.Pen(Color.Gray, 8.0F); // bod cache
        private System.Drawing.Pen pen8 = new System.Drawing.Pen(Color.Sienna, 8.0F); // bod cache
        private System.Drawing.Pen pen9 = new System.Drawing.Pen(Color.Crimson, 8.0F); // bod cache
        private System.Drawing.SolidBrush brush1 = new SolidBrush(Color.Black); // prazdnota
        private System.Drawing.SolidBrush brush2 = new SolidBrush(Color.Red); // meritko
        private System.Drawing.SolidBrush brush4 = new SolidBrush(Color.Green); // nazev cache
        private System.Drawing.SolidBrush brush5 = new SolidBrush(Color.Goldenrod); // nazev cache
        private System.Drawing.SolidBrush brush6 = new SolidBrush(Color.Navy); // nazev cache
        private System.Drawing.SolidBrush brush7 = new SolidBrush(Color.Gray); // nazev cache
        private System.Drawing.SolidBrush brush8 = new SolidBrush(Color.Sienna); // nazev cache
        private System.Drawing.SolidBrush brush9 = new SolidBrush(Color.Crimson); // nazev cache
        private System.Drawing.Font font1 = new Font("Arial", 8, FontStyle.Bold);

        // Cesta k programu a k mapovym podkladum
        private string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        private string pathMap = "";

        /// <summary>
        /// Vytvori hlavni okno aplikace
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inicializace okna
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // GPS inicializace
            updateDataHandler = new EventHandler(UpdateData);
            gps.DeviceStateChanged += new DeviceStateChangedEventHandler(gps_DeviceStateChanged);
            gps.LocationChanged += new LocationChangedEventHandler(gps_LocationChanged);

            // INI soubor
            ini = new IniFile(path + "\\settings.ini");

            // Sledovani preklopeni displeje
            orientationWatcher = new SystemState(SystemProperty.DisplayRotation);
            orientationWatcher.Changed += new ChangeEventHandler(OrientationWatcher_Changed);

            // Inicializace
            Layout();
            IniLoad();
            DatabaseLoad();
            MapLoad();

            // GPX databazovy soubor
            openFileDialog1.FileName = settings[0];
            openFileDialog1.Filter = "XML file (.xml)|*.xml|GPX file (.gpx)|*.gpx";
            openFileDialog1.FilterIndex = 1;

            // XML mapovy soubor
            openFileDialog2.FileName = settings[1];
            openFileDialog2.Filter = "XML file (.xml)|*.xml";
            openFileDialog2.FilterIndex = 1;
            comboBox1.SelectedIndex = selectedMap;
            Map map = (Map)maps[selectedMap];
            try
            {
                mapImage = new Bitmap(pathMap + map.File);
            }
            catch
            {
                mapImage = null;
            }
        }

        /// <summary>
        /// Sleduje preklopeni displeje a prizpusobuje usporadani layoutu
        /// </summary>
        private void OrientationWatcher_Changed(object sender, ChangeEventArgs args)
        {
            Layout();
        }

        /// <summary>
        /// Usporada formularove prvky na obrazovce
        /// </summary>
        private void Layout()
        {
            // Sirka a vyska
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;

            Point p = new Point();
            Size s = new Size();

            // Horni lista
            s.Width = w; s.Height = 30; panel1.Size = s;
            s.Width = w - 135; s.Height = 15; label3.Size = s;
            s.Width = w - 135; s.Height = 15; label4.Size = s;
            p.X = w - 40; p.Y = 0; label5.Location = p;
            p.X = w - 40; p.Y = 17; progressBar1.Location = p;
            
            // Zalozka main
            s.Width = w; s.Height = h - 54; pictureBox1.Size = s;
            p.X = w - 111; p.Y = 36; pictureBox9.Location = p;
            p.X = w - 74; p.Y = 36; pictureBox10.Location = p;
            p.X = w - 37; p.Y = 36; pictureBox11.Location = p;
            p.X = 0; p.Y = h - 46; label8.Location = p;
            s.Width = w; s.Height = 23; label8.Size = s;

            // Zalozka stats
            s.Width = w; s.Height = h - 96; label7.Size = s;
            p.X = 5; p.Y = h - 60; pictureBox6.Location = p;
            p.X = 42; p.Y = h - 60; pictureBox7.Location = p;
            p.X = 79; p.Y = h - 60; pictureBox8.Location = p;

            // Zalozka cache
            s.Width = w; s.Height = h - 54; webBrowser1.Size = s;

            // Zalozka database
            s.Width = w; s.Height = h - 164; listBox1.Size = s;
            s.Width = w - 10; s.Height = 20; label6.Size = s;
            p.X = w - 37; p.Y = 36; pictureBox15.Location = p;
            p.X = 5; p.Y = h - 60; pictureBox12.Location = p;
            p.X = 42; p.Y = h - 60; pictureBox13.Location = p;
            p.X = 79; p.Y = h - 60; pictureBox14.Location = p;
            
            // Zalozka menu
            s.Width = w - 10; s.Height = 20; label9.Size = s;
            s.Width = w - 10; s.Height = 22; comboBox1.Size = s;

            // Zobrazeni ramecku s hintem
            HintBox();
        }

        /// <summary>
        /// Nacte databazi cache z GPX souboru
        /// </summary>
        private void DatabaseLoad()
        {
            // Vyprazdneni seznamu
            caches.Clear();
            listBox1.Items.Clear();

            // Nastaveni formularovych prvku
            label3.Text = "no cache";
            label4.Text = "";
            if (settings[0] != "") label6.Text = settings[0];
            else label6.Text = "no file";
            label8.Text = "";
            HintBox();
            webBrowser1.DocumentText = "<html><head></head><body bgcolor='black' text='white'>no cache</body></html>";
            selectedCache = -1;

            // Nacteni cache a odfiltrovani
            if (settings[0] != "") // GPX soubor existuje
            {
                Gpx gpxDoc = new Gpx(settings[0]);
                try
                {
                    gpxDoc.CacheLoad(caches);
                    UseFilter();
                }
                catch (System.IO.FileNotFoundException)
                {
                    string message = "Database file " + gpxDoc.GpxFile + " doesn't exist!";
                    string caption = "Error";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    DatabaseDeselect();
                }
                catch
                {
                    string message = "Error! Cannot load database data!";
                    string caption = "Error";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    DatabaseDeselect();
                }
            }

            /*
            // Kontrolni vypis
            StringBuilder sb = new StringBuilder();
            foreach (Cache element in caches)
                sb.AppendFormat(@"{0}<br/>{1}<br/>{2}<br/>{3}<br/>{4}<br/>{5}<br/>{6}<br/>{7}<br/>{8}<br/>{9}<br/>{10}<br/><hr/>",
                    element.Name, element.Latitude, element.Longitude, element.Code, element.Available, element.Archived,
                    element.Type, element.Container, element.Difficulty, element.Terrain, element.View);
            webBrowser1.DocumentText = sb.ToString();
            */
        }

        /// <summary>
        /// Odznaci databazi s cache
        /// </summary>
        private void DatabaseDeselect()
        {
            caches.Clear();
            listBox1.Items.Clear();

            listBox1.Refresh();
            label3.Text = "no cache";
            label4.Text = "";
            label6.Text = "no file";
            label8.Text = "";
            HintBox();
            webBrowser1.DocumentText = "<html><head></head><body bgcolor='black' text='white'>no cache</body></html>";
            selectedCache = -1;
            settings[0] = "";
        }

        /// <summary>
        /// Odfiltruje cache
        /// </summary>
        private void UseFilter()
        {
            foreach (Cache element in caches)
            {
                bool view = true;

                // Kontrola typu
                switch (element.Type)
                {
                    case (int)cacheType.TRADITIONAL:
                        if(filter[0] == 0) view = false;
                        break;
                    case (int)cacheType.MULTI:
                        if(filter[1] == 0) view = false;
                        break;
                    case (int)cacheType.UNKNOWN:
                        if(filter[2] == 0) view = false;
                        break;
                    case (int)cacheType.LETTERBOX:
                        if(filter[3] == 0) view = false;
                        break;
                    case (int)cacheType.EARTHCACHE:
                        if(filter[4] == 0) view = false;
                        break;
                    default:
                        if(filter[5] == 0) view = false;
                        break;
                }

                // Kontrola velikosti schranky
                switch (element.Container)
                {
                    case (int)cacheContainer.MICRO:
                        if (filter[6] == 0) view = false;
                        break;
                    case (int)cacheContainer.SMALL:
                        if (filter[7] == 0) view = false;
                        break;
                    case (int)cacheContainer.REGULAR:
                        if (filter[8] == 0) view = false;
                        break;
                    case (int)cacheContainer.LARGE:
                        if (filter[9] == 0) view = false;
                        break;
                    default:
                        if (filter[10] == 0) view = false;
                        break;
                }

                // Kontrola stavu, obtiznosti a terenu
                if (!element.Available && filter[11] == 0) view = false;
                if (element.Archived && filter[12] == 0) view = false;
                if (element.Difficulty < filter[13] || element.Difficulty > filter[14]) view = false;
                if (element.Terrain < filter[15] || element.Terrain > filter[16]) view = false;

                // Nastaveni priznaku zobrazeni a pridani do seznamu cache
                element.View = view;
                if(view) this.listBox1.Items.Add(element.Name);
            }
        }

        /// <summary>
        /// Nacte veskere informace o vybrane cache
        /// </summary>
        private void UpdateCache()
        {
            bool err = false;

            if (listBox1.SelectedIndex != -1)
            {
                selectedCache = SelectedCache();
                Cache selected = (Cache)caches[selectedCache];
                Gpx gpxDoc = new Gpx(settings[0]);

                // Vypis info na horni listu
                label3.Text = selected.Name;
                label4.Text = Cache.TypeStr(selected.Type)[0] + " " +
                    Cache.ContainerStr(selected.Container)[0] + " " +
                    selected.Difficulty.ToString() + " " +
                    selected.Terrain.ToString();

                // Vypis hintu
                try
                {
                    label8.Text = gpxDoc.GetElementContent(selected.Code, "groundspeak:encoded_hints");
                }
                catch
                {
                    err = true;
                }
                HintBox();

                // Vypis kompletni info o kesi
                string latitude = "";
                string longitude = "";
                switch (settings[3])
                {
                    case "degmin":
                        latitude = "" + new Deg2DegMin(selected.Latitude, (int)orientation.LAT);
                        longitude = "" + new Deg2DegMin(selected.Longitude, (int)orientation.LON);
                        break;
                    case "degminsec":
                        latitude = "" + new Deg2DegMinSec(selected.Latitude, (int)orientation.LAT);
                        longitude = "" + new Deg2DegMinSec(selected.Longitude, (int)orientation.LON);
                        break;
                    default:
                        latitude = "" + new Deg2Deg(selected.Latitude, (int)orientation.LAT);
                        longitude = "" + new Deg2Deg(selected.Longitude, (int)orientation.LON);
                        break;
                }

                // Vypis HTML
                string head = "<b>" + selected.Name + "</b><br/><hr/>" +
                    "Latitude: " + latitude +
                    "<br/>Longitude: " + longitude + "<br/>";
                try
                {
                    webBrowser1.DocumentText = "<html><head></head><body bgcolor='black' text='white'>" + head + gpxDoc.CacheInfo(selected.Code, settings, settingsShow) + "</body></html>";
                }
                catch
                {
                    err = true;
                }

                // Error - GPX soubor neexistuje nebo je chybny
                if (err)
                {
                    string message = "Error! Cannot load database data!";
                    string caption = "Error";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    DatabaseDeselect();
                }
            }
        }

        /// <summary>
        /// Vraci index vybrane cache po odfiltrovani
        /// </summary>
        private int SelectedCache()
        {
            int view = 0;
            int notview = 0;
            foreach (Cache c in caches)
            {
                if (c.View)
                {
                    if (listBox1.SelectedIndex == view)
                    {
                        selectedCache = view + notview;
                        break;
                    }
                    view++;
                }
                else notview++;
            }

            return selectedCache;
        }

        /// <summary>
        /// Nastavi zobrazeni hintu v mape
        /// </summary>
        private void HintBox()
        {
            Size s = new Size();
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            if (label8.Text == "" || !settingsShow[2])
            {
                s.Width = w; s.Height = h - 54; pictureBox1.Size = s;
                label8.Visible = false;
            }
            else
            {
                s.Width = w; s.Height = h - 77; pictureBox1.Size = s;
                label8.Visible = true;
            }
        }

        /// <summary>
        /// Nacte databazi mapovych podkladu z XML souboru
        /// </summary>
        private void MapLoad()
        {
            int index = selectedMap;
            maps.Clear();

            // Nastavi adresar kde se nachazi mapy
            int pos = settings[1].LastIndexOf("\\");
            pathMap = settings[1].Substring(0, pos+1);

            // Pridani defaultni mapy
            maps.Add(new Map("Default", "", 1000, 700, 0.003222, 0, 0, 0.006774, 0.5));

            // Nacte XML dokument a mapy
            if (settings[1] != "") // XML soubor existuje
            {
                MapXml xmlDoc = new MapXml(settings[1]);
                try
                {
                    xmlDoc.MapLoad(maps);
                    label9.Text = settings[1];
                }
                catch
                {
                    string message = "Cannot load map database!";
                    string caption = "Error";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    label9.Text = "no file";
                    settings[1] = "";
                }
            }
            else
            {
                label9.Text = "no file";
            }

            // Prida mapy do seznamu map
            ArrayList list1 = new ArrayList();
            foreach (Map map in maps)
                list1.Add(map.Name);
            comboBox1.DataSource = list1;

            if (index <= maps.Count) selectedMap = index;
            else selectedMap = 0;

            /*
            // Kontrolni vypis
            StringBuilder sb = new StringBuilder();
            foreach (Map element in maps)
                sb.AppendFormat(@"{0}<br/>{1}<br/>{2}<br/>{3}<br/>{4}<br/>{5}<br/>{6}<br/>{7}<br/>{8}<br/>{9}<br/>{10}<br/><hr/>",
                    element.Name, element.File, element.Width, element.Height, element.Lat1, element.Lon1,
                    element.Lat2, element.Lon2, element.PixelLength, element.PixelLat, element.PixelLon);
            webBrowser1.DocumentText = sb.ToString();
             */
        }

        /// <summary>
        /// Nacte nastaveni programu z INI souboru
        /// </summary>
        private void IniLoad()
        {
            bool err = false;

            // Implicitni hodnoty
            settings[0] = "";
            settings[1] = "";
            settings[2] = "meter";
            settings[3] = "deg";
            selectedCache = -1;
            selectedMap = 0;
            
            for (int x = 0; x < settingsShow.Length; x++ )
                settingsShow[x] = true;

            for (int x = 0; x < filter.Length; x++)
                filter[x] = 1;
            filter[14] = 5;
            filter[16] = 5;

            // Nacte nastaveni
            if (ini.Exists())
            {
                try
                {
                    ini.Load();
                    settings[0] = ini["MAIN"]["gpxfile"];
                    settings[1] = ini["MAIN"]["mapfile"];
                    settings[2] = ini["MAIN"]["unit"];
                    settings[3] = ini["MAIN"]["coordinate"];
                    
                    selectedCache = Convert.ToInt32(ini["MAIN"]["cache"]);
                    selectedMap = Convert.ToInt32(ini["MAIN"]["map"]);

                    settingsShow[0] = MakeBool(ini["SHOW"]["map"]);
                    settingsShow[1] = MakeBool(ini["SHOW"]["logs"]);
                    settingsShow[2] = MakeBool(ini["SHOW"]["hint"]);
                    settingsShow[3] = MakeBool(ini["SHOW"]["description"]);
                    settingsShow[4] = MakeBool(ini["SHOW"]["name"]);
                    settingsShow[5] = MakeBool(ini["SHOW"]["distance"]);

                    filter[0] = Convert.ToInt32(ini["FILTER"]["traditional"]);
                    filter[1] = Convert.ToInt32(ini["FILTER"]["multi"]);
                    filter[2] = Convert.ToInt32(ini["FILTER"]["unknown"]);
                    filter[3] = Convert.ToInt32(ini["FILTER"]["letterbox"]);
                    filter[4] = Convert.ToInt32(ini["FILTER"]["earthcache"]);
                    filter[5] = Convert.ToInt32(ini["FILTER"]["othercache"]);
                    filter[6] = Convert.ToInt32(ini["FILTER"]["micro"]);
                    filter[7] = Convert.ToInt32(ini["FILTER"]["small"]);
                    filter[8] = Convert.ToInt32(ini["FILTER"]["regular"]);
                    filter[9] = Convert.ToInt32(ini["FILTER"]["large"]);
                    filter[10] = Convert.ToInt32(ini["FILTER"]["othercontainer"]);
                    filter[11] = Convert.ToInt32(ini["FILTER"]["notavailable"]);
                    filter[12] = Convert.ToInt32(ini["FILTER"]["archived"]);
                    filter[13] = Convert.ToInt32(ini["FILTER"]["difficultymin"]);
                    filter[14] = Convert.ToInt32(ini["FILTER"]["difficultymax"]);
                    filter[15] = Convert.ToInt32(ini["FILTER"]["terrainmin"]);
                    filter[16] = Convert.ToInt32(ini["FILTER"]["terrainmax"]);
                }
                catch
                {
                    // Vycisti INI
                    ini.Clear();
                    err = true;
                }
            }

            // Vytvori novy INI
            if (!ini.Exists() || err)
            {
                ini.Add("MAIN");
                ini["MAIN"].Add("gpxfile=");
                ini["MAIN"].Add("mapfile=");
                ini["MAIN"].Add("unit=meter");
                ini["MAIN"].Add("coordinate=deg");
                ini["MAIN"].Add("cache=-1");
                ini["MAIN"].Add("map=0");
                ini.Add("SHOW");
                ini["SHOW"].Add("map=1");
                ini["SHOW"].Add("logs=1");
                ini["SHOW"].Add("hint=1");
                ini["SHOW"].Add("description=1");
                ini["SHOW"].Add("name=1");
                ini["SHOW"].Add("distance=1");
                ini.Add("FILTER");
                ini["FILTER"].Add("traditional=1");
                ini["FILTER"].Add("multi=1");
                ini["FILTER"].Add("unknown=1");
                ini["FILTER"].Add("letterbox=1");
                ini["FILTER"].Add("earthcache=1");
                ini["FILTER"].Add("othercache=1");
                ini["FILTER"].Add("micro=1");
                ini["FILTER"].Add("small=1");
                ini["FILTER"].Add("regular=1");
                ini["FILTER"].Add("large=1");
                ini["FILTER"].Add("othercontainer=1");
                ini["FILTER"].Add("notavailable=1");
                ini["FILTER"].Add("archived=1");
                ini["FILTER"].Add("difficultymin=1");
                ini["FILTER"].Add("difficultymax=5");
                ini["FILTER"].Add("terrainmin=1");
                ini["FILTER"].Add("terrainmax=5");
                ini.Save();
            }
        }

        /// <summary>
        /// Ulozi nastaveni programu do INI
        /// </summary>
        private void IniSave()
        {
            ini["MAIN"]["gpxfile"] = settings[0];
            ini["MAIN"]["mapfile"] = settings[1];
            ini["MAIN"]["unit"] = settings[2];
            ini["MAIN"]["coordinate"] = settings[3];
            ini["MAIN"]["cache"] = selectedCache.ToString();
            ini["MAIN"]["map"] = selectedMap.ToString();

            ini["SHOW"]["map"] = MakeStringBool(settingsShow[0]);
            ini["SHOW"]["logs"] = MakeStringBool(settingsShow[1]);
            ini["SHOW"]["hint"] = MakeStringBool(settingsShow[2]);
            ini["SHOW"]["description"] = MakeStringBool(settingsShow[3]);
            ini["SHOW"]["name"] = MakeStringBool(settingsShow[4]);
            ini["SHOW"]["distance"] = MakeStringBool(settingsShow[5]);

            ini["FILTER"]["traditional"] = filter[0].ToString();
            ini["FILTER"]["multi"] = filter[1].ToString();
            ini["FILTER"]["unknown"] = filter[2].ToString();
            ini["FILTER"]["letterbox"] = filter[3].ToString();
            ini["FILTER"]["earthcache"] = filter[4].ToString();
            ini["FILTER"]["othercache"] = filter[5].ToString();
            ini["FILTER"]["micro"] = filter[6].ToString();
            ini["FILTER"]["small"] = filter[7].ToString();
            ini["FILTER"]["regular"] = filter[8].ToString();
            ini["FILTER"]["large"] = filter[9].ToString();
            ini["FILTER"]["othercontainer"] = filter[10].ToString();
            ini["FILTER"]["notavailable"] = filter[11].ToString();
            ini["FILTER"]["archived"] = filter[12].ToString();
            ini["FILTER"]["difficultymin"] = filter[13].ToString();
            ini["FILTER"]["difficultymax"] = filter[14].ToString();
            ini["FILTER"]["terrainmin"] = filter[15].ToString();
            ini["FILTER"]["terrainmax"] = filter[16].ToString();

            ini.Save();
        }
        
        /// <summary>
        /// Zmena pozice
        /// </summary>
        protected void gps_LocationChanged(object sender, LocationChangedEventArgs args)
        {
            position = args.Position;
            // call the UpdateData method via the updateDataHandler so that we update the UI on the UI thread
            try
            {
                Invoke(updateDataHandler);
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Zmena dat v zarizeni
        /// </summary>
        void gps_DeviceStateChanged(object sender, DeviceStateChangedEventArgs args)
        {
            device = args.DeviceState;
            // call the UpdateData method via the updateDataHandler so that we update the UI on the UI thread
            try
            {
                Invoke(updateDataHandler);
            }
            catch (NullReferenceException)
            {
            }
            
        }

        /// <summary>
        /// Zpracovava data z GPS prijimace
        /// </summary>
        void UpdateData(object sender, System.EventArgs args)
        {
            if (gps.Opened)
            {
                string str = "";

                /*
                if (device != null)
                    str = "GPS state: " + device.FriendlyName + " " + device.ServiceState + ", " + device.DeviceState + "\n";
                */

                if (position != null)
                {
                    // Latitude
                    if (position.LatitudeValid)
                    {
                        switch (settings[3])
                        {
                            case "degmin":
                                str += "Latitude: " + new Deg2DegMin(position.Latitude, (int)orientation.LAT) + "\n";
                                label1.Text = "" + new Deg2DegMin(position.Latitude, (int)orientation.LAT);
                                break;
                            case "degminsec":
                                str += "Latitude: " + new Deg2DegMinSec(position.Latitude, (int)orientation.LAT) + "\n";
                                label1.Text = "" + new Deg2DegMinSec(position.Latitude, (int)orientation.LAT);
                                break;
                            default:
                                str += "Latitude: " + new Deg2Deg(position.Latitude, (int)orientation.LAT) + "\n";
                                label1.Text = "" + new Deg2Deg(position.Latitude, (int)orientation.LAT);
                                break;
                        }
                    }

                    // Longitude
                    if (position.LongitudeValid)
                    {
                        switch (settings[3])
                        {
                            case "degmin":
                                str += "Longitude: " + new Deg2DegMin(position.Longitude, (int)orientation.LON) + "\n";
                                label2.Text = "" + new Deg2DegMin(position.Longitude, (int)orientation.LON);
                                break;
                            case "degminsec":
                                str += "Longitude: " + new Deg2DegMinSec(position.Longitude, (int)orientation.LON) + "\n";
                                label2.Text = "" + new Deg2DegMinSec(position.Longitude, (int)orientation.LON);
                                break;
                            default:
                                str += "Longitude: " + new Deg2Deg(position.Longitude, (int)orientation.LON) + "\n";
                                label2.Text = "" + new Deg2Deg(position.Longitude, (int)orientation.LON);
                                break;
                        }
                    }

                    // Altitude
                    if (position.SeaLevelAltitudeValid)
                        str += "Altitude: " + position.SeaLevelAltitude + " m\n";

                    /*
                    // Ellipsoid altitude
                    if (position.EllipsoidAltitudeValid)
                        str += "Ellipsoid altitude: " + position.EllipsoidAltitude + " m\n";
                    */

                    // Azimut
                    if (position.HeadingValid)
                        str += "Heading: " + position.Heading + "\n";

                    // Rychlost
                    if (position.SpeedValid)
                    {
                        switch (settings[2])
                        {
                            case "mile":
                                str += "Speed: " + Math.Round(position.Speed * 1.15077945, 1) + " mph\n";
                                break;
                            case "nautical":
                                str += "Speed: " + Math.Round(position.Speed, 1) + " knots\n";
                                break;
                            default:
                                str += "Speed: " + Math.Round(position.Speed * 1.852, 1) + " km/h\n";
                                break;
                        }
                    }

                    // Cas
                    if (position.TimeValid)
                        str += "Time: " + position.Time.ToString() + "\n";

                    // Satelity
                    if (position.SatellitesInSolutionValid && position.SatellitesInViewValid && position.SatelliteCountValid)
                    {
                        Satellite[] satellites = null;
                        try
                        {
                            satellites = position.GetSatellitesInSolution();
                        }
                        catch (NullReferenceException)
                        {
                        }

                        if (satellites.Length > 0)
                        {
                            int count = position.GetSatellitesInSolution().Length;
                            if (count > 10) count = 10;
                            progressBar1.Value = count;

                            label5.Text = position.GetSatellitesInSolution().Length + "/" + position.GetSatellitesInView().Length;
                        }
                        
                        str += "Satellite count: " + position.GetSatellitesInSolution().Length + "/" + position.GetSatellitesInView().Length + "\n";
                    }

                    // Signal
                    if (position.SatellitesInSolutionValid && position.SatellitesInViewValid && position.SatelliteCountValid)
                    {
                        string test = "";
                        Satellite[] satellites = position.GetSatellitesInSolution();
                        for (int i = 0; i < satellites.Length; i++)
                        {
                            test += satellites[i].SignalStrength.ToString();
                            if (i < satellites.Length - 1) test += "/";
                        }

                        str += "Signal: " + test + "\n";
                    }

                    if (position.PositionDilutionOfPrecisionValid)
                        str += "PDOP: " + position.PositionDilutionOfPrecision + "\n";

                    if (position.HorizontalDilutionOfPrecisionValid)
                        str += "HDOP: " + position.HorizontalDilutionOfPrecision + "\n";

                    if (position.VerticalDilutionOfPrecisionValid)
                        str += "VDOP: " + position.VerticalDilutionOfPrecision + "\n";

                    // Nastaveni souradnic
                    lat = position.Latitude;
                    lon = position.Longitude;

                    // Vypis statistik
                    label7.Text = str;

                    // Vykresleni mapy
                    if (tabControl1.SelectedIndex == 0)
                    {
                        DrawMap();
                        DrawCaches();
                        DrawMeasure();
                        DrawArrow((int)position.Heading);
                    }
                }
                // Nezamerena pozice
                else
                {
                    //if (device != null) label1.Text = device.DeviceState.ToString();
                    //else label1.Text = "No signal";
                    label1.Text = "No GPS fix";
                } 
            }
        }

        /// <summary>
        /// Vykresleni mapoveho podkladu
        /// </summary>
        private void DrawMap()
        {
            Map map = (Map)maps[selectedMap];

            // Vzdalenost aktualni pozice od pocatku mapy
            double positionLatDiff = map.Lat1 - lat;
            double positionLonDiff = lon - map.Lon1;

            // Vzdalenost aktualni pozice od pocatku mapy v pixelech
            double positionLatDiffPixel = positionLatDiff / map.PixelLat;
            double positionLonDiffPixel = positionLonDiff / map.PixelLon;

            // Aktualni souradnice
            int x = (int)positionLonDiffPixel;
            int y = (int)positionLatDiffPixel;

            // Zoom
            double zoom = zoomList[zoomIndex];

            // Vykresleni mapy
            g = pictureBox1.CreateGraphics();

            double a = x - (pictureBox1.Width * zoom / 2);
            double b = y - (pictureBox1.Height * zoom / 2);

            if (settingsShow[0])
            {
                double w = pictureBox1.Width * zoom;
                double h = pictureBox1.Height * zoom;

                Rectangle srcRect = new Rectangle((int)a, (int)b, (int)w, (int)h);
                Rectangle destRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                GraphicsUnit units = GraphicsUnit.Pixel;
                if(mapImage != null) g.DrawImage(mapImage, destRect, srcRect, units);

                // Vykresleni prazdnoty tam kde neni mapa
                if (a < 0)
                {
                    g.FillRectangle(brush1, new Rectangle(0, 0, (int)Math.Abs(a/zoom), pictureBox1.Height));
                }
                if (b < 0)
                {
                    g.FillRectangle(brush1, new Rectangle(0, 0, pictureBox1.Width, (int)Math.Abs(b / zoom)));
                }
                if (a + w > map.Width)
                {
                    g.FillRectangle(brush1, new Rectangle(pictureBox1.Width - (int)((a + w - map.Width) / zoom), 0, pictureBox1.Width, pictureBox1.Height));
                }
                if (b + h > map.Height)
                {
                    g.FillRectangle(brush1, new Rectangle(0, pictureBox1.Height - (int)((b + h - map.Height) / zoom), pictureBox1.Width, pictureBox1.Height));
                }
            }
            else
                g.Clear(Color.Black);

            //g.Dispose();
        }

        /// <summary>
        /// Vykresleni cache do mapy
        /// </summary>
        private void DrawCaches()
        {
            Map map = (Map)maps[selectedMap];
            Brush brush = brush2;
            Pen pen;

            // Vzdalenost aktualni pozice od pocatku mapy
            double positionLatDiff = map.Lat1 - lat;
            double positionLonDiff = lon - map.Lon1;

            // Vzdalenost aktualni pozice od pocatku mapy v pixelech
            double positionLatDiffPixel = positionLatDiff / map.PixelLat;
            double positionLonDiffPixel = positionLonDiff / map.PixelLon;

            // Aktualni souradnice
            int x = (int)positionLonDiffPixel;
            int y = (int)positionLatDiffPixel;

            // Zoom
            double zoom = zoomList[zoomIndex];

            // Vykresleni kesi
            g = pictureBox1.CreateGraphics();
            double a = x - (pictureBox1.Width * zoom / 2);
            double b = y - (pictureBox1.Height * zoom / 2);
            int cnt = 0;

            foreach (Cache element in caches)
            {
                // Nastaveni barev
                switch (element.Type)
                {
                    case (int)cacheType.TRADITIONAL:
                        //brush = brush4;
                        pen = pen4;
                        break;
                    case (int)cacheType.MULTI:
                        //brush = brush5;
                        pen = pen5;
                        break;
                    case (int)cacheType.UNKNOWN:
                        //brush = brush6;
                        pen = pen6;
                        break;
                    case (int)cacheType.LETTERBOX:
                        //brush = brush7;
                        pen = pen7;
                        break;
                    case (int)cacheType.EARTHCACHE:
                        //brush = brush8;
                        pen = pen8;
                        break;
                    default:
                        //brush = brush9;
                        pen = pen9;
                        break;
                }

                if (!element.View) continue; // Nevykresli vyfiltrovane cache

                // Souradnice cache
                double cache_lat = element.Latitude;
                double cache_lon = element.Longitude;

                // Vzdalenost cache od aktualni pozice
                double cache_lat_diff = lat - cache_lat;
                double cache_lon_diff = cache_lon - lon;

                // Vzdalenost cache od aktualni pozice v pixelech
                double cache_lat_diff_pixel = cache_lat_diff / map.PixelLat;
                double cache_lon_diff_pixel = cache_lon_diff / map.PixelLon;

                // Souradnice kese v mape
                int line_x = x + (int)cache_lon_diff_pixel;
                int line_y = y + (int)cache_lat_diff_pixel;

                // Souradnice na obrazovce
                double line_x_screen = (line_x - a) / zoom;
                double line_y_screen = (line_y - b) / zoom;

                // Vzdalenost cache
                double pyth1 = Math.Abs(x - line_x) * map.PixelLength;
                double pyth2 = Math.Abs(y - line_y) * map.PixelLength;
                double cache_length = Math.Sqrt(Math.Pow(pyth1, 2) + Math.Pow(pyth2, 2));

                // Navadeci cara a vzdalenost
                if (cnt == selectedCache)
                    g.DrawLine(pen1, pictureBox1.Width / 2, pictureBox1.Height / 2, (int)line_x_screen, (int)line_y_screen);

                // Vyobrazeni cache
                g.DrawLine(pen, (int)line_x_screen, (int)line_y_screen - 4, (int)line_x_screen, (int)line_y_screen + 4);

                // Zobrazeni nazvu cache
                if (settingsShow[4])
                    g.DrawString(element.Name, font1, brush, new RectangleF((int)line_x_screen + 8, (int)line_y_screen - 7, 100, 15));

                // Zobrazeni vzdalenosti cache
                if (settingsShow[5] || cnt == selectedCache)
                {
                    string cache_length_str = "";
                    switch (settings[2])
                    {
                        case "mile":
                            cache_length_str = Math.Round(cache_length/1609.344, 3).ToString() + " mile";
                            break;
                        case "nautical":
                            cache_length_str = Math.Round(cache_length/1852, 3).ToString() + " kn";
                            break;
                        default:
                            cache_length_str = Math.Round(cache_length, 1).ToString() + " m";
                            break;
                    }
                    if (settingsShow[5])
                        g.DrawString(cache_length_str, font1, brush, new RectangleF((int)line_x_screen + 8, (int)line_y_screen + 4, 100, 15));
                    if (cnt == selectedCache)
                        g.DrawString(cache_length_str, font1, brush2, new RectangleF(4, 4, 100, 20));
                }
                cnt++;
            }
            //g.Dispose();
        }

        /// <summary>
        /// Vykresleni meritka do mapy
        /// </summary>
        private void DrawMeasure()
        {
            Map map = (Map)maps[selectedMap];

            // Zoom
            double zoom = zoomList[zoomIndex];

            g = pictureBox1.CreateGraphics();

            // Meritko
            int meters = (int)(40 * map.PixelLength); // 40 pixelu
            while (meters % 10 != 0) meters++;
            double measure = meters / map.PixelLength;
            g.DrawLine(pen2, 5, pictureBox1.Height - 9, 5 + (int)measure, pictureBox1.Height - 9);
            g.DrawLine(pen2, 5, pictureBox1.Height - 12, 5, pictureBox1.Height - 6);
            g.DrawLine(pen2, 5 + (int)measure, pictureBox1.Height - 12, 5 + (int)measure, pictureBox1.Height - 6 );
            
            string unit_str = "";
            switch (settings[2])
            {
                case "mile":
                    unit_str = Math.Round((meters / 1609.344 * zoom), 3).ToString() + " mile";
                    break;
                case "nautical":
                    unit_str = Math.Round((meters / 1852.0 * zoom), 3).ToString() + " kn";
                    break;
                default:
                    unit_str = (meters * zoom).ToString() + " m";
                    break;
            }
            g.DrawString(unit_str, font1, brush2, new RectangleF(6, pictureBox1.Height - 25, 60, 12));
            //g.Dispose();
        }

        /// <summary>
        /// Vykresleni navadeci sipky do mapy
        /// </summary>
        private void DrawArrow(int angle)
        {
            g = pictureBox1.CreateGraphics();

            double sipkaX = pictureBox1.Width/2;
            double sipkaY = pictureBox1.Height / 2;
            double sipkaHalfSize = 12; // polovicni velikost sipky

            double rads = angle * Math.PI / 180;
            double vectorX = sipkaHalfSize * Math.Sin(rads);
            double vectorY = sipkaHalfSize * Math.Cos(rads);

            g.DrawLine(pen3, (int)(sipkaX - vectorX), (int)(sipkaY + vectorY), (int)(sipkaX + vectorX), (int)(sipkaY - vectorY)); // cara ve smeru sipky

            g.DrawLine(pen3, (int)(sipkaX + vectorY / 2), (int)(sipkaY + vectorX / 2), (int)(sipkaX + vectorX), (int)(sipkaY - vectorY)); // pravy rameno sipky
            g.DrawLine(pen3, (int)(sipkaX - vectorY / 2), (int)(sipkaY - vectorX / 2), (int)(sipkaX + vectorX), (int)(sipkaY - vectorY)); // levy rameno sipky

            //g.Dispose();
        }

        /// <summary>
        /// Zmena mapoveho podkladu
        /// </summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMap = comboBox1.SelectedIndex;
            Map map = (Map)maps[selectedMap];
            try
            {
                mapImage = new Bitmap(pathMap + map.File);
                zoomIndex = 2;
            }
            catch
            {
                mapImage = null;
            }
        }

        /// <summary>
        /// Tlacitko pro nacteni mapovych podkladu z XML souboru
        /// </summary>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog2.ShowDialog();

            if (res == DialogResult.OK)
            {
                settings[1] = openFileDialog2.FileName;
                MapLoad();
                selectedMap = 0;
            }
        }

        /// <summary>
        /// Tlacitko pro nastaveni programu
        /// </summary>
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Settings form = new Settings(settings, settingsShow);
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                UpdateCache();
                IniSave();
            }
        }

        /// <summary>
        /// Tlacitko pro zobrazeni informaci about
        /// </summary>
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            string message = "Geo Navigator v1.0\n©2009 Petr Nohejl\nsoftware.8bit.cz";
            MessageBox.Show(message, "About Geo Navigator", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Tlacitko pro vypnuti programu
        /// </summary>
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (gps.Opened)
            {
                gps.Close();
            }

            IniSave();
            this.Close();
        }

        /// <summary>
        /// Tlacitko pro vypnuti/zapnuti GPS
        /// </summary>
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            if (!gps.Opened)
            {
                gps.Open();
                try { pictureBox9.Image = new Bitmap(path + "\\Images\\gps_on.png"); }
                catch { }
            }
            else
            {
                gps.Close();
                label7.Text = "";
                try { pictureBox9.Image = new Bitmap(path + "\\Images\\gps_off.png"); }
                catch { }
                label1.Text = "GPS off";
                label2.Text = "";
                label5.Text = "0/0";
                progressBar1.Value = 0;
            }
        }

        /// <summary>
        /// Tlacitko Zoom +
        /// </summary>
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            if (zoomIndex > 0) zoomIndex--;

            if (!gps.Opened && selectedCache != -1)
            {
                Cache c = (Cache)caches[selectedCache];
                lat = c.Latitude;
                lon = c.Longitude;
                DrawMap();
                DrawCaches();
                DrawMeasure();
            }
        }

        /// <summary>
        /// Tlacitko Zoom -
        /// </summary>
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            if (zoomIndex < zoomList.Length - 1) zoomIndex++;

            if (!gps.Opened && selectedCache != -1)
            {
                Cache c = (Cache)caches[selectedCache];
                lat = c.Latitude;
                lon = c.Longitude;
                DrawMap();
                DrawCaches();
                DrawMeasure();
            }
        }

        /// <summary>
        /// Tlacitko pro nacteni informaci o vybrane cache
        /// </summary>
        private void pictureBox12_Click(object sender, EventArgs e)
        {
            UpdateCache();
        }

        /// <summary>
        /// Tlacitko pro pridani cache do databaze
        /// </summary>
        private void pictureBox13_Click(object sender, EventArgs e)
        {
            // Neni vybran zadny GPX soubor
            if (settings[0] == "")
            {
                settings[0] =  path + "\\default.xml";
                
                // Vytvori novy GPX soubor
                if (!File.Exists(settings[0]))
                {
                    Gpx gpxDoc = new Gpx(settings[0]);
                    gpxDoc.CreateGpx();
                }
            }

            AddCache form = new AddCache(settings[0]);
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
                DatabaseLoad();
        }

        /// <summary>
        /// Tlacitko pro odstraneni cache z databaze
        /// </summary>
        private void pictureBox14_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string message = "Do you really want to remove selected cache from GPX database file?";
                string caption = "Delete cache";
                DialogResult result;

                result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes)
                {
                    selectedCache = SelectedCache();
                    Cache selected = (Cache)caches[selectedCache];
                    Gpx gpxDoc = new Gpx(settings[0]);

                    try
                    {
                        gpxDoc.CacheDelete(selected.Code);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        message = "Database file " + gpxDoc.GpxFile + " doesn't exist!";
                        caption = "Error";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        DatabaseDeselect();
                    }
                    catch
                    {
                        string message2 = "Error! Cannot remove database data!";
                        string caption2 = "Error";
                        MessageBox.Show(message2, caption2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        DatabaseDeselect();
                    }

                    DatabaseLoad();
                }
            }
        }

        /// <summary>
        /// Tlacitko pro nastaveni filtru cache
        /// </summary>
        private void pictureBox15_Click(object sender, EventArgs e)
        {
            Filter form = new Filter(filter);
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                DatabaseLoad();
                IniSave();
            }
        }

        /// <summary>
        /// Tlacitko pro refresh databaze cache
        /// </summary>
        private void pictureBox16_Click(object sender, EventArgs e)
        {
            DatabaseLoad();
        }

        /// <summary>
        /// Tlacitko pro nacteni GPX databaze s cache
        /// </summary>
        private void pictureBox17_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                settings[0] = openFileDialog1.FileName;
                DatabaseLoad();
            }
        }

        /// <summary>
        /// Tlacitko pro odznaceni databaze s cache
        /// </summary>
        private void pictureBox18_Click(object sender, EventArgs e)
        {
            DatabaseDeselect();
        }

        // Nastaveni obrazkovych tlacitek
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox2.Image = new Bitmap(path + "\\Images\\map2.png"); }
            catch { ;}
        }
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox2.Image = new Bitmap(path + "\\Images\\map.png"); }
            catch { ;}
        }
        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox3.Image = new Bitmap(path + "\\Images\\settings2.png"); }
            catch { ;}
        }
        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox3.Image = new Bitmap(path + "\\Images\\settings.png"); }
            catch { ;}
        }
        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox4.Image = new Bitmap(path + "\\Images\\about2.png"); }
            catch { ;}
        }
        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox4.Image = new Bitmap(path + "\\Images\\about.png"); }
            catch { ;}
        }
        private void pictureBox5_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox5.Image = new Bitmap(path + "\\Images\\exit2.png"); }
            catch { ;}
        }
        private void pictureBox5_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox5.Image = new Bitmap(path + "\\Images\\exit.png"); }
            catch { ;}
        }
        private void pictureBox6_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox6.Image = new Bitmap(path + "\\Images\\stop.png"); }
            catch { ;}
        }
        private void pictureBox6_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox6.Image = new Bitmap(path + "\\Images\\start.png"); }
            catch { ;}
        }
        private void pictureBox7_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox7.Image = new Bitmap(path + "\\Images\\reset2.png"); }
            catch { ;}
        }
        private void pictureBox7_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox7.Image = new Bitmap(path + "\\Images\\reset.png"); }
            catch { ;}
        }
        private void pictureBox8_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox8.Image = new Bitmap(path + "\\Images\\graph2.png"); }
            catch { ;}
        }
        private void pictureBox8_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox8.Image = new Bitmap(path + "\\Images\\graph.png"); }
            catch { ;}
        }
        private void pictureBox10_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox10.Image = new Bitmap(path + "\\Images\\zoom_in2.png"); }
            catch { ;}
        }
        private void pictureBox10_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox10.Image = new Bitmap(path + "\\Images\\zoom_in.png"); }
            catch { ;}
        }
        private void pictureBox11_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox11.Image = new Bitmap(path + "\\Images\\zoom_out2.png"); }
            catch { ;}
        }
        private void pictureBox11_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox11.Image = new Bitmap(path + "\\Images\\zoom_out.png"); }
            catch { ;}
        }
        private void pictureBox12_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox12.Image = new Bitmap(path + "\\Images\\select2.png"); }
            catch { ;}
        }
        private void pictureBox12_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox12.Image = new Bitmap(path + "\\Images\\select.png"); }
            catch { ;}
        }
        private void pictureBox13_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox13.Image = new Bitmap(path + "\\Images\\add2.png"); }
            catch { ;}
        }
        private void pictureBox13_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox13.Image = new Bitmap(path + "\\Images\\add.png"); }
            catch { ;}
        }
        private void pictureBox14_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox14.Image = new Bitmap(path + "\\Images\\delete2.png"); }
            catch { ;}
        }
        private void pictureBox14_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox14.Image = new Bitmap(path + "\\Images\\delete.png"); }
            catch { ;}
        }
        private void pictureBox15_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox15.Image = new Bitmap(path + "\\Images\\filter2.png"); }
            catch { ;}
        }
        private void pictureBox15_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox15.Image = new Bitmap(path + "\\Images\\filter.png"); }
            catch { ;}
        }
        private void pictureBox16_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox16.Image = new Bitmap(path + "\\Images\\refresh2.png"); }
            catch { ;}
        }
        private void pictureBox16_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox16.Image = new Bitmap(path + "\\Images\\refresh.png"); }
            catch { ;}
        }
        private void pictureBox17_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox17.Image = new Bitmap(path + "\\Images\\load2.png"); }
            catch { ;}
        }
        private void pictureBox17_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox17.Image = new Bitmap(path + "\\Images\\load.png"); }
            catch { ;}
        }
        private void pictureBox18_MouseDown(object sender, MouseEventArgs e)
        {
            try { pictureBox18.Image = new Bitmap(path + "\\Images\\deselect2.png"); }
            catch { ;}
        }
        private void pictureBox18_MouseUp(object sender, MouseEventArgs e)
        {
            try { pictureBox18.Image = new Bitmap(path + "\\Images\\deselect.png"); }
            catch { ;}
        }

        /// <summary>
        /// Prevod retezce na bool 
        /// </summary>
        private bool MakeBool(string str)
        {
            if (str == "1")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Prevod bool na retezec
        /// </summary>
        private string MakeStringBool(bool val)
        {
            if (val)
                return "1";
            else
                return "0";
        }

        /// <summary>
        /// Zavreni okna
        /// </summary>
        private void MainForm_Closed(object sender, System.EventArgs e)
        {
            if (gps.Opened)
            {
                gps.Close();
            }

            IniSave();
            this.Close();
        }
    }
}
