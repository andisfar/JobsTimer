using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace JobTimer
{
    public partial class JobTimerForm : Form
    {
        private static JobTimerForm instance = null;
        private static object lockForm = new object();

        public static JobTimerForm GetInstance
        {
            get
            {
                lock (lockForm)
                {
                    if (instance == null)
                        instance = new JobTimerForm();
                    return instance;
                }
            }
        }

        private Assembly _assembly;
        private Stream _imageStream;

        private MdiLayout mdiLayoutMode = MdiLayout.TileHorizontal;

        private string DataFile
        {
            get; set;
        }
        const string _fileSubPath = @"Data\SavedTimers.xml";
        const string _path = @"Data";

        public JobTimerForm()
        {
            InitializeComponent();

            DataFile = Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), _fileSubPath);
            if (!Directory.Exists(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), _path)))
                Directory.CreateDirectory(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), _path));

            if (File.Exists(DataFile))
                GetSavedTimers();
            _isClosing = true;
        }

        private void GetSavedTimers()
        {
            DataTable dt = CreateDataTable();
            dt.ReadXml(DataFile);
            dt.AcceptChanges();

            foreach (DataRow dr in dt.Rows)
            {
                timersDataGridView.Rows.Add(dr[0].ToString(), dr[1].ToString());
            }
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoClose();
        }

        private void JobTimerForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey | Keys.Q:
                    DoClose();
                    break;

                default:
                    break;
            }
        }

        private void DoClose()
        {
            _isClosing = true;
            Properties.Settings.Default.Save();
            WriteDataToXml();
            this.Hide();
            jobTimersIcon.Visible = false;
            this.FormClosing -= JobTimerForm_FormClosing;
            this.Close();
            Application.Exit();
        }

        private bool _isClosing;

        public bool IsClosing
        {
            get { return _isClosing; }
        }

        private void JobTimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isClosing = false;
            e.Cancel = true;
            jobTimersContextMenu.Text = this.Text;
            jobTimersIcon.Text = this.Text;
            jobTimersIcon.BalloonTipText = this.Text;
            this.ShowInTaskbar = false;
            this.Visible = false;
            jobTimersIcon.Icon = this.Icon;
            jobTimersIcon.Visible = true;
        }

        private void WriteDataToXml()
        {
            DataTable dt = CreateDataTable();

            foreach (DataGridViewRow dgvr in Rows)
            {
                if (!dgvr.IsNewRow)
                    dt.Rows.Add(dgvr.Cells[0].EditedFormattedValue.ToString(), dgvr.Cells[1].EditedFormattedValue.ToString());
            }

            dt.AcceptChanges();

            dt.WriteXml(DataFile);
        }

        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable("Stored_Timers");
            DataColumn dc = new DataColumn("Ticks", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("Text", typeof(string));
            dt.Columns.Add(dc);
            return dt;
        }

        private void JobTimerForm_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("JobTimer.appLogo.bmp");
            this.Text += "-[Active]";
        }

        private void NewTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewTimer();
        }

        private void StartNewTimer()
        {
            SingleTimer.SingleTimerForm st = new SingleTimer.SingleTimerForm("00:00:00");
            GetChildWindowName(ref st);

            if (st.Text == "Cancel")
            {
                st = null;
                return;
            }
            st.OnRequestToClose += SingleTimerForm_OnRequestToClose;
            st.OnTimerFormClosing += OnMDIChildFormClosing;
            st.OnResetTimer += SingleTimerForm_OnResetTimer;
            st.OnUpdateTimer += SingleTimerForm_OnUpdateTimer;
            st.RowIndex = timersDataGridView.Rows.Add(st.ElapsedTimeOffset, st.Text);
            st.MdiParent = this;
            st.Show();
            this.LayoutMdi(mdiLayoutMode);
        }

        private void SingleTimerForm_OnRequestToClose(object sender, FormClosingEventArgs e)
        {
            e.Cancel = IsClosing && (MdiChildren.Length == 0);
            return;
        }

        private void OnMDIChildFormClosing(object sender, SingleTimer.SingleTimerEventArgs e)
        {
            if (Rows[e.RowIndex] != null)
            {
                timersDataGridView.Rows[e.RowIndex].Cells[0].Value = e.ElapsedTime;
                timersDataGridView.Rows[e.RowIndex].Cells[1].Value = e.WindowTitle;
                WriteDataToXml();
                SingleTimer.SingleTimerForm form = MatchingMDIChild(e.RowIndex);
                form.OnTimerFormClosing -= OnMDIChildFormClosing;
                form.OnRequestToClose -= SingleTimerForm_OnRequestToClose;
                form.OnResetTimer -= SingleTimerForm_OnResetTimer;
                form.OnUpdateTimer -= SingleTimerForm_OnUpdateTimer;
                form.Close();
            }
        }

        private void GetChildWindowName(ref SingleTimer.SingleTimerForm mdiChild)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Prompt", "Title", "Cancel");
            mdiChild.Text = (input == string.Empty) ? "Cancel" : input;
        }

        private void TimersDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == timersDataGridView.NewRowIndex)
                StartNewTimer();
            else
            {
                string windowTitle = Rows[e.RowIndex].Cells[1].EditedFormattedValue.ToString();
                foreach (Form child in this.MdiChildren)
                {
                    System.Diagnostics.Debug.Print("current iterator:" + child.Text.ToLower());
                    System.Diagnostics.Debug.Print("search criteria:" + windowTitle.ToLower());
                    if (child.Text.ToLower().Equals(windowTitle.ToLower()))
                    {
                        child.Focus();
                        return;
                    }
                }
                LoadPreviousTimer(e.RowIndex);
            }
        }

        private void LoadPreviousTimer(int rowIndex)
        {
            string timeSpanOffset = Convert.ToString(Rows[rowIndex].Cells[0].EditedFormattedValue);
            string text = Convert.ToString(Rows[rowIndex].Cells[1].EditedFormattedValue);

            SingleTimer.SingleTimerForm st = new SingleTimer.SingleTimerForm(timeSpanOffset)
            {
                Text = text
            };

            st.OnTimerFormClosing += OnMDIChildFormClosing;
            st.OnRequestToClose += SingleTimerForm_OnRequestToClose;
            st.OnResetTimer += SingleTimerForm_OnResetTimer;
            st.OnUpdateTimer += SingleTimerForm_OnUpdateTimer;
            st.RowIndex = rowIndex;
            st.MdiParent = this;
            st.Show();

            this.LayoutMdi(mdiLayoutMode);
        }

        private void SingleTimerForm_OnUpdateTimer(object sender, SingleTimer.SingleTimerEventArgs e)
        {
            if (Rows[e.RowIndex] != null)
            {
                Rows[e.RowIndex].Cells[0].Value = e.ElapsedTime;
                WriteDataToXml();
            }
        }

        private void SingleTimerForm_OnResetTimer(object sender, SingleTimer.SingleTimerEventArgs e)
        {
            if (Rows[e.RowIndex] != null)
            {
                Rows[e.RowIndex].Cells[0].Value = e.ElapsedTime;
            }
        }

        private DataGridViewRowCollection Rows
        {
            get { return timersDataGridView.Rows; }
        }

        private void TimersGridViewDeleteItem_Click(object sender, EventArgs e)
        {
            List<int> deleteables = new List<int>();
            foreach (DataGridViewRow dgvr in timersDataGridView.SelectedRows)
            {
                deleteables.Add(dgvr.Index);
            }

            foreach (int index in deleteables)
            {
                for (int idx = 0; idx < this.MdiChildren.Length; idx++)
                {
                    if (((SingleTimer.SingleTimerForm)this.MdiChildren[idx]).RowIndex == index)
                    {
                        SingleTimer.SingleTimerForm form = (SingleTimer.SingleTimerForm)this.MdiChildren[idx];
                        form.OnTimerFormClosing -= OnMDIChildFormClosing;
                        form.OnRequestToClose -= SingleTimerForm_OnRequestToClose;
                        form.OnResetTimer -= SingleTimerForm_OnResetTimer;
                        form.OnUpdateTimer -= SingleTimerForm_OnUpdateTimer;
                        form.Close();
                        break;
                    }
                }
                timersDataGridView.Rows.RemoveAt(index);
            }
        }

        private void JobTimerForm_ResizeEnd(object sender, EventArgs e)
        {
            this.LayoutMdi(mdiLayoutMode);
        }

        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreMainWindowState();
        }

        public void RestoreMainWindowState()
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
            jobTimersIcon.Visible = false;
            this.LayoutMdi(mdiLayoutMode);
            foreach (ToolStripItem childItem in activeTimersMenu.DropDownItems)
            {
                childItem.MouseEnter -= ChildItem_MouseEnter;
                childItem.MouseDown -= ChildItem_MouseDown;
                childItem.MouseLeave -= ChildItem_MouseLeave;
            }
            activeTimersMenu.DropDownItems.Clear();
            currentItems.Clear();
        }

        private void QuitContextMenuItem_Click(object sender, EventArgs e)
        {
            DoClose();
        }

        private void ChildItem_CheckStateChanged(object sender, EventArgs e)
        {
            ToolStripItem menuItem = (ToolStripItem)sender;
            SingleTimer.SingleTimerForm child = MatchingMDIChild(menuItem.Tag.ToString().SubStringAfterIndexOf('|').ToInt());
            if (child != null)
            {
                if (child.TimerIsRunning)
                {
                    child.StopTimer();
                }
                else
                {
                    child.StartTimer();
                }
                menuItem.BackColor = child.TimerIsRunning ? Color.LightSeaGreen : Color.LightPink;
                menuItem.ForeColor = child.TimerIsRunning ? Color.NavajoWhite : Color.MintCream;
            }
            else
            {
                menuItem.BackColor = SystemColors.InactiveCaption;
                menuItem.ForeColor = SystemColors.InactiveCaptionText;
            }
        }

        private void ChildItem_MouseEnter(object sender, EventArgs e)
        {
            int rowIndex = ((ToolStripItem)sender).Tag.ToString().SubStringAfterIndexOf('|').ToInt();
            ((ToolStripItem)sender).ToolTipText = MatchingMDIChildTitle(rowIndex);
        }

        private void ChildItem_MouseLeave(object sender, EventArgs e)
        {
            ((ToolStripItem)sender).ToolTipText = string.Empty;
        }

        private string MatchingMDIChildTitle(int rowIndex)
        {
            return MatchingMDIChild(rowIndex)?.RunningElapsedTime;
        }

        private SingleTimer.SingleTimerForm MatchingMDIChild(int rowIndex)
        {
            foreach (SingleTimer.SingleTimerForm child in this.MdiChildren)
            {
                if (child.RowIndex == rowIndex)
                    return child;
            }
            return null;
        }

        private List<String> currentItems = new List<string>();

        private void JobTimersContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (SingleTimer.SingleTimerForm child in this.MdiChildren)
            {
                if (currentItems.Contains(child.Text)) continue;
                ToolStripMenuItem childItem = new ToolStripMenuItem(child.Text, (Image.FromHbitmap(child.Icon.ToBitmap().GetHbitmap())));
                currentItems.Add(child.Text);
                childItem.Tag = child.Text + "-[{0}]|" + child.RowIndex.ToString();
                activeTimersMenu.DropDownItems.Add(childItem);
                child.OnUpdateTimer += ChildItem_OnUpdateTimer;
                childItem.CheckOnClick = true;
                childItem.Checked = child.TimerIsRunning;
                childItem.Image = new Bitmap(_imageStream);
                childItem.MouseEnter += ChildItem_MouseEnter;
                childItem.MouseDown += ChildItem_MouseDown;
                childItem.MouseLeave += ChildItem_MouseLeave;
                childItem.CheckStateChanged += ChildItem_CheckStateChanged;
                childItem.BackColor = child.TimerIsRunning ? Color.LightSeaGreen : Color.LightPink;
                childItem.ForeColor = child.TimerIsRunning ? Color.NavajoWhite : Color.MintCream;
            }
        }

        private void ChildItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolStripMenuItem childItem = (ToolStripMenuItem)sender;
                string shortName = childItem.Tag.ToString().SubStringByIndexOf("-[");
                int rowIndex = childItem.Tag.ToString().SubStringAfterIndexOf('|').ToInt();
                if (MessageBox.Show(this, string.Format("Reset Timer '{0}'", shortName), shortName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SingleTimer.SingleTimerForm child = MatchingMDIChild(rowIndex);
                    child.ResetTimer(false);
                    Rows[child.RowIndex].Cells[0].Value = child.RunningElapsedTime;
                    childItem.BackColor = child.TimerIsRunning ? Color.LightSeaGreen : Color.LightPink;
                    childItem.ForeColor = child.TimerIsRunning ? Color.NavajoWhite : Color.MintCream;
                }
            }
        }

        private void ChildItem_OnUpdateTimer(object sender, SingleTimer.SingleTimerEventArgs e)
        {
            SingleTimer.SingleTimerForm child = (SingleTimer.SingleTimerForm)sender;
            foreach (ToolStripMenuItem childItem in activeTimersMenu.DropDownItems)
            {
                if (childItem.Text.StartsWith(child.Text))
                {
                    childItem.Text = string.Format(childItem.Tag.ToString().SubStringByIndexOf("|"), e.ElapsedTime);
                    childItem.ToolTipText = e.ElapsedTime;
                    childItem.BackColor = child.TimerIsRunning ? Color.LightSeaGreen : Color.LightPink;
                    childItem.ForeColor = child.TimerIsRunning ? Color.NavajoWhite : Color.MintCream;
                    break;
                }
            }
        }

        private void JobTimerForm_MdiChildActivate(object sender, EventArgs e)
        {
            _isClosing = true;
        }

        private void ResetTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (timersDataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow _row = timersDataGridView.SelectedRows[0];
                string message = string.Format("Reset timer value for:\n'{0}'", _row.Cells[1].EditedFormattedValue);
                string caption = string.Format("{0}", "Reset Timer Value");
                if (MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _row.Cells[0].Value = "00:00:00";
                }
            }
        }

        private void DgvContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                e.Cancel = true;
                return;
            }
        }

        private void ActiveTimersMenu_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in activeTimersMenu.DropDownItems)
            {
                SingleTimer.SingleTimerForm child = MatchingMDIChild(item.Tag.ToString().SubStringAfterIndexOf('|').ToInt());
                if (child != null)
                {
                    item.BackColor = child.TimerIsRunning ? Color.LightSeaGreen : Color.LightPink;
                    item.ForeColor = child.TimerIsRunning ? Color.NavajoWhite : Color.MintCream;
                }
                else
                {
                    item.BackColor = Color.LightPink;
                    item.ForeColor = Color.MintCream;
                }
            }
        }

        private void TimersDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var htest = timersDataGridView.HitTest(e.X, e.Y);
                timersDataGridView.ClearSelection();
                timersDataGridView.Rows[htest.RowIndex].Selected = true;
            }
        }

        private void ResetTimerToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            ToolStripItem childItem = (ToolStripItem)sender;
            string _time = timersDataGridView.SelectedRows[0].Cells[0].EditedFormattedValue.ToString();
            string _name = timersDataGridView.SelectedRows[0].Cells[1].EditedFormattedValue.ToString();
            childItem.Text = string.Format(childItem.Tag.ToString(), _time, _name);
        }

        private ToolStripItem ChildItem(object sender)
        {
            return (ToolStripItem)sender;
        }

        private void ResetTimerToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ToolStripItem _childItem = ChildItem(sender);
            _childItem.Text = _childItem.Tag.ToString().SubStringByIndexOf("[").Trim();
        }

        private void JobTimerForm_SizeChanged(object sender, EventArgs e)
        {
            LayoutMdi(mdiLayoutMode);
        }
    }

    public class TimersFunctionNotImplemented : NotImplementedException
    {
        public TimersFunctionNotImplemented(string Message)
        {
            throw new System.NotImplementedException(Message);
        }
    }

    public static class Extentions
    {
        // returns a substring from pos 0 to index of 'criteria' in a string
        public static string SubStringByIndexOf(this string me, string criteria)
        {
            int split_pos = me.IndexOf(criteria);
            if (split_pos < 0) return string.Empty;
            return me.Substring(0, split_pos);
        }

        // returns the remainder of a string after the index of 'criteria', criteria is a  char
        public static string SubStringAfterIndexOf(this string me, char criteria)
        {
            int pos = me.IndexOf(criteria);
            return (pos + 1 <= me.Length) ? me.Substring(pos + 1) : string.Empty;
        }

        //  returns the remainder of a string after the index of 'criteria', criteria is a  string
        public static string SubStringAfterIndexOf(this string me, string criteria)
        {
            int pos = me.IndexOf(criteria);
            return (pos + 1 <= me.Length) ? me.Substring(pos + 1) : string.Empty;
        }

        // converts a string into is integer representation
        public static int ToInt(this string me)
        {
            return Int32.Parse(me);
        }
    }
}
