using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace GeoNavigator
{
    /// <summary>
    /// Okno pro pridani nove cache
    /// </summary>
    public partial class AddCache : Form
    {
        // Vytvori GPX objekt
        Gpx gpxDoc;

        /// <summary>
        /// Vytvori okno pro pridani nove cache
        /// </summary>
        public AddCache(string cacheFile)
        {
            InitializeComponent();

            // Vytvori XML dokument
            gpxDoc = new Gpx(cacheFile);

            // Layout
            ArrayList list1 = new ArrayList();
            list1.Add("Deg°");
            list1.Add("Deg°Min'");
            list1.Add("Deg°Min'Sec\"");
            comboBox1.DataSource = list1;

            textBox2.Text = "00.000000";
            textBox3.Text = "00.000000";

            ArrayList list2 = new ArrayList();
            list2.Add("Traditional Cache");
            list2.Add("Multi-cache");
            list2.Add("Unknown Cache");
            list2.Add("Letterbox Hybrid");
            list2.Add("Wherigo Cache");
            list2.Add("Earthcache");
            list2.Add("Virtual Cache");
            list2.Add("Webcam Cache");
            list2.Add("Event Cache");
            list2.Add("Cache In Trash Out Event");
            list2.Add("Mega-Event Cache");
            list2.Add("Other");
            comboBox2.DataSource = list2;

            ArrayList list3 = new ArrayList();
            list3.Add("Micro");
            list3.Add("Small");
            list3.Add("Regular");
            list3.Add("Large");
            list3.Add("Other");
            comboBox3.DataSource = list3;

            ArrayList list4 = new ArrayList();
            for (int i = 1; i <= 5; i++ )
                list4.Add(i);
            comboBox4.DataSource = list4;

            ArrayList list5 = new ArrayList();
            for (int i = 1; i <= 5; i++)
                list5.Add(i);
            comboBox5.DataSource = list5;
        }

        /// <summary>
        /// Stisknuti tlacitka OK
        /// </summary>
        private void menuItem1_Click(object sender, EventArgs e)
        {
            string[] arr = new string[12];
            arr[0] = textBox1.Text;
            arr[1] = "";
            arr[2] = "";
            arr[3] = textBox4.Text;
            arr[6] = comboBox2.Text;
            arr[7] = comboBox3.Text;
            arr[8] = comboBox4.Text;
            arr[9] = comboBox5.Text;
            arr[10] = textBox5.Text;
            arr[11] = textBox6.Text;

            if(checkBox1.Checked)
                arr[4] = "True";
            else
                arr[4] = "False";

            if (checkBox2.Checked)
                arr[5] = "True";
            else
                arr[5] = "False";

            // kontrola souradnic
            bool err = false;

            if (comboBox1.SelectedIndex == 0)
            {
                Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
                if (regex.IsMatch(textBox2.Text) && regex.IsMatch(textBox3.Text))
                {
                    arr[1] = textBox2.Text;
                    arr[2] = textBox3.Text;
                }
                else
                {
                    err = true;
                }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                try
                {
                    arr[1] = ToDeg.DegMin2Deg(textBox2.Text);
                    arr[2] = ToDeg.DegMin2Deg(textBox3.Text);
                }
                catch
                {
                    err = true;
                }
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                try
                {
                    arr[1] = ToDeg.DegMinSec2Deg(textBox2.Text);
                    arr[2] = ToDeg.DegMinSec2Deg(textBox3.Text);
                }
                catch
                {
                    err = true;
                }
            }

            // kontrola formulare
            if (arr[0] == "" || arr[1] == "" || arr[2] == "" || arr[3] == "")
                err = true;

            if(err)
            {
                string message = "Cannot add new cache! Fill in name, correct coordinates and ID code!";
                string caption = "Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            else
            {
                // Pridani nove cache do seznamu
                try
                {
                    gpxDoc.CacheAdd(arr);
                    this.DialogResult = DialogResult.OK;
                }
                catch (System.IO.FileNotFoundException)
                {
                    string message = "Database file " + gpxDoc.GpxFile + " doesn't exist!";
                    string caption = "Error";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                catch
                {
                    string message = "Error! Cannot add data to database!";
                    string caption = "Error";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                
                this.Close();
            }
        }

        /// <summary>
        /// Stisknuti tlacitka Cancel
        /// </summary>
        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Zmena formatu souradnic
        /// </summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                textBox2.Text = "00.000000";
                textBox3.Text = "00.000000";
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                textBox2.Text = "00 00.0000";
                textBox3.Text = "00 00.0000";
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                textBox2.Text = "00 00 00.00";
                textBox3.Text = "00 00 00.00";
            }
        }
    }
}
