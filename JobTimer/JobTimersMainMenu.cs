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
        private static bool _isClosing = false;
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

        private Dictionary<int,ToolStripMenuItem> _dropDownItems = null;
        public Dictionary<int,ToolStripMenuItem> DropDownItems { get => _dropDownItems; }

        private SingleTimerLib.SingleTimersCollection _timers = null;

        public SingleTimerLib.SingleTimersCollection TimersList
        {
            get { return _timers; }
        }

        public JobTimerForm()
        {
            InitializeComponent();

            _timers = new SingleTimerLib.SingleTimersCollection();
            _dropDownItems = new Dictionary<int, ToolStripMenuItem>();

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
                SetupSavedTimers();               
            }
            jobTimersIcon.Visible = false;
        }

        private void SetupSavedTimers(bool setDeleteHandler = true)
        {
            if (_isClosing) return;
            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("JobTimer.appLogo.bmp");

            timersDataGridView.Update();

            if(setDeleteHandler) Timers.RowDeleting += Timers_RowDeleting;

            for (int idx = 0; idx < Timers.Rows.Count; ++idx)
            {
                DataRow _r = Timers.Rows[idx];
                if (!TimerName2RowIndexDictionary.Contains(_r[0].ToString()))
                {
                    TimerName2RowIndexDictionary.Add(idx, _r.TimerCanonicalName());
                }
                SingleTimerLib.SingleTimer _t = TimersList.AddTimer(idx, new SingleTimerLib.SingleTimer(idx, _r.TimerCanonicalName(), _r.TimerElapsedTime()));
                _t.PropertyChanged += SingleTimer_PropertyChanged;
                _t.SingleTimerChanged += SingleTimer_SingleTimerChanged;
                _t.TimerReset += SingleTimer_OnTimerReset;
                ToolStripMenuItem childMenuItem = new ToolStripMenuItem(_t.MenuText, Image.FromStream(_imageStream), DropDownMenuItem_Clicked, _t.CanonicalName);
                childMenuItem.MouseDown += ChildMenuItem_MouseDown;
                childMenuItem.Tag = _t.RowIndex;
                UpDateColors(childMenuItem, _t.IsRunning);
                DropDownItems.Add(_t.RowIndex,childMenuItem);
            }
        }

        private void ChildMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                ThreadSafeBeginEdit(new DataGridViewCellCancelEventArgs(0, GetRowIndex(((ToolStripMenuItem)sender).Name)));
            }           
        }

        private void UpDateColors(ToolStripMenuItem childMenuItem, bool isRunning)
        {
            childMenuItem.ForeColor = GetForeGroundColor(isRunning);
            childMenuItem.BackColor = GetBackGroundColor(isRunning);
        }

        private static Color GetForeGroundColor(bool isRunning)
        {
            return isRunning ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaptionText;
        }

        private static Color GetBackGroundColor(bool isRunning)
        {
            return isRunning ? SystemColors.ActiveBorder : SystemColors.InactiveBorder;
        }

        private void DropDownMenuItem_Clicked(object sender, EventArgs e)
        {
            int index = GetRowIndex(((ToolStripMenuItem)sender).Name);
            if(TimersList[index].IsRunning)
            {
                TimersList[index].StopTimer();
            }
            else
            {
                TimersList[index].StartTimer();
            }
            UpDateColors((ToolStripMenuItem)sender, TimersList[index].IsRunning);
        }
        private DataTable GetSavedTimers()
        {
            Timers.ReadXml(DataFile);
            Timers.AcceptChanges();
            timersDataGridView.Update();
            return Timers;
        }

        private Dictionary<int, string> _timerName2RowIndexDictionary = new Dictionary<int, string>();

        private Dictionary<int,string> TimerName2RowIndexDictionary
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

            List<int> listOfKeys = new List<int>();
            foreach (int _k in TimersList.Keys)
                listOfKeys.Add(_k);
            foreach(int timerRow in listOfKeys)
            {
                CleanUpTimer(timerRow);
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

        private void CleanUpTimer(int rowIndex)
        {
            SingleTimerLib.SingleTimer _t = TimersList[rowIndex];
            _t.PropertyChanged -= SingleTimer_PropertyChanged;
            _t.SingleTimerChanged -= SingleTimer_SingleTimerChanged;
            _t.TimerReset -= SingleTimer_OnTimerReset;
            _t.StopTimer();
            TimersList.Remove(_t.RowIndex);
            TimerName2RowIndexDictionary.Remove(_t.RowIndex);
            DropDownItems.Remove(_t.RowIndex);
            DebugPrint(string.Format("Timer {0} is about to be disposed!", _t.CanonicalName));
            _t.Dispose();
        }

        private void JobTimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            TimersList.PreserveTimers = true;
            jobTimersContextMenu.Text = Text;
            jobTimersIcon.Text = Text;
            jobTimersIcon.BalloonTipText = Text;
            timersDataGridView.DataSource = null;
            timersDataGridView.Update();
            ShowInTaskbar = false;
            Visible = false;
            jobTimersIcon.Icon = Icon;
            jobTimersIcon.Visible = true;
        }

        private ToolStripMenuItem[] MakeArrayOf(Dictionary<int, ToolStripMenuItem>.ValueCollection values)
        {
            List<ToolStripMenuItem> _list = new List<ToolStripMenuItem>(values.Count);           
            foreach(ToolStripMenuItem item in values)
            {
                _list.Add(item);
            }
            _list.Reverse();
            return _list.ToArray();
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
                SingleTimerEditorForm editor = new SingleTimerEditorForm(timersDataGridView.NewRowIndex,true);
                editor.QueryTimerNeeded += Editor_QueryTimerNeeded;
                editor.ShowDialog(this);

                string timerName = editor.Timer.Name;//GetTimerName("Enter name for new timer:","New Timer");
                if (timerName == "Cancel")
                {
                    editor.Timer.Dispose();
                    editor.Dispose();
                    DebugPrint(string.Format("TimersList Count={0}", TimersList.Count));
                    DebugPrint(string.Format("RowIndex Count={0}", TimerName2RowIndexDictionary.Count));
                    DebugPrint(string.Format("Timers RowCount={0}", Timers.Rows.Count));
                    return null;
                }

                DataRow _r = null;
                try
                {
                    _r = Timers.Rows.Add(timerName, "00:00:00");
                    DebugPrint(string.Format("Database Row Added, RowCount={0}", Timers.Rows.Count));
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

                int index = -1;
                try
                {
                    index = GetRowIndex(timerName);
                    Debug.Assert(index >= 0);
                    if (!TimerName2RowIndexDictionary.ContainsValue(timerName) && !TimerName2RowIndexDictionary.ContainsKey(index))
                    {
                        TimerName2RowIndexDictionary.Add(index, timerName);
                        DebugPrint(string.Format("TimerName and index added to Index Dictionary [{0},{1}]!", index, timerName));
                    }
                }
                catch (ArgumentException ex)
                {
                    DebugPrint(ex.Message);
                    DebugPrint("Name already exists!");
                    DebugPrint("Removing row from database! No Duplicate timer names allowed!");
                    return null;
                }

                SingleTimerLib.SingleTimer st = null;
                if (!TimersList.ContainsKey(index))
                {
                    st = TimersList.AddTimer(index, new SingleTimerLib.SingleTimer(index, timerName, "00:00:00"));
                }
                else
                {
                    st = TimersList[index];
                }

                st.TimerReset += SingleTimer_OnTimerReset;
                st.PropertyChanged += SingleTimer_PropertyChanged;
                st.SingleTimerChanged += SingleTimer_SingleTimerChanged;
                return st;
            }
        }

        private int GetRowIndex(string timerName)
        {
            foreach(DataRow dr in Timers.Rows)
            {
                if (dr.TimerCanonicalName() == timerName)
                    return Int32.Parse(dr["key"].ToString());
            }
            return -1;
        }

        private void SingleTimer_SingleTimerChanged(object sender, SingleTimerLib.SingleTimerLibEventArgs e)
        {
            ThreadSafeUpdateTimerElapsedTime(e);            
            DebugPrint(string.Format("Name: {0}, Elapsed: {1}, RowIndex: {2}", e.Timer.CanonicalName, e.Timer.RunningElapsedTime, e.Timer.RowIndex));          
        }

        private void SingleTimer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SingleTimerLib.SingleTimer _t = (SingleTimerLib.SingleTimer)sender;
            if (sender == null) return;
            switch (e.PropertyName)
            {
                case nameof(_t.RunningElapsedTime):                    
                    ThreadSafeUpdateTimerElapsedTime(_t.RowIndex);                   
                    DebugPrint(string.Format("From {1}: [{2}] Elapsed {0}", _t.RunningElapsedTime, "SingleTimer_PropertyChanged", e.PropertyName));
                    break;
                case nameof(_t.RowIndex):
                    DebugPrint(String.Format("Timer '{0}' has new row index: {1}", _t.CanonicalName, _t.RowIndex));
                    TimerName2RowIndexDictionary.Add(_t.RowIndex, _t.CanonicalName);
                    TimersList.AddTimer(_t.RowIndex, _t);
                    break;
                case nameof(_t.Name):
                    UpdateTimerName(_t);
                    break;
                case nameof(_t.IsRunning):
                    string message1 = "{0}: {1} is running! [{2}]";
                    string message2 = "{0}: {1} is stopped! [{2}]";
                    DebugPrint(string.Format(_t.IsRunning ? message1 : message2, "SingleTimer_PropertyChanged", _t.Name, e.PropertyName));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Updates the name of the timer.
        /// </summary>
        /// <param name="_t">The timer object to test.</param>
        /// <param name="caller">Decorated with the 'CallerMemberName' attribute, will contain the name
        /// of the calling method, or the name specified optionally.
        /// </param>
        private void UpdateTimerName(SingleTimer _t, [CallerMemberName] string caller = "")
        {
            DebugPrint(string.Format("{0}: {1} is a new name!", caller, _t.Name));
            string TimerOldName = string.Empty;
            if (timersDataGridView.DataSource == null)
            {
                TimerOldName = DropDownItems[_t.RowIndex].Name;
            }
            else
            {
                TimerOldName = Rows[_t.RowIndex].TimerCanonicalName();
            }
            // remove the old key and value
            TimerName2RowIndexDictionary.Remove(_t.RowIndex);
            ToolStripMenuItem childItem = DropDownItems[_t.RowIndex];
            DropDownItems.Remove(_t.RowIndex);
            childItem.Name = _t.CanonicalName;
            childItem.Text = _t.MenuText;
            childItem.Tag = _t.RowIndex;
            UpDateColors(childItem, _t.IsRunning);
            DropDownItems.Add(_t.RowIndex,childItem);
            // remove timer with old name           
            TimersList.Remove(_t.RowIndex);
            // add timer new name with index
            TimerName2RowIndexDictionary.Add(_t.RowIndex, _t.CanonicalName);
            Debug.Assert(TimerName2RowIndexDictionary.Contains(_t.CanonicalName));
            // add new timer name with updated value
            TimersList.AddTimer(_t.RowIndex, _t);            
            // update datagridview row with new timer name
            ThreadSafeUpdateTimerName(_t);
        }

        private void ThreadSafeUpdateTimerElapsedTime(SingleTimerLibEventArgs e) => ThreadSafeUpdateTimerElapsedTime(e.Timer.RowIndex);

        /// <summary>
        /// Thread safe method to update timer elapsed time.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        private void ThreadSafeUpdateTimerElapsedTime(int rowIndex)
        {
            if (timersDataGridView.InvokeRequired)
            {
                DebugPrint("[Update Timer Elapsed Time]=>Running on non GUI Thread=>calling method on GUI Thread!");
                timersDataGridView.Invoke(new Action<int>(ThreadSafeUpdateTimerElapsedTime), rowIndex);
                return;
            }
            DebugPrint("=>Running on GUI Thread!");
            SingleTimer t = TimersList[rowIndex];
            DebugPrint("=>Update Timer Elapsed Time!");
            DropDownItems[t.RowIndex].Text = t.MenuText;
            if(timersDataGridView.DataSource == null) // main window is not visible
            {
                if (activeTimersMenu.DropDownItems.Count > 0)
                {
                    activeTimersMenu.DropDownItems[t.RowIndex].Text = t.MenuText;
                }
                Application.DoEvents();
                return;
            }
            Rows[t.RowIndex].Cells[1].Value = t.RunningElapsedTime;
            Rows[t.RowIndex].Cells[0].Value = t.Name;
            Application.DoEvents();
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
            if (timersDataGridView.DataSource == null) return;
            Rows[t.RowIndex].Cells[0].Value = t.Name;
            Timers.AcceptChanges();
        }

        private void SingleTimer_OnTimerReset(object sender, SingleTimerLib.SingleTimerLibEventArgs e)
        {
            ThreadSafeUpdateTimerElapsedTime(e);
            DebugPrint(string.Format("Name {0}, Elapsed {1}, RowIndex{2}", e.Timer.CanonicalName, e.Timer.RunningElapsedTime, e.Timer.RowIndex));
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
                    using (SingleTimerLib.SingleTimer _t = TimersList[index])
                    {
                        _t.PropertyChanged -= SingleTimer_PropertyChanged;
                        _t.SingleTimerChanged -= SingleTimer_SingleTimerChanged;
                        _t.TimerReset -= SingleTimer_OnTimerReset;
                    }
                    TimersList.Remove(index);
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
            ShowInTaskbar = true;
            Visible = true;
            timersDataGridView.DataSource = SavedTimersBindingSource;
            timersDataGridView.Update();
            activeTimersMenu.DropDownItems.Clear();
            jobTimersIcon.Visible = false;
            timersDataGridView.Update();
        }

        private void RemoveBlankRowsFromView()
        {
            DebugPrint(string.Format("Data Rows: {0}", Timers.Rows.Count));
            DebugPrint(string.Format("DataGridView Rows {0}", Rows.Count));
            if (Timers.Rows.Count != Rows.Count)
            {
                DebugPrint("----Remove Blank Rows");
                List<int> toDelete = new List<int>();
                foreach (DataGridViewRow r in Rows)
                {
                    if (r.Index >= Timers.Rows.Count)
                        toDelete.Add(r.Index);
                }

                foreach (int idx in toDelete)
                {
                    Rows.Remove(Rows[idx]);
                }

                timersDataGridView.Update();
                DebugPrint(string.Format("Data Rows: {0}", Timers.Rows.Count));
                DebugPrint(string.Format("DataGridView Rows {0}", Rows.Count));
            }

        }

        private void QuitContextMenuItem_Click(object sender, EventArgs e)
        {
            DoClose();
        }

        private void DebugPrint(string message, [CallerMemberName] string caller="")
        {
            string messageWithTimeStamp = string.Format("[{0}]\t{1} says {2}", DateTime.Now.ToString("HH:mm:ss:fff"), caller, message);
            Debug.Print(messageWithTimeStamp);
        }

        private void JobTimersContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void EditTimerFromNotificationMenu(ToolStripMenuItem childItem, string shortName, int rowIndex)
        {
            timersDataGridView.CurrentCell = timersDataGridView[0, rowIndex];
            timersDataGridView.BeginEdit(false);
        }

        private void QueryUserResetTimer(int rowIndex)
        {
            string name = Rows[rowIndex].TimerCanonicalName();
            if (MessageBox.Show(this, string.Format("Reset Timer '{0}'", name), name, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SingleTimerLib.SingleTimer _t = TimersList[rowIndex])
                {
                    if (_t != null)
                    {
                        _t.ResetTimer();
                    }
                    else
                    {
                        ThreadSafeUpdateGridViewRowElapsedTimerValue(rowIndex, "00:00:00");
                    }
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
                    if (AnswerYesNo(message, caption) == DialogResult.Yes)
                    {
                        dr.SetTimerElapsedTimeValue("00:00:00");
                    }
                }
            }
        }

        private void ActiveTimersMenu_DropDownOpening(object sender, EventArgs e)
        {
            activeTimersMenu.DropDownItems.AddRange(MakeArrayOf(DropDownItems.Values));
            foreach(ToolStripMenuItem childMenuItem in activeTimersMenu.DropDownItems)
            {
                UpDateColors(childMenuItem, TimersList[childMenuItem.Tag.ToString().ToInt()].IsRunning);
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

            using (SingleTimerLib.SingleTimer _t = TimersList[rowIndex])
            {
                if (_t != null)
                {
                    TimerName2RowIndexDictionary.Remove(rowIndex);
                    TimersList.Remove(rowIndex);
                    TimerName2RowIndexDictionary.Add(rowIndex, newname);
                    _t.ReNameTimer(newname);
                }
                else
                {
                    Rows.RemoveAt(rowIndex);
                    TimerName2RowIndexDictionary.Remove(rowIndex);
                    TimersList.Remove(rowIndex);
                }
            }
        }
        
        private void TimersDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) => DebugPrint(e.Exception.Message);

        private void QueryStartOrStopTimer(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                if (AnswerYesNo("Create new timer?", "New Timer Inquiry") == DialogResult.Yes)
                {
                    using (SingleTimer _t = StartNewTimer)
                    {}
                }
            }
            else
            {
                using (DataGridViewRow _r = (DataGridViewRow)Rows[e.RowIndex])
                {
                    string name = _r.TimerCanonicalName();

                    if (TimersList[e.RowIndex].IsRunning)
                    {
                        TimersList[e.RowIndex].StopTimer();
                        DebugPrint(string.Format("{0}: {1} is no longer running!", "timersDataGridView_RowHeaderMouseClick", name));
                        return;
                    }

                    if (!TimersList[e.RowIndex].IsRunning)
                    {
                        TimersList[e.RowIndex].StartTimer();
                        DebugPrint(string.Format("{0}: {1} is now running!", "timersDataGridView_RowHeaderMouseClick", name));
                        return;
                    }
                }
            }
        }

        private void BindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            AddNewTimer();
        }

        private void ThreadSafeAddNewTimer()
        {
            if(timersDataGridView.InvokeRequired)
            {
                timersDataGridView.Invoke(new Action(ThreadSafeAddNewTimer));
                return;
            }
            AddNewTimer();
        }

        private SingleTimer AddNewTimer()
        {
            DebugPrint("bindingNavigatorAddNewItem_Click: Adding a Row to DataGridView!");           
            return StartNewTimer;
        }

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
                string name = _dr.TimerCanonicalName();
                using (SingleTimerLib.SingleTimer _t = TimersList[_dr.Index])
                {
                    _t.SingleTimerChanged -= SingleTimer_SingleTimerChanged;
                    _t.PropertyChanged -= SingleTimer_PropertyChanged;
                    _t.TimerReset -= SingleTimer_OnTimerReset;
                    _t.StopTimer();
                    Debug.Assert(_t.CanonicalName == Timers.Rows[_t.RowIndex].TimerCanonicalName());
                    Timers.Rows.RemoveAt(_t.RowIndex);
                    Timers.AcceptChanges();
                    ThreadSafeUpdateDataGridView();
                    TimerName2RowIndexDictionary.Remove(_t.RowIndex);
                    TimersList.Remove(_t.RowIndex);                 
                }
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
            ThreadSafeBeginEdit(e);
            e.Cancel = true;
        }

        private void ThreadSafeBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            if(timersDataGridView.InvokeRequired)
            {
                timersDataGridView.Invoke(new Action<DataGridViewCellCancelEventArgs>(ThreadSafeBeginEdit), e);
                return;
            }

            SingleTimerEditorForm editor = new SingleTimerEditorForm(e.RowIndex);
            editor.QueryTimerNeeded += Editor_QueryTimerNeeded;
            editor.ShowDialog(this);
        }

        private void Editor_QueryTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            if(e.NewTimerNeeded)
            {
                e.Timer = new SingleTimer(e.RowIndex,string.Format("{0}","Cancel"));
                return;
            }
            e.Timer = TimersList[e.RowIndex];
        }

        private void Timers_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            int index = GetRowIndex(e.Row.TimerCanonicalName());
            DebugPrint(string.Format("Data Row being deleted: index = {0}", index));
            if (!TimersList.PreserveTimers)
            {
                ClearTimerEvents(index);
                TimersList[index].StopTimer();
                TimersList[index].Dispose();
                TimersList.Remove(index);
            }
        }

        private void ClearTimerEvents(int rowIndex)
        {
            TimersList[rowIndex].PropertyChanged -= SingleTimer_PropertyChanged;
            TimersList[rowIndex].SingleTimerChanged -= SingleTimer_SingleTimerChanged;
            TimersList[rowIndex].TimerReset -= SingleTimer_OnTimerReset;
        }

        private void TimersDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Timers.AcceptChanges();
            SynchronizeIndicies();
        }

        private void SynchronizeIndicies()
        {
            List<string> runningTimers = new List<string>();
            foreach(SingleTimer _t in TimersList.Values)
            {
                if(_t.IsRunning)
                {
                    runningTimers.Add(_t.CanonicalName);
                }
                DropDownItems[_t.RowIndex].Dispose();
                DropDownItems.Remove(_t.RowIndex);
                ClearTimerEvents(_t.RowIndex);
                _t.StopTimer();
                _t.Dispose();
            }

            TimerName2RowIndexDictionary.Clear();
            TimersList.Clear();

            if(Timers.Rows.Count > 0) SetupSavedTimers(false);

            foreach(string name in runningTimers)
            {
                foreach (SingleTimer t in TimersList.Values)
                {
                    if(t.CanonicalName == name)
                    {
                        t.StartTimer();
                    }
                }
            }
        }

        private void timersDataGridView_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            DebugPrint(string.Format("Error reported from DataGridView Control:\nRow {0} Column{1}\nMessage:\n{2}", e.RowIndex, e.ColumnIndex, e.ThrowException.ToString()));
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
            me.Rows[e.Timer.RowIndex].Cells[1].Value = e.Timer.RunningElapsedTime;
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

        public static string TimerElapsedTime(this DataRow me)
        {
            return me[1].ToString();
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

        public static ToolStripMenuItem[] FindItemThatStartsWith(this ToolStripItemCollection me, string startsWith)
        {
            List<ToolStripMenuItem> startedWith = new List<ToolStripMenuItem>();
            foreach(ToolStripMenuItem chldItem in me)
            {
                if(chldItem.Text.StartsWith(startsWith))
                { startedWith.Add(chldItem); }
            }
            return startedWith.ToArray();
        }

        /// <summary>
        /// Extention for a Dictionary of int, string, determines whether the dictionary contains the specified search string.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="searchString">The search string.</param>
        /// <returns>
        ///   <c>true</c> if the dictionar contains the specified search string; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this Dictionary<int,string> me, string searchString)
        {
            foreach(string term in me.Values)
            {
                if (term.Equals(searchString,StringComparison.InvariantCulture))
                    return true;
            }
            return false;
        }
    }
}
