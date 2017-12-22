namespace SingleTimerLib
{
    partial class SingleTimerEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleTimerEditorForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.acceptButton = new System.Windows.Forms.Button();
            this.rejectButton = new System.Windows.Forms.Button();
            this.TimerNameTextBox = new System.Windows.Forms.TextBox();
            this.TimerElapsedTimeTextBox = new System.Windows.Forms.TextBox();
            this.resetTimerbutton = new System.Windows.Forms.Button();
            this.RunTimerCheckBox = new System.Windows.Forms.CheckBox();
            this.stateImageList = new System.Windows.Forms.ImageList(this.components);
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(350, 120);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanel1.Controls.Add(this.acceptButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.rejectButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.TimerNameTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TimerElapsedTimeTextBox, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.resetTimerbutton, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.RunTimerCheckBox, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(350, 120);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // acceptButton
            // 
            this.acceptButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.acceptButton.Enabled = false;
            this.acceptButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acceptButton.Location = new System.Drawing.Point(3, 38);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(109, 79);
            this.acceptButton.TabIndex = 2;
            this.acceptButton.Text = "Accept";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.AcceptButton_Click);
            // 
            // rejectButton
            // 
            this.rejectButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.rejectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rejectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.rejectButton.Location = new System.Drawing.Point(118, 38);
            this.rejectButton.Name = "rejectButton";
            this.rejectButton.Size = new System.Drawing.Size(109, 79);
            this.rejectButton.TabIndex = 3;
            this.rejectButton.Text = "Cancel";
            this.rejectButton.UseVisualStyleBackColor = true;
            this.rejectButton.Click += new System.EventHandler(this.RejectButton_Click);
            // 
            // TimerNameTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.TimerNameTextBox, 2);
            this.TimerNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimerNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.TimerNameTextBox.Location = new System.Drawing.Point(3, 3);
            this.TimerNameTextBox.Name = "TimerNameTextBox";
            this.TimerNameTextBox.Size = new System.Drawing.Size(224, 29);
            this.TimerNameTextBox.TabIndex = 0;
            this.TimerNameTextBox.WordWrap = false;
            this.TimerNameTextBox.TextChanged += new System.EventHandler(this.TimerNameTextBox_TextChanged);
            this.TimerNameTextBox.Validated += new System.EventHandler(this.TimerNameTextBox_Validated);
            // 
            // TimerElapsedTimeTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.TimerElapsedTimeTextBox, 2);
            this.TimerElapsedTimeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimerElapsedTimeTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimerElapsedTimeTextBox.Location = new System.Drawing.Point(233, 3);
            this.TimerElapsedTimeTextBox.Name = "TimerElapsedTimeTextBox";
            this.TimerElapsedTimeTextBox.Size = new System.Drawing.Size(114, 29);
            this.TimerElapsedTimeTextBox.TabIndex = 1;
            this.TimerElapsedTimeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TimerElapsedTimeTextBox.WordWrap = false;
            this.TimerElapsedTimeTextBox.TextChanged += new System.EventHandler(this.TimerElapsedTimeTextBox_TextChanged);
            this.TimerElapsedTimeTextBox.Validated += new System.EventHandler(this.TimerElapsedTimeTextBox_Validated);
            // 
            // resetTimerbutton
            // 
            this.resetTimerbutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resetTimerbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.resetTimerbutton.Location = new System.Drawing.Point(269, 38);
            this.resetTimerbutton.Name = "resetTimerbutton";
            this.resetTimerbutton.Size = new System.Drawing.Size(78, 79);
            this.resetTimerbutton.TabIndex = 4;
            this.resetTimerbutton.Text = "Reset Timer";
            this.resetTimerbutton.UseVisualStyleBackColor = true;
            this.resetTimerbutton.Click += new System.EventHandler(this.ResetTimerbutton_Click);
            // 
            // RunTimerCheckBox
            // 
            this.RunTimerCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.RunTimerCheckBox.AutoSize = true;
            this.RunTimerCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RunTimerCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RunTimerCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RunTimerCheckBox.ImageKey = "play";
            this.RunTimerCheckBox.ImageList = this.stateImageList;
            this.RunTimerCheckBox.Location = new System.Drawing.Point(233, 38);
            this.RunTimerCheckBox.Name = "RunTimerCheckBox";
            this.RunTimerCheckBox.Size = new System.Drawing.Size(30, 79);
            this.RunTimerCheckBox.TabIndex = 5;
            this.RunTimerCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RunTimerCheckBox.UseVisualStyleBackColor = true;
            this.RunTimerCheckBox.CheckedChanged += new System.EventHandler(this.RunTimerCheckBox_CheckedChanged);
            // 
            // stateImageList
            // 
            this.stateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("stateImageList.ImageStream")));
            this.stateImageList.TransparentColor = System.Drawing.Color.White;
            this.stateImageList.Images.SetKeyName(0, "play");
            this.stateImageList.Images.SetKeyName(1, "stop");
            // 
            // SingleTimerEditorForm
            // 
            this.AcceptButton = this.acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.rejectButton;
            this.ClientSize = new System.Drawing.Size(350, 120);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(350, 120);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 120);
            this.Name = "SingleTimerEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.SingleTimerEditorForm_Load);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button rejectButton;
        private System.Windows.Forms.TextBox TimerNameTextBox;
        private System.Windows.Forms.TextBox TimerElapsedTimeTextBox;
        private System.Windows.Forms.Button resetTimerbutton;
        private System.Windows.Forms.CheckBox RunTimerCheckBox;
        private System.Windows.Forms.ImageList stateImageList;
    }
}

