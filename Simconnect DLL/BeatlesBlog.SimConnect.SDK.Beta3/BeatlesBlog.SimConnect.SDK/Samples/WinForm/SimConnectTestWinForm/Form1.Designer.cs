namespace SimConnectTestWinForm
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.lstViewer = new System.Windows.Forms.ListView();
            this.colheaderLatitude = new System.Windows.Forms.ColumnHeader();
            this.colheaderLongitude = new System.Windows.Forms.ColumnHeader();
            this.colheaderAltitude = new System.Windows.Forms.ColumnHeader();
            this.colheaderPitch = new System.Windows.Forms.ColumnHeader();
            this.colheaderBank = new System.Windows.Forms.ColumnHeader();
            this.colheaderHeading = new System.Windows.Forms.ColumnHeader();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuSimConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuConnectLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuConnectCustom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.lstViewer);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(736, 409);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(736, 433);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // lstViewer
            // 
            this.lstViewer.AllowColumnReorder = true;
            this.lstViewer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colheaderLatitude,
            this.colheaderLongitude,
            this.colheaderAltitude,
            this.colheaderPitch,
            this.colheaderBank,
            this.colheaderHeading});
            this.lstViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstViewer.Location = new System.Drawing.Point(0, 0);
            this.lstViewer.Name = "lstViewer";
            this.lstViewer.Size = new System.Drawing.Size(736, 409);
            this.lstViewer.TabIndex = 0;
            this.lstViewer.UseCompatibleStateImageBehavior = false;
            this.lstViewer.View = System.Windows.Forms.View.Details;
            // 
            // colheaderLatitude
            // 
            this.colheaderLatitude.Text = "Latitude";
            this.colheaderLatitude.Width = 120;
            // 
            // colheaderLongitude
            // 
            this.colheaderLongitude.Text = "Longitude";
            this.colheaderLongitude.Width = 120;
            // 
            // colheaderAltitude
            // 
            this.colheaderAltitude.Text = "Altitude";
            this.colheaderAltitude.Width = 120;
            // 
            // colheaderPitch
            // 
            this.colheaderPitch.Text = "Pitch";
            this.colheaderPitch.Width = 120;
            // 
            // colheaderBank
            // 
            this.colheaderBank.Text = "Bank";
            this.colheaderBank.Width = 120;
            // 
            // colheaderHeading
            // 
            this.colheaderHeading.Text = "Heading";
            this.colheaderHeading.Width = 120;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSimConnect});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(736, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuSimConnect
            // 
            this.menuSimConnect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuConnect,
            this.menuDisconnect,
            this.toolStripSeparator1,
            this.menuExit});
            this.menuSimConnect.Name = "menuSimConnect";
            this.menuSimConnect.Size = new System.Drawing.Size(84, 20);
            this.menuSimConnect.Text = "&SimConnect";
            // 
            // menuConnect
            // 
            this.menuConnect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuConnectLocal,
            this.menuConnectCustom});
            this.menuConnect.Name = "menuConnect";
            this.menuConnect.Size = new System.Drawing.Size(152, 22);
            this.menuConnect.Text = "&Connect";
            this.menuConnect.DropDownOpening += new System.EventHandler(this.menuConnect_DropDownOpening);
            // 
            // menuConnectLocal
            // 
            this.menuConnectLocal.Name = "menuConnectLocal";
            this.menuConnectLocal.Size = new System.Drawing.Size(152, 22);
            this.menuConnectLocal.Text = "&Local";
            this.menuConnectLocal.Click += new System.EventHandler(this.menuConnectLocal_Click);
            // 
            // menuConnectCustom
            // 
            this.menuConnectCustom.Name = "menuConnectCustom";
            this.menuConnectCustom.Size = new System.Drawing.Size(152, 22);
            this.menuConnectCustom.Text = "&Custom...";
            this.menuConnectCustom.Click += new System.EventHandler(this.menuConnectCustom_Click);
            // 
            // menuDisconnect
            // 
            this.menuDisconnect.Name = "menuDisconnect";
            this.menuDisconnect.Size = new System.Drawing.Size(152, 22);
            this.menuDisconnect.Text = "&Disconnect";
            this.menuDisconnect.Visible = false;
            this.menuDisconnect.Click += new System.EventHandler(this.menuDisconnect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(152, 22);
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 433);
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "SimConnect Test WinForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ListView lstViewer;
        private System.Windows.Forms.ColumnHeader colheaderLatitude;
        private System.Windows.Forms.ColumnHeader colheaderLongitude;
        private System.Windows.Forms.ColumnHeader colheaderAltitude;
        private System.Windows.Forms.ColumnHeader colheaderPitch;
        private System.Windows.Forms.ColumnHeader colheaderBank;
        private System.Windows.Forms.ColumnHeader colheaderHeading;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuSimConnect;
        private System.Windows.Forms.ToolStripMenuItem menuConnect;
        private System.Windows.Forms.ToolStripMenuItem menuConnectLocal;
        private System.Windows.Forms.ToolStripMenuItem menuConnectCustom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuDisconnect;
    }
}

