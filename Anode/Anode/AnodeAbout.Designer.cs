namespace Anode
{
    partial class AnodeAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnodeAbout));
            this.AnodeTitleText = new System.Windows.Forms.Label();
            this.AnodeSummaryText = new System.Windows.Forms.Label();
            this.CopyrightText = new System.Windows.Forms.Label();
            this.LinkGroup = new System.Windows.Forms.Panel();
            this.LicenseLink = new System.Windows.Forms.LinkLabel();
            this.BugLink = new System.Windows.Forms.LinkLabel();
            this.GHLink = new System.Windows.Forms.LinkLabel();
            this.LegalInfoText = new System.Windows.Forms.Label();
            this.HDiv1 = new System.Windows.Forms.Label();
            this.HDiv2 = new System.Windows.Forms.Label();
            this.versionText = new System.Windows.Forms.Label();
            this.HDiv3 = new System.Windows.Forms.Label();
            this.HDiv4 = new System.Windows.Forms.Label();
            this.LinkGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnodeTitleText
            // 
            resources.ApplyResources(this.AnodeTitleText, "AnodeTitleText");
            this.AnodeTitleText.Name = "AnodeTitleText";
            // 
            // AnodeSummaryText
            // 
            resources.ApplyResources(this.AnodeSummaryText, "AnodeSummaryText");
            this.AnodeSummaryText.Name = "AnodeSummaryText";
            // 
            // CopyrightText
            // 
            resources.ApplyResources(this.CopyrightText, "CopyrightText");
            this.CopyrightText.Name = "CopyrightText";
            // 
            // LinkGroup
            // 
            this.LinkGroup.Controls.Add(this.LicenseLink);
            this.LinkGroup.Controls.Add(this.BugLink);
            this.LinkGroup.Controls.Add(this.GHLink);
            resources.ApplyResources(this.LinkGroup, "LinkGroup");
            this.LinkGroup.Name = "LinkGroup";
            // 
            // LicenseLink
            // 
            resources.ApplyResources(this.LicenseLink, "LicenseLink");
            this.LicenseLink.Name = "LicenseLink";
            this.LicenseLink.TabStop = true;
            this.LicenseLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // BugLink
            // 
            resources.ApplyResources(this.BugLink, "BugLink");
            this.BugLink.Name = "BugLink";
            this.BugLink.TabStop = true;
            this.BugLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // GHLink
            // 
            resources.ApplyResources(this.GHLink, "GHLink");
            this.GHLink.Name = "GHLink";
            this.GHLink.TabStop = true;
            this.GHLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // LegalInfoText
            // 
            resources.ApplyResources(this.LegalInfoText, "LegalInfoText");
            this.LegalInfoText.Name = "LegalInfoText";
            // 
            // HDiv1
            // 
            this.HDiv1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.HDiv1, "HDiv1");
            this.HDiv1.Name = "HDiv1";
            // 
            // HDiv2
            // 
            this.HDiv2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.HDiv2, "HDiv2");
            this.HDiv2.Name = "HDiv2";
            // 
            // versionText
            // 
            resources.ApplyResources(this.versionText, "versionText");
            this.versionText.Name = "versionText";
            this.versionText.Click += new System.EventHandler(this.versionText_Click);
            // 
            // HDiv3
            // 
            this.HDiv3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.HDiv3, "HDiv3");
            this.HDiv3.Name = "HDiv3";
            // 
            // HDiv4
            // 
            this.HDiv4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.HDiv4, "HDiv4");
            this.HDiv4.Name = "HDiv4";
            // 
            // AnodeAbout
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HDiv4);
            this.Controls.Add(this.HDiv3);
            this.Controls.Add(this.versionText);
            this.Controls.Add(this.HDiv2);
            this.Controls.Add(this.HDiv1);
            this.Controls.Add(this.LegalInfoText);
            this.Controls.Add(this.LinkGroup);
            this.Controls.Add(this.CopyrightText);
            this.Controls.Add(this.AnodeSummaryText);
            this.Controls.Add(this.AnodeTitleText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnodeAbout";
            this.LinkGroup.ResumeLayout(false);
            this.LinkGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AnodeTitleText;
        private System.Windows.Forms.Label AnodeSummaryText;
        private System.Windows.Forms.Label CopyrightText;
        private System.Windows.Forms.Panel LinkGroup;
        private System.Windows.Forms.LinkLabel GHLink;
        private System.Windows.Forms.Label LegalInfoText;
        private System.Windows.Forms.LinkLabel BugLink;
        private System.Windows.Forms.Label HDiv1;
        private System.Windows.Forms.Label HDiv2;
        private System.Windows.Forms.Label versionText;
        private System.Windows.Forms.Label HDiv3;
        private System.Windows.Forms.Label HDiv4;
        private System.Windows.Forms.LinkLabel LicenseLink;
    }
}