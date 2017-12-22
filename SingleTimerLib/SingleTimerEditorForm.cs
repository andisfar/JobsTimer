using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SingleTimerLib
{
    public enum EditActions
    {
        ChangeName,
        ChangedElapsedTimer,
        ResetElapsedTimer
    }

    public partial class SingleTimerEditorForm : Form
    {
        private List<EditActions> editActions = new List<EditActions>();

        int _rowIndex = -1;

        private SingleTimerLib.SingleTimer _timer = null;

        public SingleTimerLib.SingleTimer Timer { get => _timer; }
        public int RowIndex { get => _rowIndex; }

        private bool _newTimerNeeded;

        private int StartIn { get; set; }

        public SingleTimerEditorForm(DataGridViewCellCancelEventArgs e, bool isNewRow = false, SingleTimerEditorFormTimerNeeded QueryTimerNeededHandler = null)
        {
            InitializeComponent();
            _timer = null;
            _rowIndex = e.RowIndex;
            _newTimerNeeded = isNewRow;
            StartIn = e.ColumnIndex;
            if(QueryTimerNeededHandler != null)
            {
                QueryTimerNeeded += QueryTimerNeededHandler;
            }
            QueryRetrieveTimer(this, new SingleTimerEditorFormTimerNeededEventArgs(e.RowIndex, _newTimerNeeded));
        }

        public delegate void SingleTimerEditorFormTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e);
        public event SingleTimerEditorFormTimerNeeded QueryTimerNeeded;

        private void QueryRetrieveTimer(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            QueryTimerNeeded?.Invoke(sender,e);
            _timer = e.Timer;
            if (Timer != null)
            {
                //Timer.StopTimer();
                RunTimerCheckBox.ImageKey = Timer.IsRunning ? "stop" : "play";
                RunTimerCheckBox.Checked = Timer.IsRunning;
                ThreadSafeUpdateOfTimerElapsedTime(Timer.RunningElapsedTime);
                ThreadSafeUpdateTimerName(Timer.CanonicalName);
            }            
        }

        private void ThreadSafeUpdateOfTimerElapsedTime(string runningElapsedTime)
        {
            if(TimerElapsedTimeTextBox.InvokeRequired)
            {
                TimerElapsedTimeTextBox.Invoke(new Action<string>(ThreadSafeUpdateOfTimerElapsedTime), runningElapsedTime);
                return;
            }
            TimerElapsedTimeTextBox.Text = editActions.Contains(EditActions.ResetElapsedTimer)? Timer.BlankTimerValue() : runningElapsedTime;
        }

        private void ThreadSafeUpdateTimerName(string canonicalName)
        {
            if (TimerNameTextBox.InvokeRequired)
            {
                TimerNameTextBox.Invoke(new Action<string>(ThreadSafeUpdateTimerName), canonicalName);
                return;
            }
            TimerNameTextBox.Text = canonicalName;
        }

        private void DebugPrint(string message)
        {
            string messageWithTimeStamp = string.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss:fff"), message);
            Debug.Print(messageWithTimeStamp);
        }


        private void SingleTimerEditorForm_Load(object sender, EventArgs e)
        {            
            if(StartIn == 0)
            { ActiveControl = TimerNameTextBox; } else { ActiveControl = TimerElapsedTimeTextBox; }
            Timer.ElapsedTimeChanging += Timer_ElapsedTimeChanging;
            Application.DoEvents();
        }

        private void Timer_ElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            ThreadSafeUpdateOfTimerElapsedTime(e.ElapsedTime);
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            if (editActions.Contains(EditActions.ChangeName))
            {
                Timer.ReNameTimer(TimerNameTextBox.Text);
            }

            if (editActions.Contains(EditActions.ChangedElapsedTimer) ||
                editActions.Contains(EditActions.ResetElapsedTimer))
            {
                Timer.SetElapsedTime(TimerElapsedTimeTextBox.Text);
            }

            if (TimerNameTextBox.Text == string.Empty)
                Timer.Name = "Cancel";

            this.Close();
        }

        private void RejectButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            if (TimerNameTextBox.Text == string.Empty)
                Timer.Name = "Cancel";

            DebugPrint(string.Format("Elasped Timer {0}", Timer.RunningElapsedTime));
            DebugPrint(string.Format("Elasped Timer {0}", TimerElapsedTimeTextBox.Text));
            this.Close();
        }

        private void ResetTimerbutton_Click(object sender, EventArgs e)
        {
            TimerElapsedTimeTextBox.Text = Timer.BlankTimerValue();
            Application.DoEvents();
            if(!editActions.Contains(EditActions.ResetElapsedTimer))
                editActions.Add(EditActions.ResetElapsedTimer);
            RunTimerCheckBox.Checked = false;
            CheckRunStopTimer();
        }

        private void RunTimerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckRunStopTimer();
        }

        public delegate void StartTimerRequestedHandler(object sender, SingleTimerEditorFormStartTimerEventArgs e);
        public event StartTimerRequestedHandler RequestStartTimer;

        private void OnRequestStartTimer(object sender, SingleTimerEditorFormStartTimerEventArgs e)
        {
            RequestStartTimer?.Invoke(sender, e);
        }

        public delegate void StopTimerRequestedHandler(object sender, SingleTimerEditorFormStopTimerEventArgs e);
        public event StopTimerRequestedHandler RequestStopTimer;

        private void OnRequestStopTimer(object sender, SingleTimerEditorFormStopTimerEventArgs e)
        {
            RequestStopTimer?.Invoke(sender, e);
        }

        private void CheckRunStopTimer()
        {
            UpdateRunStopImage();
            if (RunTimerCheckBox.Checked)
            {
                OnRequestStartTimer(this, new SingleTimerEditorFormStartTimerEventArgs(Timer.RowIndex));
                //Timer.StartTimer();
                //TimerElapsedTimeTextBox.Text = Timer.RunningElapsedTime;
            }
            else
                OnRequestStopTimer(this, new SingleTimerEditorFormStopTimerEventArgs(Timer.RowIndex));
        }

        private void UpdateRunStopImage()
        {
            RunTimerCheckBox.ImageKey = RunTimerCheckBox.Checked ? "stop" : "play";
        }

        private void TimerNameTextBox_Validated(object sender, EventArgs e)
        {
            if (Timer == null) QueryRetrieveTimer(this, new SingleTimerEditorFormTimerNeededEventArgs(RowIndex, _newTimerNeeded));
            Debug.Assert(Timer != null);
            if(TimerNameTextBox.Text != Timer.CanonicalName)
            {
                if(!editActions.Contains(EditActions.ChangeName))editActions.Add(EditActions.ChangeName);
            }
        }

        private void TimerElapsedTimeTextBox_Validated(object sender, EventArgs e)
        {
            if(TimerElapsedTimeTextBox.Text != Timer.RunningElapsedTime)
            {
                if (!editActions.Contains(EditActions.ChangedElapsedTimer)) editActions.Add(EditActions.ChangedElapsedTimer);
            }
        }

        private void TimerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            acceptButton.Enabled = true;
        }

        private void TimerElapsedTimeTextBox_TextChanged(object sender, EventArgs e)
        {
            acceptButton.Enabled = true;
        }
    }

    public class SingleTimerEditorFormStopTimerEventArgs
    {
        private int _rowIndex;
        public int RowIndex { get => _rowIndex; }

        public SingleTimerEditorFormStopTimerEventArgs(int rowIndex)
        {
            _rowIndex = rowIndex;
        }
    }

    public class SingleTimerEditorFormStartTimerEventArgs
    {
        private int _rowIndex;
        public int RowIndex { get => _rowIndex; }

        public SingleTimerEditorFormStartTimerEventArgs(int rowIndex)
        {
            _rowIndex = rowIndex;
        }
    }

    public class SingleTimerEditorFormTimerNeededEventArgs : EventArgs
    {
        private SingleTimer _t = null;
        public SingleTimer Timer { get => _t; set => _t = value; }

        private bool _needNewTimer = false;
        public bool NewTimerNeeded { get => _needNewTimer;}

        private int _rowIndex = -1;
        public int RowIndex { get => _rowIndex; }

        public SingleTimerEditorFormTimerNeededEventArgs(int rowIndex, bool needsNewTimer = false)
        {
            _rowIndex = rowIndex;
            _needNewTimer = needsNewTimer;
        }
    }

}
