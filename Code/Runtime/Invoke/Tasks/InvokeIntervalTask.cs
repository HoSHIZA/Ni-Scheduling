using NiGames.Essentials;
using NiGames.Essentials.Unsafe;

namespace NiGames.Scheduling.Tasks.Invoke
{
    public struct InvokeIntervalTask : IScheduledTask
    {
        public bool IsCompleted { get; private set; }
        public int UpdaterId => default;
        public TimeKind TimeKind { get; private set; }
        
        private ManagedPtr<InvokeTaskData> _dataPtr;
        
        private readonly float _interval;
        private readonly float _duration;
        private float _delay;
        private float _time;
        private float _elapsed;
        
        internal InvokeIntervalTask(InvokeTaskData taskData, float interval, float duration, float delay = 0) : this()
        {
            _dataPtr = taskData;

            _interval = interval;
            _duration = duration;
            _delay = delay;
        }
        
        public void Init(ref TaskWrapper wrapper, TimeKind timeKind = TimeKind.Time)
        {
            if (!_dataPtr.IsValid || !_dataPtr.Value.IsCallbackValid) IsCompleted = true;
            
            TimeKind = timeKind;
        }
        
        public void Update(in double time, in double unscaledTime, in double realtime, in double delta)
        {
            if (_dataPtr.Value.IsCancellationRequested)
            {
                IsCompleted = true;
                _dataPtr.Value.InvokeOnComplete();
                return;
            }
            
            if (_delay > 0)
            {
                _delay -= (float)delta;
                
                if (_delay < 0)
                {
                    _time -= _delay;
                    _elapsed -= _delay;
                }
                
                return;
            }
            
            _time += (float)delta;
            _elapsed += (float)delta;
            
            if (_time >= _interval)
            {
                _dataPtr.Value.InvokeCallback();

                _time -= _interval;
            }
            
            if (_duration > 0 && _elapsed >= _duration)
            {
                IsCompleted = true;
                _dataPtr.Value.InvokeOnComplete();
            }
        }
        
        public void Dispose()
        {
            _dataPtr.Dispose();
        }
    }
}