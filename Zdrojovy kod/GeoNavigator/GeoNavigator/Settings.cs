using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeoNavigator
{
    /// <summary>
    /// Okno nastaveni
    /// </summary>
    public partial class Settings : Form
    {
        // Obecne nastaveni a nastaveni zobrazeni
        private string[] settings;
        private bool[] settingsShow;

        /// <summary>
        /// Vytvori okno s nastavenim programu
        /// </summary>
        public Settings(string[] appSettings, bool[] appSettingsShow)
        {
            InitializeComponent();

            settings = appSettings;
            settingsShow = appSettingsShow;

            // Layout
            ArrayList list1 = new ArrayList();
            list1.Add("Meter");
            list1.Add("Miles");
            list1.Add("Nautical miles");
            comboBox1.DataSource = list1;

            ArrayList list2 = new ArrayList();
            list2.Add("Deg°");
            list2.Add("Deg°Min'");
            list2.Add("Deg°Min'Sec\"");
            comboBox2.DataSource = list2;

            // Aktualni hodnota
            switch(settings[2])
            {
                case "mile":
                    comboBox1.SelectedIndex = 1;
                    break;
                case "nautical":
                    comboBox1.SelectedIndex = 2;
                    break;
                default:
                    comboBox1.SelectedIndex = 0;
                    break;
            }

            switch (settings[3])
            {
                case "degmin":
                    comboBox2.SelectedIndex = 1;
                    break;
                case "degminsec":
                    comboBox2.SelectedIndex = 2;
                    break;
                default:
                    comboBox2.SelectedIndex = 0;
                    break;
            }

            if (settingsShow[0]) checkBox1.Checked = true;
            if (settingsShow[1]) checkBox2.Checked = true;
            if (settingsShow[2]) checkBox3.Checked = true;
            if (settingsShow[3]) checkBox4.Checked = true;
            if (settingsShow[4]) checkBox5.Checked = true;
            if (settingsShow[5]) checkBox6.Checked = true;
        }

        /// <summary>
        /// Stisknuti tlacitka OK
        /// </summary>
        private void menuItem1_Click(object sender, EventArgs e)
        {
            // Ulozeni nastaveni
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    settings[2] = "meter";
                    break;
                case 1:
                    settings[2] = "mile";
                    break;
                case 2:
                    settings[2] = "nautical";
                    break;
            }

            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    settings[3] = "deg";
                    break;
                case 1:
                    settings[3] = "degmin";
                    break;
                case 2:
                    settings[3] = "degminsec";
                    break;
            }

            settingsShow[0] = checkBox1.Checked;
            settingsShow[1] = checkBox2.Checked;
            settingsShow[2] = checkBox3.Checked;
            settingsShow[3] = checkBox4.Checked;
            settingsShow[4] = checkBox5.Checked;
            settingsShow[5] = checkBox6.Checked;

            // Uzavreni okna
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Stisknuti tlacitka Cancel
        /// </summary>
        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
