using System;
using NiGames.Essentials;
using NiGames.Scheduling.Dispatchers;
using Unity.Collections.LowLevel.Unsafe;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace NiGames.Scheduling
{
    public readonly struct PlayerLoopScheduler : IScheduler
    {
        public readonly PlayerLoopTiming Timing;
        public readonly TimeKind TimeKind;
        
        public PlayerLoopScheduler(PlayerLoopTiming timing, TimeKind timeKind = TimeKind.Time)
        {
            Timing = timing;
            TimeKind = timeKind;
        }
        
        public int GetRunnerId()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return EditorDispatcher.RunnerId;
            }
#endif
            return PlayerLoopDispatcher.GetTaskRunnerId(Timing);
        }
        
        public ref UnsafeList<TaskWrapper> GetTaskListRef()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return ref EditorDispatcher.GetTaskList();
            }
#endif
            
            return ref PlayerLoopDispatcher.GetTaskList(Timing);
        }
        
        public TaskWrapper Schedule<T>(T task) 
            where T : unmanaged, IScheduledTask
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return EditorDispatcher.Schedule(ref task);
            }
#endif
            
            return PlayerLoopDispatcher.Schedule(ref task, this);
        }
        
        public (THandle Handle, TaskWrapper Wrapper) Schedule<T, THandle>(T task, Func<TaskWrapper, THandle> handleFactory) 
            where T : unmanaged, IScheduledTask
        {
#if UNITY_EDITOR
            var wrapper = Application.isPlaying 
                ? EditorDispatcher.Schedule(ref task) 
                : PlayerLoopDispatcher.Schedule(ref task, this);
#else
            var wrapper = PlayerLoopDispatcher.Schedule(ref task, this);
#endif
            
            var handle = handleFactory.Invoke(wrapper);
            
            return (handle, wrapper);
        }
    }
}