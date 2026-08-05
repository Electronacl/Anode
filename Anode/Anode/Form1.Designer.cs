namespace Anode
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hardResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advanceFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.disableThrottlerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleTracelogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceHaltToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.force60hzForPALToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cartridgeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iNESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.archaicINESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iNESToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nES20ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.regionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodetectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nTSCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pALToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.emulationToolStripMenuItem,
            this.debuggingToolStripMenuItem,
            this.anodeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(296, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.ToolTipText = "Select a path to load the ROM from";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.ToolTipText = "Brings up a dialogue where you can select which ROM the emulator will use.\r\nAutom" +
    "atically starts the emulator when a ROM is loaded.";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // emulationToolStripMenuItem
            // 
            this.emulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hardResetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.pauseToolStripMenuItem,
            this.advanceFrameToolStripMenuItem,
            this.toolStripMenuItem2,
            this.disableThrottlerToolStripMenuItem,
            this.toolStripSeparator1,
            this.regionToolStripMenuItem});
            this.emulationToolStripMenuItem.Name = "emulationToolStripMenuItem";
            this.emulationToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.emulationToolStripMenuItem.Text = "Emulation";
            // 
            // hardResetToolStripMenuItem
            // 
            this.hardResetToolStripMenuItem.Enabled = false;
            this.hardResetToolStripMenuItem.Name = "hardResetToolStripMenuItem";
            this.hardResetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.hardResetToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.hardResetToolStripMenuItem.Text = "Hard reset";
            this.hardResetToolStripMenuItem.ToolTipText = "Completely resets RAM and registers.\r\nThis is equivalent to switching real hardwa" +
    "re off, leaving it, and turning it on again.";
            this.hardResetToolStripMenuItem.Click += new System.EventHandler(this.hardResetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(193, 6);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Enabled = false;
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.ToolTipText = "Pause: Stops the current action and allows it to be resumed later.\r\nPlay: Resumes" +
    " from the point that the gameplay was paused at";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // advanceFrameToolStripMenuItem
            // 
            this.advanceFrameToolStripMenuItem.Enabled = false;
            this.advanceFrameToolStripMenuItem.Name = "advanceFrameToolStripMenuItem";
            this.advanceFrameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.advanceFrameToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.advanceFrameToolStripMenuItem.Text = "Advance frame";
            this.advanceFrameToolStripMenuItem.ToolTipText = "When paused, this allows for a single frame to be processed  and rendered.";
            this.advanceFrameToolStripMenuItem.Click += new System.EventHandler(this.advanceFrameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(193, 6);
            // 
            // disableThrottlerToolStripMenuItem
            // 
            this.disableThrottlerToolStripMenuItem.Checked = true;
            this.disableThrottlerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.disableThrottlerToolStripMenuItem.Name = "disableThrottlerToolStripMenuItem";
            this.disableThrottlerToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.disableThrottlerToolStripMenuItem.Text = "Throttler enabled";
            this.disableThrottlerToolStripMenuItem.ToolTipText = resources.GetString("disableThrottlerToolStripMenuItem.ToolTipText");
            this.disableThrottlerToolStripMenuItem.Click += new System.EventHandler(this.disableThrottlerToolStripMenuItem_Click);
            // 
            // debuggingToolStripMenuItem
            // 
            this.debuggingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleTracelogToolStripMenuItem,
            this.forceHaltToolStripMenuItem});
            this.debuggingToolStripMenuItem.Name = "debuggingToolStripMenuItem";
            this.debuggingToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debuggingToolStripMenuItem.Text = "Debug";
            // 
            // toggleTracelogToolStripMenuItem
            // 
            this.toggleTracelogToolStripMenuItem.Name = "toggleTracelogToolStripMenuItem";
            this.toggleTracelogToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.toggleTracelogToolStripMenuItem.Text = "Tracelog disabled";
            this.toggleTracelogToolStripMenuItem.ToolTipText = "Writes a log of every instruction ran by the CPU onto a file.\r\nUsing this will qu" +
    "ickly consume storage and will slow down the emulator.";
            this.toggleTracelogToolStripMenuItem.Click += new System.EventHandler(this.toggleTracelogToolStripMenuItem_Click);
            // 
            // forceHaltToolStripMenuItem
            // 
            this.forceHaltToolStripMenuItem.Enabled = false;
            this.forceHaltToolStripMenuItem.Name = "forceHaltToolStripMenuItem";
            this.forceHaltToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.forceHaltToolStripMenuItem.Text = "Force Halt";
            this.forceHaltToolStripMenuItem.ToolTipText = resources.GetString("forceHaltToolStripMenuItem.ToolTipText");
            this.forceHaltToolStripMenuItem.Click += new System.EventHandler(this.forceHaltToolStripMenuItem_Click);
            // 
            // anodeToolStripMenuItem
            // 
            this.anodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.anodeToolStripMenuItem.Name = "anodeToolStripMenuItem";
            this.anodeToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.anodeToolStripMenuItem.Text = "Anode";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.videoToolStripMenuItem,
            this.cartridgeToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // videoToolStripMenuItem
            // 
            this.videoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.force60hzForPALToolStripMenuItem});
            this.videoToolStripMenuItem.Name = "videoToolStripMenuItem";
            this.videoToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.videoToolStripMenuItem.Text = "Video";
            // 
            // force60hzForPALToolStripMenuItem
            // 
            this.force60hzForPALToolStripMenuItem.Name = "force60hzForPALToolStripMenuItem";
            this.force60hzForPALToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.force60hzForPALToolStripMenuItem.Text = "Force 60hz for PAL";
            this.force60hzForPALToolStripMenuItem.ToolTipText = resources.GetString("force60hzForPALToolStripMenuItem.ToolTipText");
            this.force60hzForPALToolStripMenuItem.Click += new System.EventHandler(this.force60hzForPALToolStripMenuItem_Click);
            // 
            // cartridgeToolStripMenuItem
            // 
            this.cartridgeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iNESToolStripMenuItem,
            this.archaicINESToolStripMenuItem,
            this.iNESToolStripMenuItem1,
            this.nES20ToolStripMenuItem});
            this.cartridgeToolStripMenuItem.Name = "cartridgeToolStripMenuItem";
            this.cartridgeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cartridgeToolStripMenuItem.Text = "iNES";
            // 
            // iNESToolStripMenuItem
            // 
            this.iNESToolStripMenuItem.Checked = true;
            this.iNESToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.iNESToolStripMenuItem.Name = "iNESToolStripMenuItem";
            this.iNESToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.iNESToolStripMenuItem.Text = "Auto-detect";
            this.iNESToolStripMenuItem.ToolTipText = "Automatically determine the iNES version from the header.\r\nThis might not be able" +
    " to determine whether Archaic iNES is used, but can determine whether to use iNE" +
    "S or NES 2.0 for most carts.\r\n";
            this.iNESToolStripMenuItem.Click += new System.EventHandler(this.iNESToolStripMenuItem_Click);
            // 
            // archaicINESToolStripMenuItem
            // 
            this.archaicINESToolStripMenuItem.Name = "archaicINESToolStripMenuItem";
            this.archaicINESToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.archaicINESToolStripMenuItem.Text = "Archaic iNES";
            this.archaicINESToolStripMenuItem.ToolTipText = "Earliest version of the iNES header, which doesn\'t use bytes 7-15.\r\nUsed in early" +
    " versions of iNES and NESticle.";
            this.archaicINESToolStripMenuItem.Click += new System.EventHandler(this.archaicINESToolStripMenuItem_Click);
            // 
            // iNESToolStripMenuItem1
            // 
            this.iNESToolStripMenuItem1.Name = "iNESToolStripMenuItem1";
            this.iNESToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.iNESToolStripMenuItem1.Text = "iNES/iNES 0.7";
            this.iNESToolStripMenuItem1.ToolTipText = "Newer iNES header with more features, used between about 2000-2010.\r\nCompatible w" +
    "ith both iNES 0.7 and iNES, but not all carts support the extended features of t" +
    "he newer version.";
            this.iNESToolStripMenuItem1.Click += new System.EventHandler(this.iNESToolStripMenuItem1_Click);
            // 
            // nES20ToolStripMenuItem
            // 
            this.nES20ToolStripMenuItem.Name = "nES20ToolStripMenuItem";
            this.nES20ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.nES20ToolStripMenuItem.Text = "NES 2.0";
            this.nES20ToolStripMenuItem.ToolTipText = "Modern version of the iNES header with extended functionality. Used since around " +
    "2010.";
            this.nES20ToolStripMenuItem.Click += new System.EventHandler(this.nES20ToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(9, 22);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(279, 281);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // regionToolStripMenuItem
            // 
            this.regionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autodetectToolStripMenuItem,
            this.nTSCToolStripMenuItem,
            this.pALToolStripMenuItem});
            this.regionToolStripMenuItem.Name = "regionToolStripMenuItem";
            this.regionToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.regionToolStripMenuItem.Text = "Region";
            // 
            // autodetectToolStripMenuItem
            // 
            this.autodetectToolStripMenuItem.Checked = true;
            this.autodetectToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autodetectToolStripMenuItem.Name = "autodetectToolStripMenuItem";
            this.autodetectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.autodetectToolStripMenuItem.Text = "Auto-detect";
            this.autodetectToolStripMenuItem.ToolTipText = "Automatically determine the region from the iNES header. Uses NTSC if the iNES ve" +
    "rsion is archaic.";
            this.autodetectToolStripMenuItem.Click += new System.EventHandler(this.autodetectToolStripMenuItem_Click);
            // 
            // nTSCToolStripMenuItem
            // 
            this.nTSCToolStripMenuItem.Name = "nTSCToolStripMenuItem";
            this.nTSCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.nTSCToolStripMenuItem.Text = "NTSC";
            this.nTSCToolStripMenuItem.ToolTipText = "Region used for the standard NES and Family Computer consoles.\r\nRuns at 60Hz.";
            this.nTSCToolStripMenuItem.Click += new System.EventHandler(this.nTSCToolStripMenuItem_Click);
            // 
            // pALToolStripMenuItem
            // 
            this.pALToolStripMenuItem.Name = "pALToolStripMenuItem";
            this.pALToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pALToolStripMenuItem.Text = "PAL";
            this.pALToolStripMenuItem.ToolTipText = "Region used for the NES Edition and Mattel Edition consoles.\r\nRuns at 50Hz.";
            this.pALToolStripMenuItem.Click += new System.EventHandler(this.pALToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(193, 6);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 310);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(312, 349);
            this.Name = "Form1";
            this.Text = "Anode";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem emulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hardResetToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem debuggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleTracelogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceHaltToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advanceFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem anodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableThrottlerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem force60hzForPALToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cartridgeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iNESToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem archaicINESToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iNESToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem nES20ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem regionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodetectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nTSCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pALToolStripMenuItem;
    }
}

