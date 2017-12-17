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
                SetupSavedTimers();               
            }
            jobTimersIcon.Visible = false;
        }

        private void SetupSavedTimers()
        {
            Application.DoEvents();
            timersDataGridView.Update();
            for(int idx = 0; idx < Timers.Rows.Count; ++idx)
            {
                DataRow _r = Timers.Rows[idx];

                if (!TimerName2RowIndexDictionary.Contains(_r[0].ToString()))
                {
                    TimerName2RowIndexDictionary.Add(idx, _r[0].ToString());
                }
                SingleTimerLib.SingleTimer _t = TimersList.AddTimer(idx, new SingleTimerLib.SingleTimer(idx, _r[0].ToString(), _r[1].ToString()));
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
            DebugPrint(string.Format("Timer {0} is about to be disposed!", _t.CanonicalName));
            _t.Dispose();
        }

        private void JobTimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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

                int index = -1;
                try
                {
                    index = GetRowIndex(timerName);
                    Debug.Assert(index >= 0);
                    TimerName2RowIndexDictionary.Add(index, timerName);
                }
                catch (ArgumentException ex)
                {
                    DebugPrint(ex.Message);
                    DebugPrint("Name already exists!");
                    DebugPrint("Removing row from database! No Duplicate timer names allowed!");
                    return null;
                }

                SingleTimerLib.SingleTimer st = TimersList.AddTimer(index, new SingleTimerLib.SingleTimer(index, timerName, "00:00:00"));

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
            DebugPrint(string.Format("Name: {0}, Elapsed: {1}, RowIndex: {2}", e.Timer.CanonicalName, e.Timer.RunningElapsedTime, e.Timer.RowIndex));          
        }

        private void SingleTimer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SingleTimerLib.SingleTimer _t = (SingleTimerLib.SingleTimer)sender;
            if (sender == null) return;
            switch (e.PropertyName)
            {
                case nameof(SingleTimerLib.SingleTimer.RunningElapsedTime):                    
                    ThreadSafeUpdateTimerElapsedTime(_t.RowIndex);                   
                    DebugPrint(string.Format("From {1}: [{2}] Elapsed {0}", _t.RunningElapsedTime, "SingleTimer_PropertyChanged", e.PropertyName));
                    break;
                case nameof(SingleTimerLib.SingleTimer.RowIndex):
                    break;
                case nameof(SingleTimerLib.SingleTimer.Name):
                    UpdateTimerName(_t);
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
            string TimerOldName = Rows[_t.RowIndex].TimerCanonicalName();
            // remove the old key and value
            TimerName2RowIndexDictionary.Remove(_t.RowIndex);
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
            if (rowIndex > Rows.Count) return;
            DebugPrint("[Update Timer Elapsed Time]=>Running on GUI Thread!");
            SingleTimer t = TimersList[rowIndex];
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
            Application.DoEvents();
            ShowInTaskbar = true;
            Visible = true;
            jobTimersIcon.Visible = false;           
            Application.DoEvents();
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
            foreach (ToolStripMenuItem item in activeTimersMenu.DropDownItems)
            {
                string name = item.Text.SubStringByIndexOf("-[");
                using (SingleTimerLib.SingleTimer _t = TimersList[GetRowIndex(name)])
                {
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
            e.Timer = TimersList[e.RowIndex];
        }

        private void TimersDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            List<int> keys = new List<int>();
            foreach(int key in TimerName2RowIndexDictionary.Keys)
            {
                keys.Add(key);
            }
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
