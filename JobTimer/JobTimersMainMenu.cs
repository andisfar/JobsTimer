using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SingleTimerLib;

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

        private string DataFile { get; set; }

        private SingleTimerLib.SingleTimersCollection _timers = null;

        public SingleTimerLib.SingleTimersCollection TimersList
        {
            get { return _timers; }
        }

        public JobTimerForm()
        {
            InitializeComponent();

            _timers = new SingleTimerLib.SingleTimersCollection();

            DataFile = Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), @"Data\SavedTimers.xml");
            if (!Directory.Exists(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), @"Data")))
                Directory.CreateDirectory(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), @"Data"));
            try
            {
                if (File.Exists(DataFile))
                {
                    GetSavedTimers();
                }
            }
            catch (Exception ex)
            {
                DebugPrint(ex.Message);
            }
            finally
            {
                timersDataGridView.DataSource = SavedTimersBindingSource;
                SetupSavedTimers();               
            }
            jobTimersIcon.Visible = false;
            _isClosing = true;
        }

        private void SetupSavedTimers()
        {
            Application.DoEvents();
            timersDataGridView.Update();
            for(int idx = 0; idx < Timers.Rows.Count; ++idx)
            {
                DataRow _r = Timers.Rows[idx];

                if (!TimerName2RowIndexDictionary.ContainsKey(_r[0].ToString()))
                {
                    TimerName2RowIndexDictionary.Add(_r[0].ToString(), idx);
                }
                SingleTimerLib.SingleTimer _t = TimersList.AddTimer(_r[0].ToString(), new SingleTimerLib.SingleTimer(idx, _r[0].ToString(), _r[1].ToString()));
                _t.PropertyChanged += SingleTimer_PropertyChanged;
                _t.SingleTimerChanged += SingleTimer_SingleTimerChanged;
                _t.TimerReset += SingleTimer_OnTimerReset;
            }
        }

        private DataTable GetSavedTimers()
        {
            Timers.ReadXml(DataFile);
            Timers.AcceptChanges();
            timersDataGridView.Update();
            return Timers;
        }

        private Dictionary<string, int> _timerName2RowIndexDictionary = new Dictionary<string, int>();

        private Dictionary<string, int> TimerName2RowIndexDictionary
        {
            get
            {
                return _timerName2RowIndexDictionary;
            }
            set
            {
                _timerName2RowIndexDictionary = value;
            }
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e) => DoClose();

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

            List<string> listOfKeys = new List<string>();
            foreach (string _k in TimersList.Keys)
                listOfKeys.Add(_k);
            foreach(string name in listOfKeys)
            {
                SingleTimerLib.SingleTimer _t = TimersList[name];
                _t.PropertyChanged -= SingleTimer_PropertyChanged;               
                _t.SingleTimerChanged -= SingleTimer_SingleTimerChanged;
                _t.TimerReset -= SingleTimer_OnTimerReset;
                _t.StopTimer();
                TimersList.Remove(_t.CanonicalName);
                TimerName2RowIndexDictionary.Remove(_t.CanonicalName);
                DebugPrint(string.Format("Timer {0} is about to be disposed!", _t.Name));
                _t.Dispose();
            }

            WriteDataToXml();

            listOfKeys.Clear();
            listOfKeys = null;

            Hide();
            jobTimersIcon.Visible = false;
            FormClosing -= JobTimerForm_FormClosing;
            Close();
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
            jobTimersContextMenu.Text = Text;
            jobTimersIcon.Text = Text;
            jobTimersIcon.BalloonTipText = Text;
            ShowInTaskbar = false;
            Visible = false;
            jobTimersIcon.Icon = Icon;
            jobTimersIcon.Visible = true;
        }

        private void WriteDataToXml()
        {
            Timers.EnsureCanonicalTimerNames();
            Timers.AcceptChanges();
            Timers.WriteXml(DataFile);
            string message = string.Format("Writing {0} rows to database: '{1}'", Timers.Rows.Count, DataFile);
            DebugPrint(message);
            foreach(DataRow _r in Timers.Rows)
            {
                message = string.Format("Timer Name {0} is at {1} of elapsed time!", _r[0].ToString(), _r[1].ToString());
                DebugPrint(message);
            }
        }

        private void JobTimerForm_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("JobTimer.appLogo.bmp");
            Text += "-[Active]";
        }

        private void NewTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SingleTimer _t = StartNewTimer;
        }

        private SingleTimer StartNewTimer
        {
            get
            {
                List<string> names = Timers.Columns.ColumnNames();
                string timerName = GetTimerName("Enter name for new timer:","New Timer");
                if (timerName == "Cancel")
                {
                    return null;
                }

                DataRow _r = null;
                try
                {
                    _r = Timers.Rows.Add(timerName, "00:00:00");
                }
                catch (System.Data.ConstraintException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch(Exception ex)
                {
                    throw ex;
                }

                Timers.AcceptChanges();
                timersDataGridView.Update();

                try
                {
                    int index = GetRowIndex(timerName);
                    Debug.Assert(index >= 0);
                    TimerName2RowIndexDictionary.Add(timerName, GetRowIndex(timerName));
                }
                catch (ArgumentException ex)
                {
                    DebugPrint(ex.Message);
                    DebugPrint("Name already exists!");
                    DebugPrint("Removing row from database! No Duplicate timer names allowed!");
                    Timers.Rows.RemoveAt(0);
                    Timers.AcceptChanges();
                    timersDataGridView.Update();
                    return null;
                }

                SingleTimerLib.SingleTimer st = TimersList.AddTimer(timerName, new SingleTimerLib.SingleTimer(TimerName2RowIndexDictionary[timerName], timerName, "00:00:00"));

                st.TimerReset += SingleTimer_OnTimerReset;
                st.PropertyChanged += SingleTimer_PropertyChanged;
                st.SingleTimerChanged += SingleTimer_SingleTimerChanged;
                return st;
            }
        }

        private int GetRowIndex(string timerName)
        {
            foreach (DataGridViewRow _r in Rows)
            {
                if (_r.TimerCanonicalName() == timerName)
                    return _r.Index;
            }
            return -1;
        }

        private void SingleTimer_SingleTimerChanged(object sender, SingleTimerLib.SingleTimerLibEventArgs e)
        {
            ThreadSafeUpdateTimerElapsedTime(e);            
            DebugPrint(string.Format("From {3}: Name: {0}, Elapsed: {1}, RowIndex: {2}", e.CanonicalName, e.ElapsedTime, e.RowIndex, "SingleTimer_SingleTimerChanged"));

            foreach (ToolStripMenuItem childItem in activeTimersMenu.DropDownItems)
            {
                if (childItem.Tag.ToString().SubStringAfterIndexOf('|').ToInt() == e.RowIndex)
                {
                    try
                    {
                        string _text = string.Format(childItem.Tag.ToString().SubStringByIndexOf("|"), e.ElapsedTime);
                        try
                        {
                            childItem.Text = _text;
                        }
                        catch (Exception ex)
                        {
                            DebugPrint(ex.InnerException.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugPrint(ex.StackTrace.ToString());
                    }
                    finally
                    {
                        DebugPrint(string.Format("From {0}: {1}", "SingleTimer_SingleTimerChanged", childItem.Text));
                    }
                }
            }
        }

        private void SingleTimer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SingleTimerLib.SingleTimer _t = (SingleTimerLib.SingleTimer)sender;
            if (sender == null) return;
            switch (e.PropertyName)
            {
                case nameof(SingleTimerLib.SingleTimer.RunningElapsedTime):                    
                    ThreadSafeUpdateTimerElapsedTime(_t);                   
                    DebugPrint(string.Format("From {1}: [{2}] Elapsed {0}", _t.RunningElapsedTime, "SingleTimer_PropertyChanged", e.PropertyName));
                    break;

                case nameof(SingleTimerLib.SingleTimer.RowIndex):
                    break;

                case nameof(SingleTimerLib.SingleTimer.Name):
                    DebugPrint(string.Format("{0} is a new name!", _t.Name));
                    string TimerOldName = Rows[_t.RowIndex].TimerCanonicalName();
                    TimerName2RowIndexDictionary.Remove(TimerOldName);
                    TimersList.Remove(TimerOldName);
                    TimerName2RowIndexDictionary.Add(_t.CanonicalName, _t.RowIndex);
                    ChangeMenuItemTitlesBasedOnTimerName(TimerOldName, _t.CanonicalName);
                    Debug.Assert(TimerName2RowIndexDictionary.ContainsKey(_t.CanonicalName));
                    TimersList.AddTimer(_t.CanonicalName, _t);
                    ThreadSafeUpdateTimerName(_t);
                    break;

                case nameof(SingleTimerLib.SingleTimer.IsRunning):
                    string message1 = "{0}: {1} is running! [{2}]";
                    string message2 = "{0}: {1} is stopped! [{2}]";
                    DebugPrint(string.Format(_t.IsRunning ? message1 : message2, "SingleTimer_PropertyChanged", _t.Name, e.PropertyName));
                    break;

                default:
                    break;
            }
        }

        private void ChangeMenuItemTitlesBasedOnTimerName(string timerOldName, string timerNewCanonicalName)
        {       
            foreach(ToolStripMenuItem childItem in activeTimersMenu.DropDownItems)
            {
                if(childItem.Text.StartsWith(timerOldName))
                {
                    childItem.Text = childItem.Text.Replace(timerOldName, timerNewCanonicalName);
                    childItem.Tag = string.Format("{0}-[{1}]|{2}",timerNewCanonicalName,"{0}", TimerName2RowIndexDictionary[timerNewCanonicalName].ToString());
                    break;
                }
            }
        }

        private void ThreadSafeUpdateTimerElapsedTime(SingleTimerLibEventArgs e) => ThreadSafeUpdateTimerElapsedTime(TimersList[e.CanonicalName]);

        private void ThreadSafeUpdateTimerElapsedTime(SingleTimer t)
        {
            if (timersDataGridView.InvokeRequired)
            {
                DebugPrint("[Update Timer Elapsed Time]=>Running on non GUI Thread=>calling method on GUI Thread!");
                timersDataGridView.Invoke(new Action<SingleTimer>(ThreadSafeUpdateTimerElapsedTime), t);
                return;
            }
            DebugPrint("[Update Timer Elapsed Time]=>Running on GUI Thread!");           
            Rows[t.RowIndex].Cells[1].Value = t.RunningElapsedTime;
            Rows[t.RowIndex].Cells[0].Value = t.Name;
        }

        private void ThreadSafeUpdateTimerName(SingleTimer t)
        {
            if (timersDataGridView.InvokeRequired)
            {
                DebugPrint("[Rename Timer]=>Running on non GUI Thread=>calling method on GUI Thread!");
                timersDataGridView.Invoke(new Action<SingleTimer>(ThreadSafeUpdateTimerName), t);
                return;
            }
            DebugPrint("[Rename Timer]=>Running on GUI Thread!");           
            Rows[t.RowIndex].Cells[0].Value = t.Name;
        }

        private void SingleTimer_OnTimerReset(object sender, SingleTimerLib.SingleTimerLibEventArgs e)
        {
            ThreadSafeUpdateTimerElapsedTime(e);
            DebugPrint(string.Format("From {3}: Name {0}, Elapsed {1}, RowIndex{2}", e.CanonicalName, e.ElapsedTime, e.RowIndex, "SingleTimer_OnTimerReset"));
        }

        private string GetTimerName(string prompt = "Prompt", string title = "Title", string defaultName = "Cancel")
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, defaultName);
            return (input == string.Empty) ? "Cancel" : input;
        }

        private DataGridViewRowCollection Rows => timersDataGridView.Rows;

        private void TimersGridViewDeleteItem_Click(object sender, EventArgs e)
        {
            List<int> deleteables = new List<int>();
            foreach (ListViewItem _i in timersDataGridView.SelectedRows)
            {
                deleteables.Add(_i.Index);
            }

            timersDataGridView.ClearSelection();

            string message = "Confirm deletion of {0}";
            string caption = Application.ProductName + "Delete {0}";

            timersDataGridView.ClearSelection();
            foreach (int index in deleteables)
            {
                Rows[index].Selected = true;
                string timerName = Rows[index].TimerCanonicalName();

                if (AnswerYesNo(string.Format(message, timerName), string.Format(caption, timerName)) == DialogResult.Yes)
                {
                    SingleTimerLib.SingleTimer _t = TimersList[timerName];
                    _t.PropertyChanged -= SingleTimer_PropertyChanged;
                    _t.SingleTimerChanged -= SingleTimer_SingleTimerChanged;
                    _t.TimerReset -= SingleTimer_OnTimerReset;
                    TimersList.Remove(timerName);
                    timersDataGridView.Rows.RemoveAt(index);
                }

                timersDataGridView.ClearSelection();
            }
        }

        private static DialogResult AnswerYesNo(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
        }

        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreMainWindowState();
        }

        public void RestoreMainWindowState()
        {
            Application.DoEvents();
            ShowInTaskbar = true;
            Visible = true;
            jobTimersIcon.Visible = false;
            foreach (ToolStripMenuItem childItem in activeTimersMenu.DropDownItems)
            {
                childItem.MouseEnter -= ChildItem_MouseEnter;
                childItem.MouseDown -= ChildItem_MouseDown;
                childItem.MouseLeave -= ChildItem_MouseLeave;
                childItem.CheckStateChanged -= ChildItem_CheckStateChanged;
            }
            activeTimersMenu.DropDownItems.Clear();
            currentItems.Clear();
            Application.DoEvents();
        }

        private void QuitContextMenuItem_Click(object sender, EventArgs e)
        {
            DoClose();
        }

        private void ChildItem_CheckStateChanged(object sender, EventArgs e)
        {
            ToolStripItem menuItem = (ToolStripItem)sender;
            SingleTimerLib.SingleTimer _t = TimersList[menuItem.Text.SubStringByIndexOf("-[").Trim()];
            Color _fc = Color.Black;
            Color _bc = Color.White;
            if (_t != null)
            {
                if (_t.IsRunning)
                {
                    _fc = Color.NavajoWhite;
                    _bc = Color.LightSeaGreen;

                    _t.StopTimer();
                    DebugPrint(string.Format("{0}: {1} is no longer running!", "ChildItem_CheckStateChanged", _t.Name));
                }
                else
                {
                    _t.StartTimer();
                    DebugPrint(string.Format("{0}: {1} is now running!", "ChildItem_CheckStateChanged", _t.Name));
                }
                menuItem.BackColor = _t.IsRunning ? Color.LightSeaGreen : Color.LightPink;
                menuItem.ForeColor = _t.IsRunning ? Color.NavajoWhite : Color.MintCream;
                Rows[_t.RowIndex].Cells[1].Style.BackColor = menuItem.BackColor;
                Rows[_t.RowIndex].Cells[1].Style.ForeColor = menuItem.ForeColor;
            }
            else
            {
                menuItem.BackColor = SystemColors.InactiveCaption;
                menuItem.ForeColor = SystemColors.InactiveCaptionText;
            }
        }

        private void DebugPrint(string message, [CallerMemberName] string caller="")
        {
            string messageWithTimeStamp = string.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss:fff"), message);
            Debug.Print(messageWithTimeStamp);
        }

        private void ChildItem_MouseEnter(object sender, EventArgs e)
        {
            string _name = ((ToolStripItem)sender).Tag.ToString().SubStringByIndexOf("-[");
            ((ToolStripItem)sender).ToolTipText = TimersList[_name].RunningElapsedTime;
        }

        private void ChildItem_MouseLeave(object sender, EventArgs e)
        {
            ((ToolStripItem)sender).ToolTipText = string.Empty;
        }

        private List<String> currentItems = new List<string>();

        private void JobTimersContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (SingleTimerLib.SingleTimer _t in TimersList.Values)
            {                
                if (currentItems.Contains(_t.CanonicalName)) continue;
                ToolStripMenuItem childItem = new ToolStripMenuItem(_t.CanonicalName, (Image.FromHbitmap(Icon.ToBitmap().GetHbitmap())));
                currentItems.Add(_t.CanonicalName);
                childItem.Tag = _t.CanonicalName + "-[{0}]|" + _t.RowIndex.ToString();
                string _text = string.Format(childItem.Tag.ToString().SubStringByIndexOf("|"), _t.RunningElapsedTime);
                childItem.Text = _text;
                activeTimersMenu.DropDownItems.Add(childItem);
                SetupChildItem(_t, childItem);
            }
        }

        private void SetupChildItem(SingleTimerLib.SingleTimer _t, ToolStripMenuItem childItem)
        {
            childItem.CheckOnClick = true;
            childItem.Checked = _t.IsRunning;
            childItem.Image = new Bitmap(_imageStream);
            childItem.MouseEnter += ChildItem_MouseEnter;
            childItem.MouseDown += ChildItem_MouseDown;
            childItem.MouseLeave += ChildItem_MouseLeave;
            childItem.CheckStateChanged += ChildItem_CheckStateChanged;
            childItem.BackColor = _t.IsRunning ? Color.LightSeaGreen : Color.LightPink;
            childItem.ForeColor = _t.IsRunning ? Color.NavajoWhite : Color.MintCream;
        }

        private void ChildItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                timersDataGridView.ClearSelection();
                ToolStripMenuItem childItem = (ToolStripMenuItem)sender;
                int rowIndex = childItem.Tag.ToString().SubStringAfterIndexOf('|').ToInt();
                string shortName = Rows[rowIndex].TimerCanonicalName();
                EditTimerFromNotificationMenu(childItem, shortName, rowIndex);
                Rows[rowIndex].Selected = true;
            }
        }

        private void EditTimerFromNotificationMenu(ToolStripMenuItem childItem, string shortName, int rowIndex)
        {
            timersDataGridView.CurrentCell = timersDataGridView[0, rowIndex];
            timersDataGridView.BeginEdit(false);
            //if (MessageBox.Show(this, string.Format("Reset Timer '{0}'", shortName), shortName, MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    SingleTimerLib.SingleTimer _t = TimersList[shortName];
            //    _t.ResetTimer();
            //    ThreadSafeUpdateTimerElapsedTime(new SingleTimerLib.SingleTimerLibEventArgs(_t.RunningElapsedTime, _t.Name, _t.RowIndex, 0));
            //    childItem.BackColor = _t.IsRunning ? Color.LightSeaGreen : Color.LightPink;
            //    childItem.ForeColor = _t.IsRunning ? Color.NavajoWhite : Color.MintCream;
            //}
        }

        private void QueryUserResetTimer(int rowIndex)
        {
            string name = Rows[rowIndex].TimerCanonicalName();
            if (MessageBox.Show(this, string.Format("Reset Timer '{0}'", name), name, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SingleTimerLib.SingleTimer _t = TimersList[name];
                if (_t != null)
                {
                    _t.ResetTimer();
                }
                else
                {
                    ThreadSafeUpdateGridViewRowElapsedTimerValue(rowIndex,"00:00:00");
                }
            }
        }

        private void ThreadSafeUpdateGridViewRowElapsedTimerValue(int rowIndex, string elapsedTime)
        {
            if(timersDataGridView.InvokeRequired)
            {
                timersDataGridView.Invoke(new Action<int,string>(ThreadSafeUpdateGridViewRowElapsedTimerValue),rowIndex,elapsedTime);
                return;
            }
            Rows[rowIndex].SetTimerElapsedTimeValue(elapsedTime);
        }

        private void ResetTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThreadSafeResetSelectedTimers();
        }

        private void ThreadSafeResetSelectedTimers()
        {
            if(timersDataGridView.InvokeRequired)
            {
                timersDataGridView.Invoke(new Action(ThreadSafeResetSelectedTimers));
                return;
            }

            if (timersDataGridView.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow dr in timersDataGridView.SelectedRows)
                {
                    string message = string.Format("Reset timer value for:\n'{0}'", dr.TimerCanonicalName());
                    string caption = string.Format("{0}", "Reset Timer Value");
                    if (MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        dr.SetTimerElapsedTimeValue("00:00:00");
                    }
                }
            }
        }

        private void ActiveTimersMenu_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in activeTimersMenu.DropDownItems)
            {
                string name = item.Text.SubStringByIndexOf("-[");
                SingleTimerLib.SingleTimer _t = TimersList[name];
                if (_t != null)
                {
                    item.BackColor = _t.IsRunning ? Color.LightSeaGreen : Color.LightPink;
                    item.ForeColor = _t.IsRunning ? Color.NavajoWhite : Color.MintCream;
                }
                else
                {
                    item.BackColor = Color.LightPink;
                    item.ForeColor = Color.MintCream;
                }
            }
        }

        private void QueryStartNewTimer(object sender, MouseEventArgs e)
        {
            // update the selected row

            DataGridView.HitTestInfo hitTest = timersDataGridView.HitTest(e.X, e.Y);
            SingleTimer _t;

            if (hitTest.RowIndex < 0)
            {
               _t  = StartNewTimer;
                return;
            }

            Rows[hitTest.RowIndex].Selected = true;           
        }

        private void QueryUserRenameTimer(int rowIndex)
        {
            string oldname = Rows[rowIndex].TimerCanonicalName();
            string newname = GetTimerName("Specify as new name:","Rename Timer", oldname);
            if (newname == "Cancel") return;
            if (newname == oldname) return;

            SingleTimerLib.SingleTimer _t = TimersList[oldname];
            if (_t != null)
            {
                TimerName2RowIndexDictionary.Remove(_t.Name);
                TimersList.Remove(_t.Name);
                TimerName2RowIndexDictionary.Add(newname, _t.RowIndex);
                _t.ReNameTimer(newname);
            }
            else
            {
                Rows.RemoveAt(rowIndex);
                TimerName2RowIndexDictionary.Remove(oldname);
                TimersList.Remove(oldname);
            }

        }

        private void TimersDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DebugPrint(string.Format("timersDataGridView_RowsAdded: {0} rows added!", e.RowCount));
        }

        private void TimersDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e) => DebugPrint(string.Format("timersDataGridView_UserAddedRow: elapsed timer:{0}, timer name: {1}!", e.Row.Cells[0], e.Row.Cells[1]));

        private void TimersDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) => DebugPrint(e.Exception.Message);

        private void NewTimerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SingleTimerLib.SingleTimer _t = StartNewTimer;
            if (_t != null)
            {
                if (AnswerYesNo(string.Format("Run timer '{0}'?", _t.Name), _t.Name) == DialogResult.Yes)
                {
                    _t.StartTimer();
                }
            }
            else
            {
                MessageBox.Show("Adding new Timer failed.\nDid you cancel or attempt to add a duplicate name?");
            }
        }

        private void QueryStartOrStopTimer(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                if (AnswerYesNo("Create new timer?", "New Timer Inquiry") == DialogResult.Yes)
                {
                    SingleTimer _t = StartNewTimer;
                }
            }
            else
            {
                DataGridViewRow _r = (DataGridViewRow)Rows[e.RowIndex];
                string name = _r.TimerCanonicalName();

                if (TimersList[name].IsRunning)
                {
                    TimersList[name].StopTimer();
                    DebugPrint(string.Format("{0}: {1} is no longer running!", "timersDataGridView_RowHeaderMouseClick", name));
                    return;
                }

                if (!TimersList[name].IsRunning)
                {
                    TimersList[name].StartTimer();
                    DebugPrint(string.Format("{0}: {1} is now running!", "timersDataGridView_RowHeaderMouseClick", name));
                    return;
                }
            }
        }

        private void BindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            DebugPrint("bindingNavigatorAddNewItem_Click: Adding a Row to DataGridView!");
            ToolStripButton addButton = (ToolStripButton)sender; // maybe future need
            SingleTimer _t = StartNewTimer;
        }

        private void TimersDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) => QueryStartOrStopTimer(sender, e);

        private void BindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            TreadSafeDeleteDataGridViewRows();
        }

        private void TreadSafeDeleteDataGridViewRows()
        {
            if(timersDataGridView.InvokeRequired)
            {
                timersDataGridView.Invoke(new Action(TreadSafeDeleteDataGridViewRows));
                return;
            }
            foreach (DataGridViewRow _dr in timersDataGridView.SelectedRows)
            {
                string name = timersDataGridView.CurrentRow.TimerCanonicalName();
                SingleTimerLib.SingleTimer _t = TimersList[name];
                _t.SingleTimerChanged -= SingleTimer_SingleTimerChanged;
                _t.PropertyChanged -= SingleTimer_PropertyChanged;
                _t.TimerReset -= SingleTimer_OnTimerReset;
                _t.StopTimer();
                Debug.Assert(_t.CanonicalName == Timers.Rows[_t.RowIndex].TimerCanonicalName());
                Timers.Rows.RemoveAt(_t.RowIndex);
                Timers.AcceptChanges();
                ThreadSafeUpdateDataGridView();
                TimerName2RowIndexDictionary.Remove(name);
                TimersList.Remove(name);
                _t.Dispose();
            }
        }

        private void ThreadSafeUpdateDataGridView()
        {
            if(timersDataGridView.InvokeRequired)
            {
                timersDataGridView.Invoke(new Action(ThreadSafeUpdateDataGridView));
                return;
            }
            timersDataGridView.Update();
        }

        private void TimersDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            SingleTimerEditorForm editor = new SingleTimerEditorForm(e.RowIndex);
            editor.QueryTimerNeeded += Editor_QueryTimerNeeded;
            editor.TimerNameChanged += Editor_TimerNameChanged;
            editor.TimerElapsedTimeChanged += Editor_TimerElapsedTimeChanged;
            editor.ShowDialog(this);
            e.Cancel = true;
        }

        private void Editor_TimerElapsedTimeChanged(object sender, SingleTimerEditorFormElapsedTimeEventArgs e)
        {
            Rows[e.RowIndex].SetTimerElapsedTimeValue(e.TimerElapsedTimer);
        }

        private void Editor_TimerNameChanged(object sender, SingleTimerEditorFormNewNameEventArgs e)
        {
            Rows[e.RowIndex].SetTimerNameValue(e.TimerNewName);
            ThreadSafeAutoUpdateTimerName(TimersList[e.TimerOldName], e);
        }

        private void ThreadSafeAutoUpdateTimerName(SingleTimer t, SingleTimerEditorFormNewNameEventArgs e)
        {
            if (t != null)
            {
                DebugPrint(string.Format("Timer being renamed: {0} => {1}", e.TimerOldName, e.TimerNewName));
                TimerName2RowIndexDictionary.Remove(e.TimerOldName);
                TimersList.Remove(e.TimerOldName);
                TimerName2RowIndexDictionary.Add(e.TimerNewName, t.RowIndex);
                t.ReNameTimer(e.TimerNewName);                
                //TimersList.AddTimer(e.TimerNewName, t); this will be called in the PropertyChanged event for the timer
            }
            else
            {
                Rows.RemoveAt(e.RowIndex);
                TimerName2RowIndexDictionary.Remove(e.TimerOldName);
                TimersList.Remove(e.TimerOldName);
            }
        }

        private void Editor_QueryTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            e.Timer = TimersList[Rows[e.RowIndex].TimerCanonicalName()];
        }
    }

    [Serializable]
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
            if (split_pos < 0) return me;
            return me.Substring(0, split_pos);
        }

        // returns the remainder of a string after the index of 'criteria', criteria is a  char
        public static string SubStringAfterIndexOf(this string me, char criteria)
        {
            return me.Split(criteria)[1];
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

        public static void SetTimerElapsedTimeValue(this DataGridView me, SingleTimerLib.SingleTimerLibEventArgs e)
        {
            me.Rows[e.RowIndex].Cells[1].Value = e.ElapsedTime;
        }

        public static void SetTimerElapsedTimeValue(this DataGridViewRow me, string elapsedTime)
        {
            me.Cells[1].Value = elapsedTime;
        }

        public static void SetTimerNameValue(this DataGridViewRow me, string name)
        {
            me.Cells[0].Value = name;
        }

        public static string TimerCanonicalName(this DataGridViewRow me) => me.Cells[0].EditedFormattedValue.ToString().Trim('*');

        public static string TimerElapsedTime(this DataGridViewRow me) => me.Cells[1].ToString();

        public static List<string> ColumnNames(this DataColumnCollection me)
        {
            List<string> _list = new List<string>();
            foreach (DataColumn _dc in me)
            {
                _list.Add(_dc.ColumnName);
            }
            return _list;
        }

        public static void EnsureCanonicalTimerNames(this DataTable me)
        {
            foreach(DataRow _r in me.Rows)
            {
                _r[0] = _r.TimerCanonicalName();
                _r.AcceptChanges();
            }
        }

        public static string TimerCanonicalName(this DataRow me)
        {
            return me[0].ToString().Trim('*');
        }

        public static void MoveToSelectedItem(this BindingNavigator me, int rowIndex)
        {
            int currentIndex = Int32.Parse(me.PositionItem.Text);
            while (me.PositionItem.Text != rowIndex.ToString())
            {
                if (currentIndex < rowIndex)
                {
                    me.MoveNextItem.PerformClick();
                }
                else
                {
                    me.MovePreviousItem.PerformClick();
                }
                currentIndex = Int32.Parse(me.PositionItem.Text);
            }
        }
    }
}
