using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NiGames.Scheduling.Tasks.Invoke
{
    internal readonly struct InvokeTaskData
    {
        public readonly Action Callback;
        public readonly Action OnComplete;
        public readonly Action OnStart;
        public readonly Action OnStartDelayed;
        public readonly CancellationToken CancellationToken;
        
        public bool IsCallbackValid => Callback != null;
        public bool IsCancellationRequested => CancellationToken.IsCancellationRequested;
        
        public InvokeTaskData(Action callback, Action onComplete, Action onStart, Action onStartDelayed, CancellationToken cancellationToken) 
            : this()
        {
            Callback = callback;
            OnComplete = onComplete;
            OnStart = onStart;
            OnStartDelayed = onStartDelayed;
            CancellationToken = cancellationToken;
        }
        
        [MethodImpl(256)]
        public void InvokeCallback() => Callback.Invoke();
        
        [MethodImpl(256)]
        public void InvokeOnStart() => OnStart?.Invoke();
        
        [MethodImpl(256)]
        public void InvokeOnStartDelayed() => OnStartDelayed?.Invoke();
        
        [MethodImpl(256)]
        public void InvokeOnComplete() => OnComplete?.Invoke();
    }
}