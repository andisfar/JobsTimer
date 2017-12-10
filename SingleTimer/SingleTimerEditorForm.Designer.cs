namespace SingleTimer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleTimerEditorForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TimerNameLabel = new System.Windows.Forms.TextBox();
            this.TimerElapsedTimeLabel = new System.Windows.Forms.TextBox();
            this.acceptButton = new System.Windows.Forms.Button();
            this.rejectButton = new System.Windows.Forms.Button();
            this.resetTimerbutton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(330, 77);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel1.Controls.Add(this.acceptButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.rejectButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.TimerNameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TimerElapsedTimeLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.resetTimerbutton, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 77);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // TimerNameLabel
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.TimerNameLabel, 2);
            this.TimerNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimerNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.TimerNameLabel.Location = new System.Drawing.Point(3, 3);
            this.TimerNameLabel.Name = "TimerNameLabel";
            this.TimerNameLabel.Size = new System.Drawing.Size(204, 29);
            this.TimerNameLabel.TabIndex = 0;
            this.TimerNameLabel.WordWrap = false;
            // 
            // TimerElapsedTimeLabel
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.TimerElapsedTimeLabel, 2);
            this.TimerElapsedTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimerElapsedTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimerElapsedTimeLabel.Location = new System.Drawing.Point(213, 3);
            this.TimerElapsedTimeLabel.Name = "TimerElapsedTimeLabel";
            this.TimerElapsedTimeLabel.Size = new System.Drawing.Size(114, 29);
            this.TimerElapsedTimeLabel.TabIndex = 1;
            this.TimerElapsedTimeLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TimerElapsedTimeLabel.WordWrap = false;
            // 
            // acceptButton
            // 
            this.acceptButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.acceptButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acceptButton.Location = new System.Drawing.Point(3, 38);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(99, 36);
            this.acceptButton.TabIndex = 2;
            this.acceptButton.Text = "Accept";
            this.acceptButton.UseVisualStyleBackColor = true;
            // 
            // rejectButton
            // 
            this.rejectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rejectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.rejectButton.Location = new System.Drawing.Point(108, 38);
            this.rejectButton.Name = "rejectButton";
            this.rejectButton.Size = new System.Drawing.Size(99, 36);
            this.rejectButton.TabIndex = 3;
            this.rejectButton.Text = "Cancel";
            this.rejectButton.UseVisualStyleBackColor = true;
            // 
            // resetTimerbutton
            // 
            this.resetTimerbutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resetTimerbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.resetTimerbutton.Location = new System.Drawing.Point(249, 38);
            this.resetTimerbutton.Name = "resetTimerbutton";
            this.resetTimerbutton.Size = new System.Drawing.Size(78, 36);
            this.resetTimerbutton.TabIndex = 4;
            this.resetTimerbutton.Text = "Reset Timer";
            this.resetTimerbutton.UseVisualStyleBackColor = true;
            // 
            // SingleTimerEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(330, 77);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(350, 120);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 120);
            this.Name = "SingleTimerEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
        private System.Windows.Forms.TextBox TimerNameLabel;
        private System.Windows.Forms.TextBox TimerElapsedTimeLabel;
        private System.Windows.Forms.Button resetTimerbutton;
    }
}

