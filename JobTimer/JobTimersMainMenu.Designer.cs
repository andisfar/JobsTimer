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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobTimerForm));
            this.jobTimersMainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timersDataGridView = new System.Windows.Forms.DataGridView();
            this.timerNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.previousTimeSpanDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.keyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SavedTimersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.SavedTimersDataSet = new System.Data.DataSet();
            this.Timers = new System.Data.DataTable();
            this.TimerName = new System.Data.DataColumn();
            this.PreviousTimeSpan = new System.Data.DataColumn();
            this.key = new System.Data.DataColumn();
            this.timersToolStrinp = new System.Windows.Forms.ToolStrip();
            this.DeleteSelectedTimer = new System.Windows.Forms.ToolStripButton();
            this.jobTimersIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.jobTimersContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.activeTimersMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newTimerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.RestoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QuitContextMenuSeparatorItem = new System.Windows.Forms.ToolStripSeparator();
            this.QuitContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SavedTimersBindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.jobTimersMainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SavedTimersBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SavedTimersDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Timers)).BeginInit();
            this.timersToolStrinp.SuspendLayout();
            this.jobTimersContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SavedTimersBindingNavigator)).BeginInit();
            this.SavedTimersBindingNavigator.SuspendLayout();
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
            this.jobTimersMainMenu.Size = new System.Drawing.Size(428, 34);
            this.jobTimersMainMenu.TabIndex = 1;
            this.jobTimersMainMenu.Text = "Job Timers Main Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Image = global::JobTimer.Properties.Resources.file;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(65, 32);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(94, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Image = global::JobTimer.Properties.Resources.quit;
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItem_Click);
            // 
            // timersToolStripMenuItem
            // 
            this.timersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTimerToolStripMenuItem});
            this.timersToolStripMenuItem.Image = global::JobTimer.Properties.Resources.appLogo;
            this.timersToolStripMenuItem.Name = "timersToolStripMenuItem";
            this.timersToolStripMenuItem.Size = new System.Drawing.Size(83, 32);
            this.timersToolStripMenuItem.Text = "Timers";
            // 
            // newTimerToolStripMenuItem
            // 
            this.newTimerToolStripMenuItem.Image = global::JobTimer.Properties.Resources._new;
            this.newTimerToolStripMenuItem.Name = "newTimerToolStripMenuItem";
            this.newTimerToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.newTimerToolStripMenuItem.Text = "New &Timer";
            this.newTimerToolStripMenuItem.Click += new System.EventHandler(this.NewTimerToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.timersDataGridView);
            this.panel1.Controls.Add(this.timersToolStrinp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 34);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(428, 216);
            this.panel1.TabIndex = 3;
            // 
            // timersDataGridView
            // 
            this.timersDataGridView.AllowUserToAddRows = false;
            this.timersDataGridView.AllowUserToDeleteRows = false;
            this.timersDataGridView.AutoGenerateColumns = false;
            this.timersDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.timersDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.timersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.timersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timerNameDataGridViewTextBoxColumn,
            this.previousTimeSpanDataGridViewTextBoxColumn,
            this.keyDataGridViewTextBoxColumn});
            this.timersDataGridView.DataSource = this.SavedTimersBindingSource;
            this.timersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timersDataGridView.Location = new System.Drawing.Point(0, 35);
            this.timersDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.timersDataGridView.Name = "timersDataGridView";
            this.timersDataGridView.ReadOnly = true;
            this.timersDataGridView.RowTemplate.Height = 31;
            this.timersDataGridView.Size = new System.Drawing.Size(428, 181);
            this.timersDataGridView.TabIndex = 2;
            this.timersDataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.TimersDataGridView_RowHeaderMouseClick);
            this.timersDataGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TimersDataGridView_MouseClick);
            // 
            // timerNameDataGridViewTextBoxColumn
            // 
            this.timerNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.timerNameDataGridViewTextBoxColumn.DataPropertyName = "TimerName";
            this.timerNameDataGridViewTextBoxColumn.DividerWidth = 8;
            this.timerNameDataGridViewTextBoxColumn.HeaderText = "Timer Name";
            this.timerNameDataGridViewTextBoxColumn.Name = "timerNameDataGridViewTextBoxColumn";
            this.timerNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // previousTimeSpanDataGridViewTextBoxColumn
            // 
            this.previousTimeSpanDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.previousTimeSpanDataGridViewTextBoxColumn.DataPropertyName = "PreviousTimeSpan";
            this.previousTimeSpanDataGridViewTextBoxColumn.DividerWidth = 8;
            this.previousTimeSpanDataGridViewTextBoxColumn.HeaderText = "PreviousTimeSpan";
            this.previousTimeSpanDataGridViewTextBoxColumn.Name = "previousTimeSpanDataGridViewTextBoxColumn";
            this.previousTimeSpanDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // keyDataGridViewTextBoxColumn
            // 
            this.keyDataGridViewTextBoxColumn.DataPropertyName = "key";
            this.keyDataGridViewTextBoxColumn.HeaderText = "key";
            this.keyDataGridViewTextBoxColumn.Name = "keyDataGridViewTextBoxColumn";
            this.keyDataGridViewTextBoxColumn.ReadOnly = true;
            this.keyDataGridViewTextBoxColumn.Visible = false;
            // 
            // SavedTimersBindingSource
            // 
            this.SavedTimersBindingSource.DataMember = "Timers";
            this.SavedTimersBindingSource.DataSource = this.SavedTimersDataSet;
            // 
            // SavedTimersDataSet
            // 
            this.SavedTimersDataSet.CaseSensitive = true;
            this.SavedTimersDataSet.DataSetName = "SavedTimersDataSet";
            this.SavedTimersDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.Timers});
            // 
            // Timers
            // 
            this.Timers.CaseSensitive = true;
            this.Timers.Columns.AddRange(new System.Data.DataColumn[] {
            this.TimerName,
            this.PreviousTimeSpan,
            this.key});
            this.Timers.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "key"}, true),
            new System.Data.UniqueConstraint("Constraint3", new string[] {
                        "TimerName"}, false)});
            this.Timers.PrimaryKey = new System.Data.DataColumn[] {
        this.key};
            this.Timers.TableName = "Timers";
            // 
            // TimerName
            // 
            this.TimerName.Caption = "Timer Name";
            this.TimerName.ColumnName = "TimerName";
            this.TimerName.DefaultValue = "Timer";
            // 
            // PreviousTimeSpan
            // 
            this.PreviousTimeSpan.Caption = "Saved Elapsed Timer";
            this.PreviousTimeSpan.ColumnName = "PreviousTimeSpan";
            this.PreviousTimeSpan.DefaultValue = "00:00:00";
            // 
            // key
            // 
            this.key.AllowDBNull = false;
            this.key.AutoIncrement = true;
            this.key.ColumnMapping = System.Data.MappingType.Hidden;
            this.key.ColumnName = "key";
            this.key.DataType = typeof(int);
            this.key.ReadOnly = true;
            // 
            // timersToolStrinp
            // 
            this.timersToolStrinp.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.timersToolStrinp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DeleteSelectedTimer});
            this.timersToolStrinp.Location = new System.Drawing.Point(0, 0);
            this.timersToolStrinp.Name = "timersToolStrinp";
            this.timersToolStrinp.Size = new System.Drawing.Size(428, 35);
            this.timersToolStrinp.TabIndex = 1;
            this.timersToolStrinp.Text = "timersToolStrinp";
            // 
            // DeleteSelectedTimer
            // 
            this.DeleteSelectedTimer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DeleteSelectedTimer.Image = ((System.Drawing.Image)(resources.GetObject("DeleteSelectedTimer.Image")));
            this.DeleteSelectedTimer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DeleteSelectedTimer.Name = "DeleteSelectedTimer";
            this.DeleteSelectedTimer.Size = new System.Drawing.Size(32, 32);
            this.DeleteSelectedTimer.Text = "Delete Selected Timer";
            this.DeleteSelectedTimer.Click += new System.EventHandler(this.TimersGridViewDeleteItem_Click);
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
            this.newTimerToolStripMenuItem1,
            this.toolStripSeparator2,
            this.RestoreToolStripMenuItem,
            this.QuitContextMenuSeparatorItem,
            this.QuitContextMenuItem});
            this.jobTimersContextMenu.Name = "fcmContextMenu";
            this.jobTimersContextMenu.Size = new System.Drawing.Size(154, 152);
            this.jobTimersContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.JobTimersContextMenu_Opening);
            // 
            // activeTimersMenu
            // 
            this.activeTimersMenu.Image = ((System.Drawing.Image)(resources.GetObject("activeTimersMenu.Image")));
            this.activeTimersMenu.Name = "activeTimersMenu";
            this.activeTimersMenu.Size = new System.Drawing.Size(153, 34);
            this.activeTimersMenu.Text = "Timers";
            this.activeTimersMenu.DropDownOpening += new System.EventHandler(this.ActiveTimersMenu_DropDownOpening);
            // 
            // newTimerToolStripMenuItem1
            // 
            this.newTimerToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("newTimerToolStripMenuItem1.Image")));
            this.newTimerToolStripMenuItem1.Name = "newTimerToolStripMenuItem1";
            this.newTimerToolStripMenuItem1.Size = new System.Drawing.Size(153, 34);
            this.newTimerToolStripMenuItem1.Text = "New Timer...";
            this.newTimerToolStripMenuItem1.Click += new System.EventHandler(this.NewTimerToolStripMenuItem1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(150, 6);
            // 
            // RestoreToolStripMenuItem
            // 
            this.RestoreToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("RestoreToolStripMenuItem.Image")));
            this.RestoreToolStripMenuItem.Name = "RestoreToolStripMenuItem";
            this.RestoreToolStripMenuItem.Size = new System.Drawing.Size(153, 34);
            this.RestoreToolStripMenuItem.Text = "Restore";
            this.RestoreToolStripMenuItem.Click += new System.EventHandler(this.RestoreToolStripMenuItem_Click);
            // 
            // QuitContextMenuSeparatorItem
            // 
            this.QuitContextMenuSeparatorItem.Name = "QuitContextMenuSeparatorItem";
            this.QuitContextMenuSeparatorItem.Size = new System.Drawing.Size(150, 6);
            // 
            // QuitContextMenuItem
            // 
            this.QuitContextMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("QuitContextMenuItem.Image")));
            this.QuitContextMenuItem.Name = "QuitContextMenuItem";
            this.QuitContextMenuItem.Size = new System.Drawing.Size(153, 34);
            this.QuitContextMenuItem.Text = "Quit";
            this.QuitContextMenuItem.Click += new System.EventHandler(this.QuitContextMenuItem_Click);
            // 
            // SavedTimersBindingNavigator
            // 
            this.SavedTimersBindingNavigator.AddNewItem = null;
            this.SavedTimersBindingNavigator.BindingSource = this.SavedTimersBindingSource;
            this.SavedTimersBindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.SavedTimersBindingNavigator.DeleteItem = null;
            this.SavedTimersBindingNavigator.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.SavedTimersBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem});
            this.SavedTimersBindingNavigator.Location = new System.Drawing.Point(0, 34);
            this.SavedTimersBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.SavedTimersBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.SavedTimersBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.SavedTimersBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.SavedTimersBindingNavigator.Name = "SavedTimersBindingNavigator";
            this.SavedTimersBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.SavedTimersBindingNavigator.Size = new System.Drawing.Size(428, 35);
            this.SavedTimersBindingNavigator.TabIndex = 4;
            this.SavedTimersBindingNavigator.Text = "Saved Timers";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 32);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            this.bindingNavigatorCountItem.Visible = false;
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(32, 32);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            this.bindingNavigatorMoveFirstItem.Visible = false;
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(32, 32);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            this.bindingNavigatorMovePreviousItem.Visible = false;
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 35);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(29, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            this.bindingNavigatorPositionItem.Visible = false;
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(32, 32);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            this.bindingNavigatorMoveNextItem.Visible = false;
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(32, 32);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            this.bindingNavigatorMoveLastItem.Visible = false;
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(32, 32);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            this.bindingNavigatorAddNewItem.Click += new System.EventHandler(this.BindingNavigatorAddNewItem_Click);
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(32, 32);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            this.bindingNavigatorDeleteItem.Click += new System.EventHandler(this.BindingNavigatorDeleteItem_Click);
            // 
            // JobTimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 250);
            this.Controls.Add(this.SavedTimersBindingNavigator);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.jobTimersMainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(0, 29);
            this.MainMenuStrip = this.jobTimersMainMenu;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(444, 289);
            this.MinimumSize = new System.Drawing.Size(444, 289);
            this.Name = "JobTimerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Job Timer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JobTimerForm_FormClosing);
            this.Load += new System.EventHandler(this.JobTimerForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.JobTimerForm_KeyUp);
            this.jobTimersMainMenu.ResumeLayout(false);
            this.jobTimersMainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SavedTimersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SavedTimersDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Timers)).EndInit();
            this.timersToolStrinp.ResumeLayout(false);
            this.timersToolStrinp.PerformLayout();
            this.jobTimersContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SavedTimersBindingNavigator)).EndInit();
            this.SavedTimersBindingNavigator.ResumeLayout(false);
            this.SavedTimersBindingNavigator.PerformLayout();
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
        private System.Windows.Forms.ToolStrip timersToolStrinp;
        private System.Windows.Forms.ToolStripButton DeleteSelectedTimer;
        internal System.Windows.Forms.NotifyIcon jobTimersIcon;
        internal System.Windows.Forms.ContextMenuStrip jobTimersContextMenu;
        internal System.Windows.Forms.ToolStripMenuItem RestoreToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator QuitContextMenuSeparatorItem;
        internal System.Windows.Forms.ToolStripMenuItem QuitContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activeTimersMenu;
        private System.Windows.Forms.ToolStripMenuItem newTimerToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.DataGridView timersDataGridView;
        private System.Data.DataSet SavedTimersDataSet;
        private System.Data.DataTable Timers;
        private System.Data.DataColumn TimerName;
        private System.Data.DataColumn PreviousTimeSpan;
        private System.Windows.Forms.BindingSource SavedTimersBindingSource;
        private System.Windows.Forms.BindingNavigator SavedTimersBindingNavigator;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Data.DataColumn key;
        private System.Windows.Forms.DataGridViewTextBoxColumn timerNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn previousTimeSpanDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyDataGridViewTextBoxColumn;
    }
}

