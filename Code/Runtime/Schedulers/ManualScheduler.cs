using System;
using NiGames.Scheduling.Dispatchers;
using Unity.Collections.LowLevel.Unsafe;

namespace NiGames.Scheduling
{
    public readonly struct ManualScheduler : IScheduler
    {
        public uint GetRunnerId() => ManualDispatcher.RunnerId;
        public ref UnsafeList<TaskWrapper> GetTaskListRef() => ref ManualDispatcher.GetRunnerList();
        
        public TaskWrapper Schedule<T>(T task) where T : unmanaged, IScheduledTask
        {
            return ManualDispatcher.Schedule(ref task);
        }
        
        public (THandle Handle, TaskWrapper Wrapper) Schedule<T, THandle>(T task, Func<TaskWrapper, THandle> handleFactory) 
            where T : unmanaged, IScheduledTask
        {
            var wrapper = ManualDispatcher.Schedule(ref task);
            var handle = handleFactory.Invoke(wrapper);
            
            return (handle, wrapper);
        }
    }
}