using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Anode
{
    public partial class AnodeOptions : Form
    {
        int[] languagesUsed =
        {
            1031, 1033, 1034, 1035, 1040, 1041, 1042, 1043, 2052, 2057, -1, -2, -3
        };

        int[] LangOrder;

        string[] altLanguages =
        {
            "Pirate Speak",
            "English (Shakespearian)",
            "LOLCAT"
        };

        public AnodeOptions()
        {
            InitializeComponent();

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.Locale);

            switch (Properties.Settings.Default.NESCore)
            {
                case 0:
                    radioButton1.Checked = true;
                    break;
                case 1:
                    radioButton2.Checked = true;
                    break;
            }


            // Stupidly long thing for a simple thing
            LanguageBox.Text = CultureInfo.CurrentCulture.DisplayName;
            LanguageBox.Items.Clear();
            LangOrder = new int[languagesUsed.Length];
            int LangIndex = 0;
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                //Console.WriteLine(ci.);
                if (languagesUsed.Contains(ci.LCID))
                {
                    Console.WriteLine(ci.Name);
                    LangOrder[LangIndex] = ci.LCID;
                    LangIndex++;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                LangOrder[LangIndex] = (-i)-1;
                LangIndex++;
            }

            foreach (int i in LangOrder)
            {
                if (i > 0)
                {
                    LanguageBox.Items.Add(CultureInfo.GetCultureInfo(i));
                }
                else if (i == 0)
                {
                    LanguageBox.Items.Add("Unknown Language");
                }
                else
                {
                    LanguageBox.Items.Add(altLanguages[-(i + 1)]);
                }
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

        private void LanguageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Locale = LanguageBox.SelectedIndex;
            Properties.Settings.Default.Save();
        }
    }
}
