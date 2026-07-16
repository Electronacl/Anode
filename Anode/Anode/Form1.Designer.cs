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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hardResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advanceFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleTracelogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceHaltToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.disableThrottlerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savestatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableRewindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rewindTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.rewindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.openToolStripMenuItem.ToolTipText = "Brings up dialogue for which ROM to use";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // emulationToolStripMenuItem
            // 
            this.emulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hardResetToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.advanceFrameToolStripMenuItem,
            this.rewindToolStripMenuItem,
            this.disableThrottlerToolStripMenuItem});
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
            this.hardResetToolStripMenuItem.ToolTipText = "Completely resets RAM and registers, equivalent to a power cycle";
            this.hardResetToolStripMenuItem.Click += new System.EventHandler(this.hardResetToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Enabled = false;
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.ToolTipText = "Stops the current action and allows it to be resumed later.";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // advanceFrameToolStripMenuItem
            // 
            this.advanceFrameToolStripMenuItem.Enabled = false;
            this.advanceFrameToolStripMenuItem.Name = "advanceFrameToolStripMenuItem";
            this.advanceFrameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.advanceFrameToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.advanceFrameToolStripMenuItem.Text = "Advance frame";
            this.advanceFrameToolStripMenuItem.ToolTipText = "Moves onto the next frame when paused";
            this.advanceFrameToolStripMenuItem.Click += new System.EventHandler(this.advanceFrameToolStripMenuItem_Click);
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
            this.toggleTracelogToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.toggleTracelogToolStripMenuItem.Text = "Enable Tracelog";
            this.toggleTracelogToolStripMenuItem.ToolTipText = "Provides a log of every instruction run by the CPU.";
            this.toggleTracelogToolStripMenuItem.Click += new System.EventHandler(this.toggleTracelogToolStripMenuItem_Click);
            // 
            // forceHaltToolStripMenuItem
            // 
            this.forceHaltToolStripMenuItem.Enabled = false;
            this.forceHaltToolStripMenuItem.Name = "forceHaltToolStripMenuItem";
            this.forceHaltToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.forceHaltToolStripMenuItem.Text = "Force Halt";
            this.forceHaltToolStripMenuItem.ToolTipText = "Sets the CPU\'s state to halted, crashing it.";
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            // disableThrottlerToolStripMenuItem
            // 
            this.disableThrottlerToolStripMenuItem.Name = "disableThrottlerToolStripMenuItem";
            this.disableThrottlerToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.disableThrottlerToolStripMenuItem.Text = "Disable throttler";
            this.disableThrottlerToolStripMenuItem.Click += new System.EventHandler(this.disableThrottlerToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.savestatesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // savestatesToolStripMenuItem
            // 
            this.savestatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disableRewindToolStripMenuItem,
            this.rewindTimeToolStripMenuItem});
            this.savestatesToolStripMenuItem.Name = "savestatesToolStripMenuItem";
            this.savestatesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.savestatesToolStripMenuItem.Text = "Savestates";
            // 
            // disableRewindToolStripMenuItem
            // 
            this.disableRewindToolStripMenuItem.Name = "disableRewindToolStripMenuItem";
            this.disableRewindToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.disableRewindToolStripMenuItem.Text = "Disable rewind";
            this.disableRewindToolStripMenuItem.Click += new System.EventHandler(this.disableRewindToolStripMenuItem_Click);
            // 
            // rewindTimeToolStripMenuItem
            // 
            this.rewindTimeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.secondsToolStripMenuItem,
            this.secondsToolStripMenuItem1,
            this.secondsToolStripMenuItem2,
            this.secondsToolStripMenuItem3,
            this.secondsToolStripMenuItem4});
            this.rewindTimeToolStripMenuItem.Name = "rewindTimeToolStripMenuItem";
            this.rewindTimeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.rewindTimeToolStripMenuItem.Text = "Rewind frequency";
            // 
            // secondsToolStripMenuItem
            // 
            this.secondsToolStripMenuItem.Name = "secondsToolStripMenuItem";
            this.secondsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.secondsToolStripMenuItem.Text = "2 seconds";
            this.secondsToolStripMenuItem.Click += new System.EventHandler(this.secondsToolStripMenuItem_Click);
            // 
            // secondsToolStripMenuItem1
            // 
            this.secondsToolStripMenuItem1.Name = "secondsToolStripMenuItem1";
            this.secondsToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.secondsToolStripMenuItem1.Text = "5 seconds";
            this.secondsToolStripMenuItem1.Click += new System.EventHandler(this.secondsToolStripMenuItem1_Click);
            // 
            // secondsToolStripMenuItem2
            // 
            this.secondsToolStripMenuItem2.Name = "secondsToolStripMenuItem2";
            this.secondsToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.secondsToolStripMenuItem2.Text = "10 seconds";
            this.secondsToolStripMenuItem2.Click += new System.EventHandler(this.secondsToolStripMenuItem2_Click);
            // 
            // secondsToolStripMenuItem3
            // 
            this.secondsToolStripMenuItem3.Name = "secondsToolStripMenuItem3";
            this.secondsToolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            this.secondsToolStripMenuItem3.Text = "15 seconds";
            this.secondsToolStripMenuItem3.Click += new System.EventHandler(this.secondsToolStripMenuItem3_Click);
            // 
            // secondsToolStripMenuItem4
            // 
            this.secondsToolStripMenuItem4.Name = "secondsToolStripMenuItem4";
            this.secondsToolStripMenuItem4.Size = new System.Drawing.Size(180, 22);
            this.secondsToolStripMenuItem4.Text = "30 seconds";
            this.secondsToolStripMenuItem4.Click += new System.EventHandler(this.secondsToolStripMenuItem4_Click);
            // 
            // rewindToolStripMenuItem
            // 
            this.rewindToolStripMenuItem.Name = "rewindToolStripMenuItem";
            this.rewindToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.rewindToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.rewindToolStripMenuItem.Text = "Rewind";
            this.rewindToolStripMenuItem.Click += new System.EventHandler(this.rewindToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 310);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
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
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savestatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableRewindToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rewindTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem rewindToolStripMenuItem;
    }
}

