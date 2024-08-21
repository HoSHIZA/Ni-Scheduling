using System;
using System.Threading;

namespace NiGames.Scheduling.Invoke
{
    public sealed class NiInvokeBuilderBuffer
    {
        private static NiInvokeBuilderBuffer _poolRoot = new();
        
        public ushort Revision;
        public NiInvokeBuilderBuffer Next;
        
        public Action Callback;
        public Action OnStart;
        public Action OnStartDelayed;
        public Action OnComplete;
        public CancellationToken CancellationToken;
        
        public IScheduler Scheduler = Scheduling.Scheduler.Default;
        public float Delay;
        public float Duration;
        public float Interval;
        
        public bool Preserve;
        
        public static NiInvokeBuilderBuffer GetPooled()
        {
            if (_poolRoot == null) return new NiInvokeBuilderBuffer();
            
            var result = _poolRoot;
            _poolRoot = _poolRoot.Next;
            result.Next = null;
            
            return result;
        }
        
        public static void Release(NiInvokeBuilderBuffer buffer)
        {
            buffer.Revision++;
            
            buffer.Callback = default;
            buffer.OnStart = default;
            buffer.OnStartDelayed = default;
            buffer.OnComplete = default;
            buffer.CancellationToken = default;
            
            buffer.Scheduler = Scheduling.Scheduler.Default;
            buffer.Delay = default;
            buffer.Duration = default;
            buffer.Interval = default;
            
            buffer.Preserve = default;
            
            if (buffer.Revision != ushort.MaxValue)
            {
                buffer.Next = _poolRoot;
                _poolRoot = buffer;
            }
        }
    }
}