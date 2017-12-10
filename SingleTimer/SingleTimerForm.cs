using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SingleTimer
{
    public partial class SingleTimerForm : Form
    {
        private long _running_hours = 0;
        private long _running_minutes = 0;
        private long _running_seconds = 0;

        private long _hours_offset = 0;
        private long _minutes_offset = 0;
        private long _seconds_offset = 0;

        private Stopwatch stopWatch = null;

        public delegate void SingleTimerFormClosingHandler(object sender, SingleTimerEventArgs e);

        public event SingleTimerFormClosingHandler OnTimerFormClosing;

        public delegate void SingleTimerResetTimerHandler(object sender, SingleTimerEventArgs e);

        public event SingleTimerResetTimerHandler OnResetTimer;

        public delegate void SingleTimerUpdateTimerHandler(object sender, SingleTimerEventArgs e);

        public event SingleTimerUpdateTimerHandler OnUpdateTimer;

        public delegate void SingleTimerFormRequestToClose(object sender, FormClosingEventArgs e);

        public event SingleTimerFormRequestToClose OnRequestToClose;

        public void HandleOnRequestToClose(object sender, FormClosingEventArgs e) => OnRequestToClose?.Invoke(this, e);

        public void OnTimerFormClosingHandler() => OnTimerFormClosing?.Invoke(this, new SingleTimerEventArgs(RunningElapsedTime, Text, RowIndex));

        public void OnResetTimerHandler() => OnResetTimer?.Invoke(this, new SingleTimerEventArgs(RunningElapsedTime, Text, RowIndex));

        public void OnUpdateTimerHandler() => OnUpdateTimer?.Invoke(this, new SingleTimerEventArgs(RunningElapsedTime, Text, RowIndex));

        public int RowIndex { get; set; }

        public string RunningElapsedTime
        {
            get
            {
                return string.Format("{0:00}:{1:00}:{2:00}", _running_hours, _running_minutes, _running_seconds);
            }
        }

        public string ElapsedTimeOffset
        {
            get
            {
                return string.Format("{0:00}:{1:00}:{2:00}", _hours_offset, _minutes_offset, _seconds_offset);
            }

            set
            {
                if (value == string.Empty)
                {
                    _hours_offset = 0;
                    _minutes_offset = 0;
                    _seconds_offset = 0;
                }
                else
                {
                    string[] elapsedTime = value.Split(':');
                    _hours_offset = Int32.Parse(elapsedTime[0]);
                    _minutes_offset = Int32.Parse(elapsedTime[1]);
                    _seconds_offset = Int32.Parse(elapsedTime[2]);
                }
            }
        }

        public SingleTimerForm(string elapsedTimeOffset)
        {
            InitializeComponent();
            Properties.Settings.Default.Reload();
            heartBeat.Interval = 1000;
            heartBeat.Enabled = false;
            heartBeat.Tick += HeartBeat_Tick;
            stopWatch = new Stopwatch();
            ElapsedTimeOffset = elapsedTimeOffset;
            IncrementTime();
            SetElapsedTimeLabel();
        }

        private void HeartBeat_Tick(object sender, EventArgs e) => HandleTimerElapsed();

        public void HandleTimerElapsed()
        {
            IncrementTime();
            SetElapsedTimeLabel();
        }

        private void IncrementTime()
        {
            TimeSpan runningTime = new TimeSpan((int)_hours_offset, (int)_minutes_offset, (int)_seconds_offset);
            runningTime += stopWatch.Elapsed;
            _running_seconds = runningTime.Seconds;
            _running_minutes = runningTime.Minutes;
            _running_hours = runningTime.Hours;
            this.OnUpdateTimerHandler();
        }

        public void ResetTimer(bool resetStopWatch = true)
        {
            _hours_offset = 0;
            _minutes_offset = 0;
            _seconds_offset = 0;
            if (resetStopWatch)
                stopWatch.Reset();
            else
            {
                if (TimerIsRunning)
                {
                    stopWatch.Restart();
                }
                else
                    stopWatch.Reset();
            }
            IncrementTime();
            SetElapsedTimeLabel();
            OnUpdateTimerHandler();
        }

        private void SingleTimerForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey | Keys.W:
                    DoClose();
                    break;

                default:
                    break;
            }
        }

        public void StopTimer()
        {
            if (stopWatch.IsRunning)
                StopButton_Click((object)stopButton, new EventArgs());
            OnUpdateTimerHandler();
        }

        public void StartTimer()
        {
            if (!stopWatch.IsRunning)
                StartButton_Click((object)startButton, new EventArgs());
            OnUpdateTimerHandler();
        }

        public bool TimerIsRunning
        {
            get { return stopWatch.IsRunning; }
        }

        private void DoClose()
        {
            Close();
        }

        private void SetElapsedTimeLabel()
        {
            timerLabel.Text = _elapsedTime;
        }

        private string _elapsedTime
        {
            get
            {
                return RunningElapsedTime;
            }
        }

        private void SingleTimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            HandleOnRequestToClose(this, e);
            e.Cancel = (!(e.CloseReason == CloseReason.UserClosing));
            if (e.Cancel) return;
            heartBeat.Enabled = false;
            heartBeat.Tick -= HeartBeat_Tick;
            Properties.Settings.Default.Save();
            this.OnTimerFormClosingHandler();
        }

        private void TimerLabel_TextChanged(object sender, EventArgs e)
        {
            Debug.Print(string.Format("The Label Changed for {0}!", Text));
            Debug.Print(timerLabel.Text);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            stopWatch.Start();
            heartBeat.Enabled = true;
            stopButton.Enabled = true;
            startButton.Enabled = false;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            stopWatch.Stop();
            heartBeat.Enabled = false;
            stopButton.Enabled = false;
            startButton.Enabled = true;
            this.OnUpdateTimerHandler();
        }

        private void TimerLabel_DoubleClick(object sender, EventArgs e)
        {
            if (startButton.Enabled)
            {
                if (MessageBox.Show(this, string.Format("Reset timer value for:\n'{0}'", Text), "Reset Timer Value", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ResetTimer(); IncrementTime();
                    SetElapsedTimeLabel();
                    this.OnResetTimerHandler();
                }
            }
        }
    }

    public class SingleTimerEventArgs : EventArgs
    {
        private string _elapsedTime;
        private string _windowTitle;
        private Int32 _rowIndex;

        public string ElapsedTime { get { return _elapsedTime; } }
        public string WindowTitle { get { return _windowTitle; } }
        public Int32 RowIndex { get { return _rowIndex; } }

        public SingleTimerEventArgs(string elapsedTime, string windowTitle, Int32 RowIndex)
        {
            _elapsedTime = elapsedTime;
            _windowTitle = windowTitle;
            _rowIndex = RowIndex;
        }       
    }
}
