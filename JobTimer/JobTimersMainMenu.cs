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
        private string DataFile { get; set; }
        private ToolStripDropDownMenu _dropDownMenu = null;
        private SingleTimerLib.SingleTimersCollection _timers = null;
        public SingleTimerLib.SingleTimersCollection TimersList
        {
            get { return _timers; }
        }
        public JobTimerForm()
        {
            InitializeComponent();
            jobTimersIcon.Visible = false;
            _timers = new SingleTimersCollection();
            _dropDownMenu = new ToolStripDropDownMenu();
            Timers.RowDeleting += Timers_RowDeleting;
            InitializeDataFile();
        }

        private void Timers_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            DebugPrint(String.Format("Row Index: {0}, Row State: {1}", e.Row.TimerKey(), e.Row.RowState.ToString()));
            // Have to remove the Timer from the Timers list as well
            DisconnectTimerEvents(TimersList[e.Row.TimerKey()]);
            TimersList[e.Row.TimerKey()].Dispose();
            TimersList.Remove(e.Row.TimerKey());            
            DebugPrint(TimersList);
            int index = TimersDataGridView.Rows.ViewIndexFromDataIndex(e.Row.TimerKey());
            activeTimersMenu.DropDown.Items.RemoveAt(index);            
        }

        private void DisconnectTimerEvents(SingleTimer t)
        {
            TimersList[t.RowIndex].NameChanging -= Timer_NameChanging;
            TimersList[t.RowIndex].ElapsedTimeChanging -= Timer_ElapsedTimeChanging;
            TimersList[t.RowIndex].TimerReset -= Timer_TimerElapsedTimeReset;
        }

        private void InitializeDataFile()
        {
            if (!Directory.Exists(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), @"Data")))
                Directory.CreateDirectory(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), @"Data"));

            FileInfo inf = new FileInfo(Application.ExecutablePath);
            if (File.Exists(Path.Combine(inf.DirectoryName, @"SavedTimers.xml")))
            {
                DebugPrint("copy datafile to data directory...");
                File.Copy(Path.Combine(inf.DirectoryName, @"SavedTimers.xml"),
                    Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), @"Data\SavedTimers.xml"), true);
            }

            DataFile = Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), @"Data\SavedTimers.xml");

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
               //
            }
        }
        private DataTable GetSavedTimers()
        {
            Timers.ReadXml(DataFile);
            Timers.AcceptChanges();
            TimersDataGridView.Update();
            AddTimersToTimersList(Timers.Rows);
            AddTimersChildMenuItemsToDropDownMenu();
            ConnectTimerEvents(TimersList.Timers);
            DebugPrint(Timers);
            DebugPrint(_timers,InfoTypes.TimerEvents);
            DebugPrint(_dropDownMenu.Items);
            return Timers;
        }

        private void ConnectTimerEvents(Dictionary<int, SingleTimer> timers)
        {
            foreach (SingleTimer t in timers.Values)
            {
                ConnectTimerEvents(t);
            }
        }

        private void ConnectTimerEvents(SingleTimer t)
        {
            TimersList[t.RowIndex].NameChanging += Timer_NameChanging;
            TimersList[t.RowIndex].ElapsedTimeChanging += Timer_ElapsedTimeChanging;
            TimersList[t.RowIndex].TimerReset += Timer_TimerElapsedTimeReset;
        }

        private void Timer_TimerElapsedTimeReset(object sender, SingleTimerLibEventArgs e)
        {
            Timers.Rows[e.Timer.RowIndex].SetTimerElapsedTime(e.Timer.RunningElapsedTime);
            Timers.AcceptChanges();
            ThredSafeUpdateChildMenuItemText((ToolStripMenuItem)_dropDownMenu.Items[e.Timer.RowIndex], e.Timer.MenuText);
        }

        private void Timer_ElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [CallerMemberName] string caller = "")
        {
            Timers.RowByRowIndex(e.Timer.RowIndex).SetTimerElapsedTime(e.ElapsedTime);
            Timers.AcceptChanges();
            ThredSafeUpdateChildMenuItemText((ToolStripMenuItem)_dropDownMenu.ItemByRowIndex(e.Timer.RowIndex), e.Timer.MenuText);
            DebugPrint(e);
        }

        private void ThredSafeUpdateChildMenuItemText(ToolStripMenuItem cMI, string updatedMenuText)
        {
            if(_dropDownMenu.InvokeRequired)
            {
                _dropDownMenu.Invoke(new Action<ToolStripMenuItem,string>(ThredSafeUpdateChildMenuItemText), cMI, updatedMenuText);
                return;
            }
            cMI.Text = updatedMenuText;
        }

        private void ThreadSafeUpdateTimerDataGridView(DataGridView timerDGV)
        {
           if(timerDGV.InvokeRequired)
           {
                timerDGV.Invoke(new Action<DataGridView>(ThreadSafeUpdateTimerDataGridView), timerDGV);
                return;
           }
            timerDGV.Update();
        }

        private void DebugPrint(SingleTimerElapsedTimeChangingEventArgs e)
        {
            string singletimerelapsedtimechangingeventargs =
                 string.Format("Timer name: {0}, Elapsed Time: {1}", e.Timer.CanonicalName, e.ElapsedTime);
            DebugPrint(singletimerelapsedtimechangingeventargs);
        }

        private void Timer_NameChanging(object sender, SingleTimerNameChangingEventArgs e, [CallerMemberName] string caller = "")
        {
            Timers.Rows[e.Timer.RowIndex].SetCanoniicalName(e.NewName);
            Timers.AcceptChanges();
            DebugPrint(e);
            DebugPrint(e.Timer, InfoTypes.TimerEvents);
        }

        private void DebugPrint(SingleTimerNameChangingEventArgs e)
        {
            string singletimernamechangingeventargs =
                string.Format("New name: {0}, Old Name: {1}", e.NewName, e.OldName);
            DebugPrint(singletimernamechangingeventargs);
        }

        private void DebugPrint(ToolStripItemCollection items)
        {
            foreach (ToolStripMenuItem childMenuItem in items)
            {
                DebugPrint(childMenuItem);
            }
        }

        private void DebugPrint(ToolStripMenuItem childMenuItem)
        {
            DebugPrint(string.Format("Menu Row Index: {0}, Menu State: {1}", childMenuItem.MergeIndex, childMenuItem.CheckState.ToString()));
        }

        private void AddTimersChildMenuItemsToDropDownMenu()
        {
            foreach(int index in TimersList.Keys)
            {
                AddTimerChildItemToDropDownMenu(CreateChildMenuItem(TimersList[index]));
            }
            activeTimersMenu.DropDown = jobTimersIcon.Visible ? _dropDownMenu : null;
        }

        private void AddTimerChildItemToDropDownMenu(ToolStripMenuItem ChildMenuItem)
        {
            _dropDownMenu.Items.Add(ChildMenuItem);
        }

        private void DebugPrint(SingleTimersCollection timerlist, InfoTypes showMe = InfoTypes.Default)
        {
            foreach(SingleTimer _t in timerlist.Values)
            {
                DebugPrint(_t, showMe);
            }
        }

        private void AddTimersToTimersList(DataRowCollection rows)
        {
            foreach(DataRow row in rows)
            {
                AddTimerToTimersList(row);
            }
        }

        private void AddTimerToTimersList(DataRow row)
        {
            if (!_timers.ContainsKey(row.TimerKey()))
            {
                _timers.AddTimer(row.TimerKey(), row.TimerCanonicalName(), row.TimerElapsedTime());
            }
        }

        private void WriteDataToXml()
        {
            Timers.EnsureCanonicalTimerNames();
            Timers.AcceptChanges();
            Timers.WriteXml(DataFile);
            string message = string.Format("Writing {0} rows to database: '{1}'", Timers.Rows.Count, DataFile);
            DebugPrint(message);
            foreach (DataRow _r in Timers.Rows)
            {
                message = string.Format("Timer Name {0} is at {1} of elapsed time!", _r[0].ToString(), _r[1].ToString());
                DebugPrint(message);
            }

            /*FileInfo inf = new FileInfo(DataFile);
            FileInfo appInf = new FileInfo(Application.ExecutablePath);
            
            if(AnswerYesNo("Save current data file from data directory?\n(will overwrite existing)","Save Data File!") == DialogResult.Yes)
                File.Copy(DataFile, Path.Combine(appInf.DirectoryName, inf.Name), true);*/
        }
        private ToolStripMenuItem CreateChildMenuItem(SingleTimer t)
        {
            Debug.Assert(t != null);
            Image img = activeTimersMenu.Image;
            ToolStripMenuItem cMI = new ToolStripMenuItem(img)
            {
                Name = t.CanonicalName,
                Text = t.MenuText,
                Tag = t.RowIndex
            };
            cMI.MouseDown += ChildMenuItem_MouseDown;
            cMI.DropDownItems.Add(CreateChildMenuDeleteItem(bindingNavigatorDeleteItem.Image));
            cMI.DropDownItems[0].Tag = cMI;
            return cMI;
        }

        private ToolStripMenuItem CreateChildMenuDeleteItem(Image image)
        {
            ToolStripMenuItem cMI = new ToolStripMenuItem("Delete_Timer", image, Delete_TimerItem_Click)
            {
                Name = Text
            };

            cMI.Click += BindingNavigatorDeleteItem_Click;
            return cMI;
        }

        private void ChildMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem _cMi = (ToolStripMenuItem)sender;
            if (_cMi == null) return;

            if(e.Button == MouseButtons.Left)
            {
                SingleTimer _t = TimersList[(int)_cMi.Tag];
                _t.StartOrStop();

                return;
            }

            if(e.Button == MouseButtons.Right)
            {
                TimersDataGridView_CellBeginEdit(sender, new DataGridViewCellCancelEventArgs(0, (int)_cMi.Tag));
            }
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
            Properties.Settings.Default.Save();
            List<int> listOfKeys = new List<int>();
            foreach (int _k in TimersList.Keys)
            {
                listOfKeys.Add(_k);
            }
            foreach (int timerRow in listOfKeys)
            {
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
        private void JobTimerForm_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            Text += "-[Active]";
            DebugPrint(TimersDataGridView.Rows);
        }
        private void JobTimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            activeTimersMenu.DropDown = _dropDownMenu;
            TimersList.PreserveTimers = true;
            jobTimersContextMenu.Text = Text;
            jobTimersIcon.Text = Text;
            jobTimersIcon.BalloonTipText = Text;
            TimersDataGridView.Update();
            ShowInTaskbar = false;
            Visible = false;
            jobTimersIcon.Icon = Icon;
            jobTimersIcon.Visible = true;
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
            activeTimersMenu.DropDown = null;
            jobTimersIcon.Visible = false;
        }
        private void QuitContextMenuItem_Click(object sender, EventArgs e)
        {
            DoClose();
        }
        private void DebugPrint(DataRow row)
        {
            DebugPrint(string.Format("Row Index: {0}, Row State: {1}", row.TimerKey(), row.RowState.ToString()));
        }

        private void DebugPrint(DataTable timers)
        {
            foreach (DataRow row in timers.Rows)
            {
                DebugPrint(row);
            }
        }
        private void DebugPrint(SingleTimer t, InfoTypes showMe = InfoTypes.Default)
        {
            switch (showMe)
            {
                case InfoTypes.TimerEvents:
                    {
                        DebugPrint(string.Format("Name  = {0}", nameof(t.ElapsedTimeChanging)));
                        try
                        {
                            foreach (Delegate @d in t.ElapsedTimeChangingInvocationList)
                            {
                                DebugPrint(string.Format("Value = {0}.{1}", d.GetMethodInfo().ReflectedType.Name, d.GetMethodInfo().Name));
                            }
                        }
                        catch (Exception)
                        {
                            DebugPrint(string.Format("Value = {0}", "Not Set"));
                        }

                        DebugPrint(string.Format("Name  = {0}", nameof(t.NameChanging)));
                        try
                        {
                            foreach (Delegate @d in t.NameChangingInvocationList)
                            {
                                DebugPrint(string.Format("Value = {0}.{1}", d.GetMethodInfo().ReflectedType.Name, d.GetMethodInfo().Name));
                            }
                        }
                        catch (Exception)
                        {
                            DebugPrint(string.Format("Value = {0}", "Not Set"));
                        }    

                        DebugPrint(string.Format("Name  = {0}", nameof(t.TimerReset)));
                        try
                        {
                            foreach (Delegate @d in t.TimerResetInvocationList)
                            {
                                DebugPrint(string.Format("Value = {0}.{1}", d.GetMethodInfo().ReflectedType.Name, d.GetMethodInfo().Name));
                            }
                        }
                        catch (NullReferenceException)
                        {
                            DebugPrint(string.Format("Value = {0}", "Not Set"));
                        }
                        break;
                    }
                default:
                    {
                        DebugPrint(string.Format("Timer Row Index: {0}, Timer State: {1}", t.RowIndex, t.TimerState.ToString()));
                        break;
                    }
            }
        }

        private void DebugPrint(string message, [CallerMemberName] string caller="")
        {
            string messageWithTimeStamp = string.Format("[{0}]\t{1} says {2}", DateTime.Now.ToString("HH:mm:ss:fff"), caller, message);
            Debug.Print(messageWithTimeStamp);
        }

        private DialogResult ShowSingleTimerEditorForm(DataGridViewCellCancelEventArgs e, out SingleTimer t, bool needNewTimer = false)
        {
            SingleTimerEditorForm editor = new SingleTimerEditorForm(e, needNewTimer, Editor_QueryTimerNeeded);//,Editor_QueryTimerNeeded);
            editor.QueryTimerNeeded += Editor_QueryTimerNeeded;
            editor.RequestStartTimer += Editor_RequestStartTimer;
            editor.RequestStopTimer += Editor_RequestStopTimer;
            t = editor.Timer;
            return editor.ShowDialog(this);
        }

        private void Editor_RequestStopTimer(object sender, SingleTimerEditorFormStopTimerEventArgs e)
        {
            DebugPrint(TimersList[e.RowIndex], InfoTypes.TimerEvents);
            TimersList[e.RowIndex].StopTimer();
        }

        private void Editor_RequestStartTimer(object sender, SingleTimerEditorFormStartTimerEventArgs e)
        {
            DebugPrint(TimersList[e.RowIndex],InfoTypes.TimerEvents);
            TimersList[e.RowIndex].StartTimer();
        }

        private void Editor_QueryTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            int dataRowIndex = TimersDataGridView.Rows[e.RowIndex].DataRowIndex();
            Debug.Assert(dataRowIndex >= 0);
            e.Timer = TimersList[dataRowIndex];
        }

        private void SavedTimersBindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e)
        {

        }

        private void SavedTimersBindingSource_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e)
        {

        }
        private void Delete_TimerItem_Click(object sender, EventArgs e)
        {
            if (sender.ToString() == "Delete_Timer")
            {
                int index = ((ToolStripMenuItem)sender).OwnerItem.Tag.ToString().ToInt();
                Timers.Rows.RemoveAt(Timers.RowByRowIndex(index).TimerKey());
            }
        }

        private void BindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DebugPrint(string.Format("Object is {0}", sender.ToString()));        
            Timers.AcceptChanges();
            //AddTimersChildMenuItemsToDropDownMenu();
            DebugPrint(Timers.Rows);
            DebugPrint(TimersDataGridView.Rows);
            DebugPrint(_dropDownMenu.Items);
            DebugPrint(sender.GetType().ToString());            
        }

        private void DebugPrint(DataGridViewRowCollection rows)
        {
            foreach (DataGridViewRow row in rows)
            {
                DebugPrint(row);
            }
        }

        private void DebugPrint(DataGridViewRow row)
        {
            DebugPrint(string.Format("View Row Index: {0}, View Row State: {1}", row.Index, row.State.ToString()));
        }

        private void TimersDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (ShowSingleTimerEditorForm(e,out SingleTimer t, false) == DialogResult.OK)
            {
                DebugPrint(TimersList[DataRowIndex(e.RowIndex)], InfoTypes.TimerEvents);
                Timers.Rows[DataRowIndex(e.RowIndex)].SetCanoniicalName(t.CanonicalName);
                Timers.Rows[DataRowIndex(e.RowIndex)].SetTimerElapsedTime(t.RunningElapsedTime);               
            }
            e.Cancel = true;
        }

        private int DataRowIndex(int rowIndex)
        {
            return TimersDataGridView.Rows[rowIndex].DataRowIndex();
        }

        private void DebugPrint(DataRowCollection rows)
        {
            foreach(DataRow row in rows)
            {
                DebugPrint(row);
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

        public static void SetTimerElapsedTimeValue(this DataGridView me, SingleTimerLib.SingleTimerLibEventArgs e) => me.Rows[e.Timer.RowIndex].Cells[1].Value = e.Timer.RunningElapsedTime;

        public static void SetTimerElapsedTimeValue(this DataGridViewRow me, string elapsedTime) => me.Cells[1].Value = elapsedTime;

        public static void SetTimerNameValue(this DataGridViewRow me, string name) => me.Cells[0].Value = name;

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
                switch (_r.RowState)
                {
                    case DataRowState.Added:
                        {
                            _r[0] = _r.TimerCanonicalName();
                            _r.AcceptChanges();
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public static string TimerCanonicalName(this DataRow me)
        {
            return me[0].ToString().Trim('*');
        }

        public static void SetCanoniicalName(this DataRow me, string newName)
        {
            me[0] = newName;
            me.AcceptChanges();
        }

        public static void SetTimerElapsedTime(this DataRow me, string elapsedTime)
        {
            me[1] = elapsedTime;
            me.AcceptChanges();
        }

        public static string TimerElapsedTime(this DataRow me)
        {
            return me[1].ToString();
        }

        public static int TimerKey(this DataRow me)
        {
            return me[2].ToString().ToInt();
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

        public static int DataRowIndex(this DataGridViewRow row)
        {
            return Int32.Parse(row.Cells[2].EditedFormattedValue.ToString());
        }

        public static void Remove(this ToolStripDropDownMenu me, string key)
        {
            me.Items.RemoveByKey(key);
        }

        public static int ViewIndexFromDataIndex(this DataGridViewRowCollection me, int dataIndex)
        {
            foreach(DataGridViewRow row in me)
            {
                if(row.DataRowIndex() == dataIndex)
                {
                    return row.Index;
                }
            }
            return -1;
        }

        public static ToolStripMenuItem GetItemByKey(this ToolStripDropDownMenu me, string key)
        {
            return (ToolStripMenuItem)me.Items[me.Items.IndexOfKey(key)];
        }

        public static DataRow RowByRowIndex(this DataTable me, int rowIndex)
        {
            foreach(DataRow row in me.Rows)
            {
                if (row.RowState == DataRowState.Unchanged)
                {
                    if (row.TimerKey() == rowIndex)
                    {
                        return row;
                    }
                }
            }
            return null;
        }

        public static ToolStripMenuItem ItemByRowIndex(this ToolStripDropDownMenu me, int rowIndex)
        {
            foreach (ToolStripMenuItem cMI in me.Items)
            {
                if (cMI.Tag.TimerKey() == rowIndex)
                {
                    return cMI;
                }
            }
            return null;
        }

        public static int TimerKey(this object me)
        {
            return me.ToString().ToInt();
        }
    }
}
