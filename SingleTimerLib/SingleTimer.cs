using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SingleTimerLib
{
    public enum InfoTypes
    {
        Default,
        TimerEvents
    }

    public enum TimerStates
    {
        Running,
        Stopped
    }

    public class SingleTimer : INotifyPropertyChanged, IDisposable
    {
        public delegate void TimerResetHandler(object sender, SingleTimerLibEventArgs e);

        public event TimerResetHandler TimerReset;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void SingleTimerChangedHandler(object sender, SingleTimerLibEventArgs e);

        public delegate void SingleTimerNameChanging(object sender, SingleTimerNameChangingEventArgs e, [CallerMemberName] string caller="");
        public event SingleTimerNameChanging NameChanging;

        public delegate void SingleTimerElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [CallerMemberName] string caller = "");
        public event SingleTimerElapsedTimeChanging ElapsedTimeChanging;

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

        public Delegate[] @ElapsedTimeChangingInvocationList
        {
            get
            {
                MulticastDelegate m = (MulticastDelegate)ElapsedTimeChanging;
                return m?.GetInvocationList();
            }
        }

        public Delegate[] @NameChangingInvocationList
        {
            get
            {
                MulticastDelegate m = (MulticastDelegate)this.NameChanging;
                return m?.GetInvocationList();
            }
        }

        public Delegate[] @TimerResetInvocationList
        {
            get
            {
                
                MulticastDelegate m = (MulticastDelegate)this.TimerReset;
                return m?.GetInvocationList();
            }
        }

        public void OnElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e)
        {
            ElapsedTimeChanging?.Invoke(this, e);
        }

        private void OnNameChanging(object sender, SingleTimerNameChangingEventArgs e)
        {
            NameChanging?.Invoke(sender, e);
        }

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
            OnElapsedTimeChanging(this, new SingleTimerElapsedTimeChangingEventArgs(RunningElapsedTime, this));
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

        internal string BlankTimerValue()
        {            
            return string.Format("{0}:{1}:{2}","00","00","00");
        }

        public void StartOrStop()
        {
            if (IsRunning)
                StopTimer();
            else
                StartTimer();
        }

        private void HeartBeat_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            heartBeat.Enabled = false;
            HandleTimerElapsed();
            heartBeat.Enabled = true;            
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
            set { OnNameChanging(this, new SingleTimerNameChangingEventArgs(CanonicalName, value, this)); _name = value; OnPropertyChangedEventHandler(); }
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

        public void SetElapsedTime(string runningElapsedTime)
        {
           TimeSpan newTime = new TimeSpan(ParseHours(runningElapsedTime)[0], ParseHours(runningElapsedTime)[1], ParseHours(runningElapsedTime)[2]);
            _running_hours = newTime.Hours;
            _running_minutes = newTime.Minutes;
            _running_seconds = newTime.Seconds;
            OnElapsedTimeChanging(this, new SingleTimerElapsedTimeChangingEventArgs(RunningElapsedTime, this));
        }

        private int[] ParseHours(string runningElapsedTime)
        {
            List<int> retInts = new List<int>{0,0,0};
            string[] intStr = runningElapsedTime.Split(':');
            retInts[0] = Int32.Parse(intStr[0]);
            retInts[1] = Int32.Parse(intStr[1]);
            retInts[2] = Int32.Parse(intStr[2]);
            return retInts.ToArray();
        }

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

        private void DebugPrint(InfoTypes showMe = InfoTypes.Default)
        {
            switch (showMe)
            {
                case InfoTypes.TimerEvents:
                    {
                        DebugPrint(string.Format("Name  = {0}", nameof(ElapsedTimeChanging)));
                        try
                        {
                            foreach (Delegate @d in ElapsedTimeChangingInvocationList)
                            {
                                DebugPrint(string.Format("Value = {0}.{1}", d.GetMethodInfo().ReflectedType.Name, d.GetMethodInfo().Name));
                            }
                        }
                        catch (Exception)
                        {
                            DebugPrint(string.Format("Value = {0}", "Not Set"));
                        }

                        DebugPrint(string.Format("Name  = {0}", nameof(NameChanging)));
                        try
                        {
                            foreach (Delegate @d in NameChangingInvocationList)
                            {
                                DebugPrint(string.Format("Value = {0}.{1}", d.GetMethodInfo().ReflectedType.Name, d.GetMethodInfo().Name));
                            }
                        }
                        catch (Exception)
                        {
                            DebugPrint(string.Format("Value = {0}", "Not Set"));
                        }

                        DebugPrint(string.Format("Name  = {0}", nameof(TimerReset)));
                        try
                        {
                            foreach (Delegate @d in TimerResetInvocationList)
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
                        DebugPrint(string.Format("Timer Row Index: {0}, Timer State: {1}", RowIndex, TimerState.ToString()));
                        break;
                    }
            }
        }

        public TimerStates TimerState { get => stopWatch.IsRunning ? TimerStates.Running : TimerStates.Stopped; }

        public void StopTimer()
        {
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                heartBeat.Enabled = false;
            }
            DebugPrint(string.Format("'{0}' is now stopped!",CanonicalName));
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
            DebugPrint(string.Format("'{0}' is now running!",CanonicalName));
            DebugPrint(InfoTypes.TimerEvents);
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnPropertyChangedEventHandler(nameof(IsRunning));
            OnElapsedTimeChanging(this, new SingleTimerElapsedTimeChangingEventArgs(RunningElapsedTime, this));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //release any native resources here
            }
            StopTimer();
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

    public class SingleTimerElapsedTimeChangingEventArgs : EventArgs
    {
        private SingleTimer _t = null;
        public SingleTimer Timer { get => _t; }

        private string _elapsedTime = string.Empty;
        public string ElapsedTime { get => _elapsedTime; }

        public SingleTimerElapsedTimeChangingEventArgs(string elapsedTime, SingleTimer t, [CallerMemberName] string caller = "")
        {
            _t = t;
            _elapsedTime = elapsedTime;
        }
    }

    public class SingleTimerNameChangingEventArgs : EventArgs
    {
        private string _oldName = string.Empty;
        private string _newName = string.Empty;

        public string OldName { get => _oldName; }
        public string NewName { get => _newName; }
        public SingleTimer Timer { get; private set; }

        public SingleTimerNameChangingEventArgs(string oldName, string newName, SingleTimer t, [CallerMemberName] string caller = "")
        {
            _oldName = oldName;
            _newName = newName;
            Timer = t;
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
