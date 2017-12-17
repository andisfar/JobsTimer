using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SingleTimerLib
{
    public class SingleTimer : INotifyPropertyChanged, IDisposable
    {
        public delegate void TimerResetHandler(object sender, SingleTimerLibEventArgs e);

        public event TimerResetHandler TimerReset;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void SingleTimerChangedHandler(object sender, SingleTimerLibEventArgs e);

        public event SingleTimerChangedHandler SingleTimerChanged;

        private long _running_hours = 0;
        private long _running_minutes = 0;
        private long _running_seconds = 0;

        private long _hours_offset = 0;
        private long _minutes_offset = 0;
        private long _seconds_offset = 0;

        private Int32 _rowIndex;
        public Int32 RowIndex { get => _rowIndex; set { _rowIndex = value; OnPropertyChangedEventHandler(); } }

        private System.Timers.Timer heartBeat;
        private Stopwatch stopWatch;

        private void OnResetTimer()
        {
            TimerReset?.Invoke(this, new SingleTimerLibEventArgs(this));
        }

        private void OnPropertyChangedEventHandler([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SingleTimerChanged?.Invoke(this, new SingleTimerLibEventArgs(this));
        }

        public SingleTimer(Int32 rowIndex, string name)
        {
            _name = name;
            _rowIndex = rowIndex;
            FinishInit("00:00:00");
        }

        public SingleTimer(Int32 rowIndex, string name, string elapsedTimeOffset)
        {
            _name = name;
            _rowIndex = rowIndex;
            FinishInit(elapsedTimeOffset);
        }

        private void FinishInit(string elapsedTimeOffset)
        {
            OnPropertyChangedEventHandler(nameof(RowIndex));
            OnPropertyChangedEventHandler(nameof(Name));
            heartBeat = new System.Timers.Timer();
            stopWatch = new Stopwatch();

            heartBeat.Interval = 1000;
            heartBeat.Enabled = false;
            heartBeat.Elapsed += HeartBeat_Elapsed;
            stopWatch = new Stopwatch();
            ElapsedTimeOffset = elapsedTimeOffset;
            IncrementTime();
            SetElapsedTimeLabel();
        }

        private void SingleTimerEditorForm_QueryTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            e.Timer = this;
        }

        public SingleTimer Instance { get => this; }

        private void SetElapsedTimeLabel()
        {
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
        }

        private void IncrementTime()
        {
            TimeSpan runningTime = new TimeSpan((int)_hours_offset, (int)_minutes_offset, (int)_seconds_offset);
            runningTime += stopWatch.Elapsed;
            _running_seconds = runningTime.Seconds;
            _running_minutes = runningTime.Minutes;
            _running_hours = runningTime.Hours;
        }

        private string ElapsedTimeOffset
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

        private void HeartBeat_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            HandleTimerElapsed();
        }

        public void HandleTimerElapsed()
        {
            IncrementTime();
            SetElapsedTimeLabel();
        }

        private string _name;

        public string Name
        {
            get { return IsRunning ? _name + "*" : _name; }
            set { _name = value; OnPropertyChangedEventHandler(); }
        }

        public string RunningElapsedTime
        {
            get { return string.Format("{0:00}:{1:00}:{2:00}", _running_hours, _running_minutes, _running_seconds); }
        }

        public bool IsRunning
        {
            get { return stopWatch.IsRunning; }
        }

        public string CanonicalName => Name.Trim('*');

        public string MenuText { get => string.Format(CanonicalName + "-[{0}]",RunningElapsedTime); }

        public void ResetTimer()
        {
            _hours_offset = 0;
            _minutes_offset = 0;
            _seconds_offset = 0;

            if (IsRunning)
            {
                stopWatch.Restart();
            }
            else
            {
                stopWatch.Reset();
            }

            IncrementTime();
            SetElapsedTimeLabel();
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnResetTimer();
        }

        public void StopTimer()
        {
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                heartBeat.Enabled = false;
            }
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnPropertyChangedEventHandler(nameof(IsRunning));
        }

        public void StartTimer()
        {
            if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
                heartBeat.Enabled = true;
            }
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnPropertyChangedEventHandler(nameof(IsRunning));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //release any native resources here
            }
            StopTimer();
            ResetTimer();
            heartBeat.Dispose();
        }

        public void Dispose()
        {
            DebugPrint(string.Format("Timer '{0}' is being disposed!", CanonicalName));
            Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }

        public void ReNameTimer(string name)
        {
            Name = name; ;
        }

        private void DebugPrint(string message, [CallerMemberName] string caller = "")
        {
            string messageWithTimeStamp = string.Format("[{0}]\t{1} says {2}", DateTime.Now.ToString("HH:mm:ss:fff"), caller, message);
            Debug.Print(messageWithTimeStamp);
        }
    }

    public class SingleTimerLibEventArgs : EventArgs
    {
        private SingleTimer _t = null;
        public SingleTimer Timer { get => _t; }

        public SingleTimerLibEventArgs(SingleTimer t)
        {           
            _t = t;
        }
    }    
}
