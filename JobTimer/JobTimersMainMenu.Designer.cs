namespace JobTimer
{
    partial class JobTimerForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobTimerForm));
            this.jobTimersMainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timersDataGridView = new System.Windows.Forms.DataGridView();
            this.previousTimeSpan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WindowTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timersToolStrinp = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.jobTimersIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.jobTimersContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.activeTimersMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.RestoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QuitContextMenuSeparatorItem = new System.Windows.Forms.ToolStripSeparator();
            this.QuitContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jobTimersMainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timersDataGridView)).BeginInit();
            this.dgvContextMenu.SuspendLayout();
            this.timersToolStrinp.SuspendLayout();
            this.jobTimersContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // jobTimersMainMenu
            // 
            this.jobTimersMainMenu.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.jobTimersMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.timersToolStripMenuItem});
            this.jobTimersMainMenu.Location = new System.Drawing.Point(0, 0);
            this.jobTimersMainMenu.Name = "jobTimersMainMenu";
            this.jobTimersMainMenu.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.jobTimersMainMenu.Size = new System.Drawing.Size(949, 24);
            this.jobTimersMainMenu.TabIndex = 1;
            this.jobTimersMainMenu.Text = "Job Timers Main Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(94, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItem_Click);
            // 
            // timersToolStripMenuItem
            // 
            this.timersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTimerToolStripMenuItem});
            this.timersToolStripMenuItem.Name = "timersToolStripMenuItem";
            this.timersToolStripMenuItem.Size = new System.Drawing.Size(55, 22);
            this.timersToolStripMenuItem.Text = "Timers";
            // 
            // newTimerToolStripMenuItem
            // 
            this.newTimerToolStripMenuItem.Name = "newTimerToolStripMenuItem";
            this.newTimerToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.newTimerToolStripMenuItem.Text = "New &Timer";
            this.newTimerToolStripMenuItem.Click += new System.EventHandler(this.NewTimerToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.timersDataGridView);
            this.panel1.Controls.Add(this.timersToolStrinp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(502, 232);
            this.panel1.TabIndex = 3;
            // 
            // timersDataGridView
            // 
            this.timersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.timersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.previousTimeSpan,
            this.WindowTitle});
            this.timersDataGridView.ContextMenuStrip = this.dgvContextMenu;
            this.timersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timersDataGridView.Location = new System.Drawing.Point(0, 35);
            this.timersDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.timersDataGridView.Name = "timersDataGridView";
            this.timersDataGridView.RowTemplate.Height = 31;
            this.timersDataGridView.Size = new System.Drawing.Size(502, 197);
            this.timersDataGridView.TabIndex = 0;
            this.timersDataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.TimersDataGridView_RowHeaderMouseClick);
            this.timersDataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimersDataGridView_MouseDown);
            // 
            // previousTimeSpan
            // 
            this.previousTimeSpan.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle1.Format = "00:00:00";
            dataGridViewCellStyle1.NullValue = "00:00:00";
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.previousTimeSpan.DefaultCellStyle = dataGridViewCellStyle1;
            this.previousTimeSpan.DividerWidth = 8;
            this.previousTimeSpan.HeaderText = "Previous TimeSpan";
            this.previousTimeSpan.Name = "previousTimeSpan";
            // 
            // WindowTitle
            // 
            this.WindowTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle2.NullValue = "Timer";
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.WindowTitle.DefaultCellStyle = dataGridViewCellStyle2;
            this.WindowTitle.DividerWidth = 8;
            this.WindowTitle.HeaderText = "Window Title";
            this.WindowTitle.Name = "WindowTitle";
            // 
            // dgvContextMenu
            // 
            this.dgvContextMenu.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.dgvContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetTimerToolStripMenuItem});
            this.dgvContextMenu.Name = "dgvContextMenu";
            this.dgvContextMenu.Size = new System.Drawing.Size(137, 26);
            this.dgvContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.DgvContextMenu_Opening);
            // 
            // resetTimerToolStripMenuItem
            // 
            this.resetTimerToolStripMenuItem.Name = "resetTimerToolStripMenuItem";
            this.resetTimerToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.resetTimerToolStripMenuItem.Tag = "Reset Timer [{0},{1}]?";
            this.resetTimerToolStripMenuItem.Text = "Reset Timer";
            this.resetTimerToolStripMenuItem.Click += new System.EventHandler(this.ResetTimerToolStripMenuItem_Click);
            this.resetTimerToolStripMenuItem.MouseEnter += new System.EventHandler(this.ResetTimerToolStripMenuItem_MouseEnter);
            this.resetTimerToolStripMenuItem.MouseLeave += new System.EventHandler(this.ResetTimerToolStripMenuItem_MouseLeave);
            // 
            // timersToolStrinp
            // 
            this.timersToolStrinp.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.timersToolStrinp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.timersToolStrinp.Location = new System.Drawing.Point(0, 0);
            this.timersToolStrinp.Name = "timersToolStrinp";
            this.timersToolStrinp.Size = new System.Drawing.Size(502, 35);
            this.timersToolStrinp.TabIndex = 1;
            this.timersToolStrinp.Text = "timersToolStrinp";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(32, 32);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.TimersGridViewDeleteItem_Click);
            // 
            // jobTimersIcon
            // 
            this.jobTimersIcon.BalloonTipTitle = "Job Timers";
            this.jobTimersIcon.ContextMenuStrip = this.jobTimersContextMenu;
            this.jobTimersIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("jobTimersIcon.Icon")));
            this.jobTimersIcon.Text = "Job Timers";
            this.jobTimersIcon.Visible = true;
            // 
            // jobTimersContextMenu
            // 
            this.jobTimersContextMenu.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.jobTimersContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activeTimersMenu,
            this.RestoreToolStripMenuItem,
            this.QuitContextMenuSeparatorItem,
            this.QuitContextMenuItem});
            this.jobTimersContextMenu.Name = "fcmContextMenu";
            this.jobTimersContextMenu.Size = new System.Drawing.Size(126, 112);
            this.jobTimersContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.JobTimersContextMenu_Opening);
            // 
            // activeTimersMenu
            // 
            this.activeTimersMenu.Image = ((System.Drawing.Image)(resources.GetObject("activeTimersMenu.Image")));
            this.activeTimersMenu.Name = "activeTimersMenu";
            this.activeTimersMenu.Size = new System.Drawing.Size(125, 34);
            this.activeTimersMenu.Text = "Timers";
            this.activeTimersMenu.DropDownOpening += new System.EventHandler(this.ActiveTimersMenu_DropDownOpening);
            // 
            // RestoreToolStripMenuItem
            // 
            this.RestoreToolStripMenuItem.Name = "RestoreToolStripMenuItem";
            this.RestoreToolStripMenuItem.Size = new System.Drawing.Size(125, 34);
            this.RestoreToolStripMenuItem.Text = "Restore";
            this.RestoreToolStripMenuItem.Click += new System.EventHandler(this.RestoreToolStripMenuItem_Click);
            // 
            // QuitContextMenuSeparatorItem
            // 
            this.QuitContextMenuSeparatorItem.Name = "QuitContextMenuSeparatorItem";
            this.QuitContextMenuSeparatorItem.Size = new System.Drawing.Size(122, 6);
            // 
            // QuitContextMenuItem
            // 
            this.QuitContextMenuItem.Name = "QuitContextMenuItem";
            this.QuitContextMenuItem.Size = new System.Drawing.Size(125, 34);
            this.QuitContextMenuItem.Text = "Quit";
            this.QuitContextMenuItem.Click += new System.EventHandler(this.QuitContextMenuItem_Click);
            // 
            // JobTimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 256);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.jobTimersMainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(0, 29);
            this.MainMenuStrip = this.jobTimersMainMenu;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(965, 295);
            this.Name = "JobTimerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Job Timer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JobTimerForm_FormClosing);
            this.Load += new System.EventHandler(this.JobTimerForm_Load);
            this.MdiChildActivate += new System.EventHandler(this.JobTimerForm_MdiChildActivate);
            this.ResizeEnd += new System.EventHandler(this.JobTimerForm_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.JobTimerForm_SizeChanged);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.JobTimerForm_KeyUp);
            this.jobTimersMainMenu.ResumeLayout(false);
            this.jobTimersMainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timersDataGridView)).EndInit();
            this.dgvContextMenu.ResumeLayout(false);
            this.timersToolStrinp.ResumeLayout(false);
            this.timersToolStrinp.PerformLayout();
            this.jobTimersContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip jobTimersMainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newTimerToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView timersDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn previousTimeSpan;
        private System.Windows.Forms.DataGridViewTextBoxColumn WindowTitle;
        private System.Windows.Forms.ToolStrip timersToolStrinp;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        internal System.Windows.Forms.NotifyIcon jobTimersIcon;
        internal System.Windows.Forms.ContextMenuStrip jobTimersContextMenu;
        internal System.Windows.Forms.ToolStripMenuItem RestoreToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator QuitContextMenuSeparatorItem;
        internal System.Windows.Forms.ToolStripMenuItem QuitContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activeTimersMenu;
        private System.Windows.Forms.ContextMenuStrip dgvContextMenu;
        private System.Windows.Forms.ToolStripMenuItem resetTimerToolStripMenuItem;
    }
}

