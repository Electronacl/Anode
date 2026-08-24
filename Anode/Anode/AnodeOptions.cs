using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anode
{
    public partial class AnodeOptions : Form
    {
        public AnodeOptions()
        {
            InitializeComponent();
            switch (Properties.Settings.Default.NESCore)
            {
                case 0:
                    radioButton1.Checked = true;
                    break;
                case 1:
                    radioButton2.Checked = true;
                    break;
            }
        }

        private void UpdateNESCore()
        {
            if (radioButton1.Checked)
            {
                Properties.Settings.Default.NESCore = 0;
            }
            else if (radioButton2.Checked)
            {
                Properties.Settings.Default.NESCore = 1;
            }
            Properties.Settings.Default.Save();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNESCore();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNESCore();
        }
    }
}
