using System;
using System.Windows.Forms;

namespace Anode
{
    public partial class AnodeAbout : Form
    {
        public AnodeAbout()
        {
            InitializeComponent();
            versionText.Text = Constants.version_name;
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

        private void versionText_Click(object sender, EventArgs e)
        {
            try
            {
                // Ehehehehehe
                OpenLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            }
            catch
            {
                // Do nothing
            }
        }
    }
}
