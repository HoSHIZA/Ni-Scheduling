using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NiGames.Scheduling.Tasks.Invoke
{
    internal readonly struct InvokeTaskData
    {
        public readonly CancellationToken CancellationToken;
        
        private readonly Action _callback;
        private readonly Action _onComplete;
        private readonly Action _onStart;
        private readonly Action _onStartDelayed;
        
        public bool IsCallbackValid => _callback != null;
        public bool IsCancellationRequested => CancellationToken.IsCancellationRequested;
        
        public InvokeTaskData(Action callback, Action onComplete, Action onStart, Action onStartDelayed, CancellationToken cancellationToken) 
            : this()
        {
            _callback = callback;
            _onComplete = onComplete;
            _onStart = onStart;
            _onStartDelayed = onStartDelayed;
            CancellationToken = cancellationToken;
        }
        
        [MethodImpl(256)]
        public void InvokeCallback() => _callback.Invoke();
        
        [MethodImpl(256)]
        public void InvokeOnStart() => _onStart?.Invoke();
        
        [MethodImpl(256)]
        public void InvokeOnStartDelayed() => _onStartDelayed?.Invoke();
        
        [MethodImpl(256)]
        public void InvokeOnComplete() => _onComplete?.Invoke();
    }
}