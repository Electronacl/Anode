using System;
using System.Windows.Forms;

/*
Copyright © 2026 Electronacl

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the “Software”), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace Anode
{
    public partial class AnodeAbout : Form
    {
        public AnodeAbout()
        {
            InitializeComponent();
            versionText.Text = "Version: " + Constants.version_name;
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

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel2.LinkVisited = true;
                OpenLink("https://github.com/Electronacl/Anode/blob/main/LICENSE");
            }
            catch
            {
                MessageBox.Show("An error occurred when trying to open the link");
            }
        }
    }
}
