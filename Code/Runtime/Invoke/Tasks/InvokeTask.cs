using System;
using System.Threading;
using NiGames.Essentials;
using NiGames.Essentials.Unsafe;

namespace NiGames.Scheduling.Tasks.Invoke
{
    public struct InvokeTask : IScheduledTask
    {
        public bool IsCompleted { get; private set; }
        public int UpdaterId => default;
        public TimeKind TimeKind { get; private set; }
        
        private ManagedPtr<InvokeTaskData> _dataPtr;
        
        private float _delay;
        
        internal InvokeTask(InvokeTaskData taskData, float delay = 0) : this()
        {
            _dataPtr = taskData;
            
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
                
                return;
            }
            
            _dataPtr.Value.InvokeCallback();
            
            IsCompleted = true;
            _dataPtr.Value.InvokeOnComplete();
        }
        
        public void Dispose()
        {
            _dataPtr.Dispose();
        }
    }
}