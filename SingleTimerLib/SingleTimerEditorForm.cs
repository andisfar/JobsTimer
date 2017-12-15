using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SingleTimerLib
{
    public partial class SingleTimerEditorForm : Form
    {
        int _rowIndex = -1;

        public delegate void TimerElapsedTimeChangedHandler(object sender, SingleTimerEditorFormElapsedTimeEventArgs e);
        public event TimerElapsedTimeChangedHandler TimerElapsedTimeChanged;

        public delegate void TimerNameChangedHandler(object sender, SingleTimerEditorFormNewNameEventArgs e);
        public event TimerNameChangedHandler TimerNameChanged;

        public void HandleTimerNameChanged(object sender, SingleTimerEditorFormNewNameEventArgs e)
        {
            TimerNameChanged?.Invoke(this, e);
        }

        public void HandleTimerElapsedTimeChanged(object sender, SingleTimerEditorFormElapsedTimeEventArgs e)
        {
            TimerElapsedTimeChanged?.Invoke(this, e);
        }

        private SingleTimerLib.SingleTimer _timer = null;

        public SingleTimerLib.SingleTimer Timer { get => _timer; set => _timer = value; }
        public int RowIndex { get => _rowIndex; }

        public SingleTimerEditorForm(int rowIndex)
        {
            InitializeComponent();
            Timer = null;
            _rowIndex = rowIndex;
            QueryRetrieveTimer(this, new SingleTimerEditorFormTimerNeededEventArgs(RowIndex, 0));
        }

        public delegate void SingleTimerEditorFormTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e);
        public event SingleTimerEditorFormTimerNeeded QueryTimerNeeded;

        private void QueryRetrieveTimer(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            QueryTimerNeeded?.Invoke(sender,e);
            Timer = e.Timer;
            if (Timer != null)
            {
                Timer.PropertyChanged += Timer_PropertyChanged;
                RunTimerCheckBox.ImageKey = Timer.IsRunning ? "stop" : "play";
                RunTimerCheckBox.Checked = Timer.IsRunning;
                ThreadSafeUpdateOfTimerElapsedTime(Timer.RunningElapsedTime);
                ThreadSafeUpdateTimerName(Timer.CanonicalName);
            }
            
        }

        private void Timer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DebugPrint(string.Format("[Timer_PropertyChanged]\t{0} changed!", e.PropertyName));
            switch(e.PropertyName)
            {
                case nameof(Timer.RunningElapsedTime):
                    ThreadSafeUpdateOfTimerElapsedTime(Timer.RunningElapsedTime);
                    break;
                case nameof(Timer.Name):
                    ThreadSafeUpdateTimerName(Timer.CanonicalName);
                    break;
                default:
                    break;
            }
        }

        private void ThreadSafeUpdateOfTimerElapsedTime(string runningElapsedTime)
        {
            if(TimerElapsedTimeLabel.InvokeRequired)
            {
                TimerElapsedTimeLabel.Invoke(new Action<string>(ThreadSafeUpdateOfTimerElapsedTime), runningElapsedTime);
                return;
            }
            TimerElapsedTimeLabel.Text = runningElapsedTime;
        }

        private void ThreadSafeUpdateTimerName(string canonicalName)
        {
            if (TimerNameLabel.InvokeRequired)
            {
                TimerNameLabel.Invoke(new Action<string>(ThreadSafeUpdateTimerName), canonicalName);
                return;
            }
            TimerNameLabel.Text = canonicalName;
        }

        private void DebugPrint(string message)
        {
            string messageWithTimeStamp = string.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss:fff"), message);
            Debug.Print(messageWithTimeStamp);
        }

        private void SingleTimerEditorForm_Load(object sender, EventArgs e)
        {
            QueryRetrieveTimer(this, new SingleTimerEditorFormTimerNeededEventArgs(RowIndex, 0));
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            if (Timer.CanonicalName != TimerNameLabel.Text)
            {
                Timer.ReNameTimer(TimerNameLabel.Text);
                //HandleTimerNameChanged(sender: TimerNameLabel, e: new SingleTimerEditorFormNewNameEventArgs(RowIndex, TimerNameLabel.Text, _timer.CanonicalName));
            }

            if (Timer.RunningElapsedTime != TimerElapsedTimeLabel.Text)
            {
                Timer.ResetTimer();
                //HandleTimerElapsedTimeChanged(sender: TimerElapsedTimeLabel, e: new SingleTimerEditorFormElapsedTimeEventArgs(RowIndex, TimerElapsedTimeLabel.Text));
            }

            this.Close();
        }

        private void rejectButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ResetTimerbutton_Click(object sender, EventArgs e)
        {
            Timer.ResetTimer();
        }

        private void RunTimerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RunTimerCheckBox.ImageKey = RunTimerCheckBox.Checked ? "stop" : "play";
            if (RunTimerCheckBox.Checked)
                Timer.StartTimer();
            else
                Timer.StopTimer();
        }
    }

    public class SingleTimerEditorFormNewNameEventArgs : EventArgs
    {
        private int _rowIndex;
        private string _timerNewName;
        private string _timerOldName;

        public int RowIndex { get => _rowIndex; }
        public string TimerNewName { get => _timerNewName; }
        public string TimerOldName { get => _timerOldName; }

        public SingleTimerEditorFormNewNameEventArgs(int rowIndex, string timerNewName, string timerOldName)
        {
            _rowIndex = rowIndex;
            _timerNewName = timerNewName;
            _timerOldName = timerOldName;
        }
    }

    public class SingleTimerEditorFormElapsedTimeEventArgs : EventArgs
    {
        private int _rowIndex;
        private string _timerElapsedTimer;
        private string _timerAlottedTime; // future use

        public int RowIndex { get => _rowIndex; }
        public string TimerElapsedTimer { get => _timerElapsedTimer; }
        public string TimerAlottedTime { get => _timerAlottedTime; }

        public SingleTimerEditorFormElapsedTimeEventArgs(int rowIndex, string timerElapsedTime, string timerAllotedTime = "00:00:00")
        {
            _rowIndex = rowIndex;
            _timerElapsedTimer = timerElapsedTime;
            _timerAlottedTime = timerAllotedTime;
        }
    }

    public class SingleTimerEditorFormTimerNeededEventArgs : EventArgs
    {
        private SingleTimer _t = null;
        public SingleTimer Timer { get => _t; set => _t = value; }

        private int _rowIndex = -1;
        public int RowIndex { get => _rowIndex; }

        private int _columnIndex = -1;
        public int ColumnIndex { get => _columnIndex; }



        public SingleTimerEditorFormTimerNeededEventArgs(int rowIndex, int columnIndex)
        {
            _rowIndex = rowIndex;
            _columnIndex = columnIndex;
        }
    }

}
