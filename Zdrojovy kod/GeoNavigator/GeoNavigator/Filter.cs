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
    /// Okno s filtry cache
    /// </summary>
    public partial class Filter : Form
    {
        // Filtry
        private int[] filter;

        /// <summary>
        /// Vytvori okno s filtry
        /// </summary>
        public Filter(int[] appFilter)
        {
            InitializeComponent();
            filter = appFilter;

            // Zobrazi formularove prvky podle aktualniho nastaveni
            if (filter[0] == 1) checkBox1.Checked = true;
            if (filter[1] == 1) checkBox2.Checked = true;
            if (filter[2] == 1) checkBox3.Checked = true;
            if (filter[3] == 1) checkBox4.Checked = true;
            if (filter[4] == 1) checkBox5.Checked = true;
            if (filter[5] == 1) checkBox6.Checked = true;
            if (filter[6] == 1) checkBox7.Checked = true;
            if (filter[7] == 1) checkBox8.Checked = true;
            if (filter[8] == 1) checkBox9.Checked = true;
            if (filter[9] == 1) checkBox10.Checked = true;
            if (filter[10] == 1) checkBox11.Checked = true;
            if (filter[11] == 1) checkBox12.Checked = true;
            if (filter[12] == 1) checkBox13.Checked = true;
            numericUpDown1.Value = filter[13];
            numericUpDown2.Value = filter[14];
            numericUpDown3.Value = filter[15];
            numericUpDown4.Value = filter[16];
        }

        /// <summary>
        /// Stisknuti tlacitka OK
        /// </summary>
        private void menuItem1_Click(object sender, EventArgs e)
        {
            // Pole s obtiznosti a terenem vyplneno spravne
            if (numericUpDown1.Value <= numericUpDown2.Value && numericUpDown3.Value <= numericUpDown4.Value)
            {
                // Nastavi filtr
                filter[0] = Convert.ToInt32(checkBox1.Checked);
                filter[1] = Convert.ToInt32(checkBox2.Checked);
                filter[2] = Convert.ToInt32(checkBox3.Checked);
                filter[3] = Convert.ToInt32(checkBox4.Checked);
                filter[4] = Convert.ToInt32(checkBox5.Checked);
                filter[5] = Convert.ToInt32(checkBox6.Checked);
                filter[6] = Convert.ToInt32(checkBox7.Checked);
                filter[7] = Convert.ToInt32(checkBox8.Checked);
                filter[8] = Convert.ToInt32(checkBox9.Checked);
                filter[9] = Convert.ToInt32(checkBox10.Checked);
                filter[10] = Convert.ToInt32(checkBox11.Checked);
                filter[11] = Convert.ToInt32(checkBox12.Checked);
                filter[12] = Convert.ToInt32(checkBox13.Checked);
                filter[13] = (int)numericUpDown1.Value;
                filter[14] = (int)numericUpDown2.Value;
                filter[15] = (int)numericUpDown3.Value;
                filter[16] = (int)numericUpDown4.Value;

                // Uzavreni okna
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // Obtiznost nebo teren spatne nastaveny
                string message = "Cannot set filter! Set correct interval for difficulty and terrain!";
                string caption = "Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Stisknuti tlacitka Cancel
        /// </summary>
        private void menuItem2_Click(object sender, EventArgs e)
        {
            // Uzavreni okna
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Stisknuti tlacitka Reset
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // Implicitni nastaveni
            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox3.Checked = true;
            checkBox4.Checked = true;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
            checkBox7.Checked = true;
            checkBox8.Checked = true;
            checkBox9.Checked = true;
            checkBox10.Checked = true;
            checkBox11.Checked = true;
            checkBox12.Checked = true;
            checkBox13.Checked = true;
            numericUpDown1.Value = 1;
            numericUpDown2.Value = 5;
            numericUpDown3.Value = 1;
            numericUpDown4.Value = 5;
        }
    }
}
