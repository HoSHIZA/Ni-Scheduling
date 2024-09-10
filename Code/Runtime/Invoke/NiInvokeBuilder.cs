using System;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using NiGames.Essentials;
using NiGames.Essentials.Pooling.Buffer;
using NiGames.Scheduling.Tasks.Invoke;

namespace NiGames.Scheduling.Invoke
{
    internal sealed class NiInvokeBuilderBuffer : AbstractPooledBuffer<NiInvokeBuilderBuffer>
    {
        public IScheduler Scheduler;
        
        // Callback Data
        public Action Callback;
        public Action OnStart;
        public Action OnStartDelayed;
        public Action OnComplete;
        public CancellationToken CancellationToken;
        
        // Data
        public float Delay;
        public float Duration;
        public float Interval;
        
        public bool Preserve;
        
        protected override void Reset()
        {
            Scheduler = Scheduling.Scheduler.Default;
            
            Callback = default;
            OnStart = default;
            OnStartDelayed = default;
            OnComplete = default;
            CancellationToken = default;
            
            Delay = default;
            Duration = default;
            Interval = default;
            
            Preserve = false;
        }
    }
    
    [PublicAPI]
    public struct NiInvokeBuilder
    {
        internal ushort Revision;
        internal NiInvokeBuilderBuffer Buffer;
        
        internal NiInvokeBuilder(NiInvokeBuilderBuffer buffer)
        {
            Revision = buffer.Revision;
            Buffer = buffer;
        }
        
        #region Creation
        
        [MethodImpl(256)]
        public static NiInvokeBuilder Create(Action callback)
        {
            var buffer = NiInvokeBuilderBuffer.GetPooled();
            buffer.Callback = callback;
            
            return new NiInvokeBuilder(buffer);
        }
        
        #endregion
        
        #region With - {...}
        
        [MethodImpl(256)]
        public readonly NiInvokeBuilder WithDelay(float delay)
        {
            ValidateBuffer();

            Buffer.Delay = delay;

            return this;
        }
        
        [MethodImpl(256)]
        public readonly NiInvokeBuilder WithDuration(float duration)
        {
            ValidateBuffer();
            
            Buffer.Duration = duration;
            
            return this;
        }
        
        [MethodImpl(256)]
        public readonly NiInvokeBuilder WithInterval(float interval)
        {
            ValidateBuffer();

            Buffer.Interval = interval;

            return this;
        }
        
        [MethodImpl(256)]
        public readonly NiInvokeBuilder WithScheduler(IScheduler scheduler)
        {
            ValidateBuffer();

            Buffer.Scheduler = scheduler;

            return this;
        }
        
        [MethodImpl(256)]
        public readonly NiInvokeBuilder WithPlayerLoopScheduler(PlayerLoopTiming timing, TimeKind timeKind = TimeKind.Time)
        {
            ValidateBuffer();
            
            Buffer.Scheduler = Scheduler.GetScheduler(timing, timeKind);
            
            return this;
        }
        
        [MethodImpl(256)]
        public readonly NiInvokeBuilder WithCancellationToken(CancellationToken token)
        {
            ValidateBuffer();
            
            Buffer.CancellationToken = token;

            return this;
        }
        
        /// <summary>
        /// Does not call <c>Dispose()</c> on binding if <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <c>Dispose()</c> must be called manually to return the buffer to the pool.
        /// </remarks>
        [MethodImpl(256)]
        public readonly NiInvokeBuilder Preserve(bool preserve = true)
        {
            ValidateBuffer();
            
            Buffer.Preserve = preserve;
            
            return this;
        }
        
        #endregion
        
        #region On - {...}

        /// <summary>
        /// Subscribes to <c>OnStartAction</c>. Called when the callback prepared to start.
        /// </summary>
        [MethodImpl(256)]
        public readonly NiInvokeBuilder OnStart([NotNull] Action callback)
        {
            ValidateBuffer();
            
            Buffer.OnStart += callback;
            
            return this;
        }
        
        /// <summary>
        /// Subscribes to <c>OnStartDelayedAction</c>. Called when the callback starts after delay.
        /// </summary>
        [MethodImpl(256)]
        public readonly NiInvokeBuilder OnStartDelayed([NotNull] Action callback)
        {
            ValidateBuffer();
            
            Buffer.OnStartDelayed += callback;
            
            return this;
        }

        /// <summary>
        /// Subscribes to <c>OnCompleteAction</c>. Called when the callback completes.
        /// </summary>
        [MethodImpl(256)]
        public readonly NiInvokeBuilder OnComplete([NotNull] Action callback)
        {
            ValidateBuffer();
            
            Buffer.OnComplete += callback;
            
            return this;
        }
        
        #endregion
        
        /// <summary>
        /// Calls only the first scheduler update.
        /// </summary>
        public void InvokeOnce()
        {
            var data = new InvokeTaskData(Buffer.Callback, Buffer.OnComplete, Buffer.OnStart, Buffer.OnStartDelayed, Buffer.CancellationToken);
            var task = new InvokeTask(data, Buffer.Delay);
            
            Buffer.Scheduler.Schedule(task);
            
            if (!Buffer.Preserve)
            {
                Dispose();
            }
        }
        
        /// <summary>
        /// Calls every update of the scheduler.
        /// </summary>
        public void InvokeRepeat()
        {
            var data = new InvokeTaskData(Buffer.Callback, Buffer.OnComplete, Buffer.OnStart, Buffer.OnStartDelayed, Buffer.CancellationToken);

            if (Buffer.Interval > 0)
            {
                Buffer.Scheduler.Schedule(new InvokeIntervalTask(data, Buffer.Interval, Buffer.Duration, Buffer.Delay));
            }
            else
            {
                Buffer.Scheduler.Schedule(new InvokeRepeatTask(data, Buffer.Duration, Buffer.Delay));
            }
            
            if (!Buffer.Preserve)
            {
                Dispose();
            }
        }
        
        public void Dispose()
        {
            if (Buffer == null) return;
            
            NiInvokeBuilderBuffer.Release(Buffer);
            
            Buffer = null;
        }
        
        [MethodImpl(256)]
        private readonly void ValidateBuffer()
        {
            if (Buffer == null || Buffer.Revision != Revision)
            {
                throw new InvalidOperationException("[NiInvoke] NiInvokeBuilder is not initialized before execution, or binding has already been done. Use Preserve() to reuse the builder.");
            }
        }
    }
}