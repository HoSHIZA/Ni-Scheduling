using System;
using Unity.Collections.LowLevel.Unsafe;

#if UNITY_EDITOR
using NiGames.Scheduling.Dispatchers;
#else
using Unity.Collections;
#endif

namespace NiGames.Scheduling
{
    public readonly struct EditorScheduler : IScheduler
    {
#if UNITY_EDITOR
        public int GetRunnerId() => EditorDispatcher.RunnerId;
        public ref UnsafeList<TaskWrapper> GetTaskListRef() => ref EditorDispatcher.GetTaskList();
        
        public TaskWrapper Schedule<T>(T task) where T : unmanaged, IScheduledTask
        {
            return EditorDispatcher.Schedule(ref task);
        }
        
        public (THandle Handle, TaskWrapper Wrapper) Schedule<T, THandle>(T task, Func<TaskWrapper, THandle> handleFactory) 
            where T : unmanaged, IScheduledTask
        {
            var wrapper = EditorDispatcher.Schedule(ref task);
            var handle = handleFactory.Invoke(wrapper);
            
            return (handle, wrapper);
        }
#else
        private static UnsafeList<TaskWrapper> _empty = new(0, Allocator.None);
        
        public int GetRunnerId() => -1;
        public ref UnsafeList<TaskWrapper> GetTaskListRef() => ref _empty;
        
        public TaskWrapper Schedule<T>(T task) where T : unmanaged, IScheduledTask
        {
            return default;
        }
        
        public (THandle Handle, TaskWrapper Wrapper) Schedule<T, THandle>(T task, Func<TaskWrapper, THandle> handleFactory) 
            where T : unmanaged, IScheduledTask
        {
            return default;
        }
#endif
    }
}