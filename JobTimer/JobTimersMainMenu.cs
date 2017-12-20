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
            InitializeDataFile();          
            jobTimersIcon.Visible = false;
            _timers = new SingleTimersCollection();
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
                //                SetupSavedTimers();               
            }
        }
        private DataTable GetSavedTimers()
        {
            Timers.ReadXml(DataFile);
            Timers.AcceptChanges();
            TimersDataGridView.Update();
            //AddTimersToTimersList();
            //AddTimersChildMenuItemsToDropDownMenu();
            DebugPrint(Timers);
            return Timers;
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
                //TimersDataGridView_CellBeginEdit(sender, new DataGridViewCellCancelEventArgs(0, (int)_cMi.Tag));
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
            DebugPrint(string.Format("Row Data [{0},{1},{2}]", row.TimerCanonicalName(), row.TimerElapsedTime(), row.TimerKey()));
        }
        private void DebugPrint(DataTable timers)
        {
            foreach (DataRow row in timers.Rows)
            {
                DebugPrint(row);
            }
        }
        private void DebugPrint(SingleTimer t)
        {
            DebugPrint(string.Format("Name: {0} ElapsedTime: {1} Running: {2}", t.CanonicalName, t.RunningElapsedTime, t.IsRunning ? "Yes" : "No"));
        }
        private void DebugPrint(string message, [CallerMemberName] string caller="")
        {
            string messageWithTimeStamp = string.Format("[{0}]\t{1} says {2}", DateTime.Now.ToString("HH:mm:ss:fff"), caller, message);
            Debug.Print(messageWithTimeStamp);
        }
        private DialogResult ShowSingleTimerEditorForm(DataGridViewCellCancelEventArgs e, out SingleTimer t, bool needNewTimer = false)
        {
            SingleTimerEditorForm editor = new SingleTimerEditorForm(e, needNewTimer);//,Editor_QueryTimerNeeded);
           // editor.QueryTimerNeeded += Editor_QueryTimerNeeded;            
            t = editor.Timer;
            return editor.ShowDialog(this);
        }

        private void SavedTimersBindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e)
        {

        }

        private void SavedTimersBindingSource_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e)
        {

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

        public static void Remove(this ToolStripDropDownMenu me, string key)
        {
            me.Items.RemoveByKey(key);
        }

        public static ToolStripMenuItem GetItemByKey(this ToolStripDropDownMenu me, string key)
        {
            return (ToolStripMenuItem)me.Items[me.Items.IndexOfKey(key)];
        }
    }
}
