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
        public Int32 RowIndex { get { return _rowIndex; } }

        private SingleTimerEditorForm _editor;
        public SingleTimerEditorForm Editor { get => _editor; }

        private System.Timers.Timer heartBeat;
        private Stopwatch stopWatch;

        private void OnResetTimer()
        {
            TimerReset?.Invoke(this, new SingleTimerLibEventArgs(RunningElapsedTime, CanonicalName, RowIndex, 0));
        }

        private void OnPropertyChangedEventHandler([CallerMemberName] string propertyName = "")
        {
            Int32 ColIndex = (propertyName == nameof(RunningElapsedTime)) ? 0 : 1;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SingleTimerChanged?.Invoke(this, new SingleTimerLibEventArgs(RunningElapsedTime, CanonicalName, RowIndex, ColIndex));
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
            _editor = new SingleTimerEditorForm();
            _editor.QueryTimerNeeded += SingleTimerEditorForm_QueryTimerNeeded;
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
            get { return TimerIsRunning ? _name + "*" : _name; }
            set { _name = value; OnPropertyChangedEventHandler(nameof(Name)); }
        }

        public string RunningElapsedTime
        {
            get { return string.Format("{0:00}:{1:00}:{2:00}", _running_hours, _running_minutes, _running_seconds); }
        }

        public bool TimerIsRunning
        {
            get { return stopWatch.IsRunning; }
        }

        public string CanonicalName => Name.Trim('*');

        public void ResetTimer()
        {
            _hours_offset = 0;
            _minutes_offset = 0;
            _seconds_offset = 0;

            if (TimerIsRunning)
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
            OnPropertyChangedEventHandler(nameof(TimerIsRunning));
        }

        public void StartTimer()
        {
            if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
                heartBeat.Enabled = true;
            }
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnPropertyChangedEventHandler(nameof(TimerIsRunning));
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
            Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }

        public void ReNameTimer(string name)
        {
            Name = name; ;
        }
    }

    public class SingleTimerLibEventArgs : EventArgs
    {
        private string _elapsedTime;
        private string _name;
        private Int32 _rowIndex;
        private Int32 _colIndex;

        public string ElapsedTime { get { return _elapsedTime; } }
        public string CanonicalName { get { return _name; } }
        public Int32 RowIndex { get { return _rowIndex; } }
        public Int32 ColumIndex { get { return _colIndex; } }

        public SingleTimerLibEventArgs(string elapsedTime, string name, Int32 rowIndex, Int32 colIndex)
        {
            _elapsedTime = elapsedTime;
            _name = name;
            _rowIndex = rowIndex;
            _colIndex = colIndex;
        }
    }
}
