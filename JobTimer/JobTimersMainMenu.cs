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

        private ToolStripDropDownMenu _dropDownMenu = null;

        private SingleTimerLib.SingleTimersCollection _timers = null;

        public SingleTimerLib.SingleTimersCollection TimersList
        {
            get { return _timers; }
        }

        public JobTimerForm()
        {
            InitializeComponent();

            _timers = new SingleTimerLib.SingleTimersCollection();
            _dropDownMenu = new ToolStripDropDownMenu();

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
//                SetupSavedTimers();               
            }
            jobTimersIcon.Visible = false;
        }

        private DataTable GetSavedTimers()
        {
            Timers.ReadXml(DataFile);
            Timers.AcceptChanges();
            timersDataGridView.Update();
            return Timers;
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
            activeTimersMenu.DropDownItems.Clear();
            jobTimersIcon.Visible = false;
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

        private DialogResult ShowSingleTimerEditorForm(DataGridViewCellCancelEventArgs e, out SingleTimer t)
        {
            SingleTimerEditorForm editor = new SingleTimerEditorForm(e.RowIndex);
            editor.QueryTimerNeeded += Editor_QueryTimerNeeded;
            t = editor.Timer;
            return editor.ShowDialog(this);
        }

        private void Editor_QueryTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            if(e.NewTimerNeeded)
            {
                e.Timer = new SingleTimer(e.RowIndex,string.Format("{0}","Cancel"));
                return;
            }
            try
            {
                e.Timer = TimersList[e.RowIndex];
            }
            catch (KeyNotFoundException)
            {
                timersDataGridView.Rows.RemoveAt(e.RowIndex);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void Timers_RowDeleting(object sender, DataRowChangeEventArgs e)
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
