using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SingleTimerLib
{
    public partial class SingleTimerEditorForm : Form
    {
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
        int _rowIndex = -1;

        public int Index { get => _rowIndex; }
        int _columnIndex = -1;

        public SingleTimerEditorForm(int rowIndex, int columnIndex)
        {
            Timer = null;            
        }

        public delegate void SingleTimerEditorFormTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e);
        public event SingleTimerEditorFormTimerNeeded QueryTimerNeeded;

        private void QueryRetrieveTimer(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            QueryTimerNeeded?.Invoke(sender,e);
            Timer = e.Timer;
            if(Timer != null) Timer.PropertyChanged += Timer_PropertyChanged;
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
            QueryRetrieveTimer(this, new SingleTimerEditorFormTimerNeededEventArgs(RowIndex));
        }

        private void TimerNameLabel_Leave(object sender, EventArgs e)
        {
            if(Timer?.CanonicalName != TimerNameLabel.Text)
            {
                HandleTimerNameChanged(TimerNameLabel, new SingleTimerEditorFormNewNameEventArgs(Timer.RowIndex, TimerNameLabel.Text, Timer.CanonicalName));
            }
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
        private int _rowIndex = -1;

        public SingleTimer Timer { get => _t; set => _t = value; }
        public int RowIndex { get => _rowIndex; }

        public SingleTimerEditorFormTimerNeededEventArgs(int rowIndex)
        {
            _rowIndex = rowIndex;
        }
    }

}
