using System;
using System.Threading;
using NiGames.Essentials;
using NiGames.Essentials.Unsafe;
using UnityEngine;

namespace NiGames.Scheduling.Tasks.Invoke
{
    public struct InvokeRepeatTask : IScheduledTask
    {
        public bool IsCompleted { get; private set; }
        public int UpdaterId => default;
        public TimeKind TimeKind { get; private set; }
        
        private ManagedPtr<InvokeTaskData> _dataPtr;
        
        private readonly float _duration;
        private float _delay;
        private float _time;
        
        internal InvokeRepeatTask(InvokeTaskData taskData, float duration, float delay = 0) : this()
        {
            _dataPtr = taskData;
            
            _duration = duration;
            _delay = delay;
        }
        
        public void Init(ref TaskWrapper wrapper, TimeKind timeKind = TimeKind.Time)
        {
            if (!_dataPtr.IsValid || !_dataPtr.Value.IsCallbackValid) IsCompleted = true;
            
            TimeKind = timeKind;
            
            _dataPtr.Value.InvokeOnStart();
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
                    _dataPtr.Value.InvokeOnStartDelayed();
                    _time -= _delay;
                }
                
                return;
            }
            
            _dataPtr.Value.InvokeCallback();
            
            if (_duration > 0)
            {
                _time += (float)delta;
                
                if (_time >= _duration)
                {
                    IsCompleted = true;
                    _dataPtr.Value.InvokeOnComplete();
                }
            }
        }
        
        public void Dispose()
        {
            _dataPtr.Dispose();
        }
    }
}