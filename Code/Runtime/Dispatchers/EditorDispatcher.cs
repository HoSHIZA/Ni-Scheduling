#if UNITY_EDITOR
using System.Runtime.CompilerServices;
using NiGames.Essentials.Unsafe;
using NiGames.Scheduling.Helpers;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

namespace NiGames.Scheduling.Dispatchers
{
    public static unsafe class EditorDispatcher
    {
        public static readonly int RunnerId = SchedulerInternalHelper.GetNextRunnerId();
        
        private static UnsafeList<TaskWrapper> _tasks = new(4, Allocator.Persistent);
        
        private static double _lastTime;
        private static double _lastRealtime;
        
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.update += static () => Update();
            
            _lastTime = EditorApplication.timeSinceStartup;
            _lastRealtime = Time.realtimeSinceStartupAsDouble;
        }
        
        public static TaskWrapper Schedule<T>(ref T task)
            where T : unmanaged, IScheduledTask
        {
            var wrapper = TaskWrapper.Create(ref task, &SchedulerTimeHelper.GetEditorTimeValues, RunnerId);
            
            var ptr = wrapper.TaskPtr.ToPointer<T>();
            ptr->Init(ref wrapper);
            
            _tasks.Add(wrapper);
            
            return wrapper;
        }

        private static void Update()
        {
            SchedulerTimeHelper.GetEditorTimeValues(out var time, out var realtime);
            
            if (!_tasks.IsEmpty)
            {
                var deltaTime = time - _lastTime;
                var deltaRealtime = realtime - _lastRealtime;
                
                SchedulerInternalHelper.ProcessTasks(ref _tasks, time, time, realtime, deltaTime, deltaTime, deltaRealtime);
            }
            
            _lastTime = time;
            _lastRealtime = realtime;
        }

        [MethodImpl(256)]
        public static ref UnsafeList<TaskWrapper> GetTaskList()
        {
            return ref _tasks;
        }
    }
}
#endif
