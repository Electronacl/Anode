namespace Anode
{
    partial class AnodeMainUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnodeMainUI));
            this.ControlStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hardResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDivider1 = new System.Windows.Forms.ToolStripSeparator();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advanceFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDivider2 = new System.Windows.Forms.ToolStripSeparator();
            this.throttlerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicsDisplayArea = new System.Windows.Forms.PictureBox();
            this.ControlStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphicsDisplayArea)).BeginInit();
            this.SuspendLayout();
            // 
            // ControlStrip
            // 
            this.ControlStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.emulationToolStripMenuItem,
            this.anodeToolStripMenuItem});
            resources.ApplyResources(this.ControlStrip, "ControlStrip");
            this.ControlStrip.Name = "ControlStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // emulationToolStripMenuItem
            // 
            this.emulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hardResetToolStripMenuItem,
            this.toolStripDivider1,
            this.pauseToolStripMenuItem,
            this.advanceFrameToolStripMenuItem,
            this.toolStripDivider2,
            this.throttlerToolStripMenuItem});
            this.emulationToolStripMenuItem.Name = "emulationToolStripMenuItem";
            resources.ApplyResources(this.emulationToolStripMenuItem, "emulationToolStripMenuItem");
            // 
            // hardResetToolStripMenuItem
            // 
            resources.ApplyResources(this.hardResetToolStripMenuItem, "hardResetToolStripMenuItem");
            this.hardResetToolStripMenuItem.Name = "hardResetToolStripMenuItem";
            this.hardResetToolStripMenuItem.Click += new System.EventHandler(this.hardResetToolStripMenuItem_Click);
            // 
            // toolStripDivider1
            // 
            this.toolStripDivider1.Name = "toolStripDivider1";
            resources.ApplyResources(this.toolStripDivider1, "toolStripDivider1");
            // 
            // pauseToolStripMenuItem
            // 
            resources.ApplyResources(this.pauseToolStripMenuItem, "pauseToolStripMenuItem");
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // advanceFrameToolStripMenuItem
            // 
            resources.ApplyResources(this.advanceFrameToolStripMenuItem, "advanceFrameToolStripMenuItem");
            this.advanceFrameToolStripMenuItem.Name = "advanceFrameToolStripMenuItem";
            this.advanceFrameToolStripMenuItem.Click += new System.EventHandler(this.advanceFrameToolStripMenuItem_Click);
            // 
            // toolStripDivider2
            // 
            this.toolStripDivider2.Name = "toolStripDivider2";
            resources.ApplyResources(this.toolStripDivider2, "toolStripDivider2");
            // 
            // throttlerToolStripMenuItem
            // 
            this.throttlerToolStripMenuItem.Checked = true;
            this.throttlerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.throttlerToolStripMenuItem.Name = "throttlerToolStripMenuItem";
            resources.ApplyResources(this.throttlerToolStripMenuItem, "throttlerToolStripMenuItem");
            this.throttlerToolStripMenuItem.Click += new System.EventHandler(this.disableThrottlerToolStripMenuItem_Click);
            // 
            // anodeToolStripMenuItem
            // 
            this.anodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.anodeToolStripMenuItem.Name = "anodeToolStripMenuItem";
            resources.ApplyResources(this.anodeToolStripMenuItem, "anodeToolStripMenuItem");
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // graphicsDisplayArea
            // 
            resources.ApplyResources(this.graphicsDisplayArea, "graphicsDisplayArea");
            this.graphicsDisplayArea.Name = "graphicsDisplayArea";
            this.graphicsDisplayArea.TabStop = false;
            // 
            // AnodeMainUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graphicsDisplayArea);
            this.Controls.Add(this.ControlStrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this.ControlStrip;
            this.Name = "AnodeMainUI";
            this.ControlStrip.ResumeLayout(false);
            this.ControlStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphicsDisplayArea)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip ControlStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem emulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hardResetToolStripMenuItem;
        private System.Windows.Forms.PictureBox graphicsDisplayArea;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advanceFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem anodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem throttlerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripDivider1;
        private System.Windows.Forms.ToolStripSeparator toolStripDivider2;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    }
}

