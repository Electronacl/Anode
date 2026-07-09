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
    public partial class AnodeAbout : Form
    {
        public AnodeAbout()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel1.LinkVisited = true;
                OpenLink("https://github.com/Electronacl/Anode");
            }
            catch
            {
                MessageBox.Show("An error occurred when trying to open the link");
            }
        }

        private void OpenLink(string link)
        {
            System.Diagnostics.Process.Start(link);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel2.LinkVisited = true;
                OpenLink("https://github.com/Electronacl/Anode/issues");
            }
            catch
            {
                MessageBox.Show("An error occurred when trying to open the link");
            }
        }
    }
}
