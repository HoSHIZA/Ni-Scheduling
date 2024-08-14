#if UNITY_EDITOR
using System.Runtime.CompilerServices;
using NiGames.Essentials.Unsafe;
using NiGames.Scheduling.Helpers;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;

namespace NiGames.Scheduling.Dispatchers
{
    public static unsafe class EditorDispatcher
    {
        public static readonly uint RunnerId = SchedulerInternalHelper.GetNextRunnerId();
        
        private static UnsafeList<TaskWrapper> _tasks = new(8, Allocator.Persistent);
        
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.update += static () => Update();
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
            
            for (var i = 0; i < _tasks.Length; i++)
            {
                ref var task = ref _tasks.ElementAt(i);
                
                try
                {
                    task.UpdateFunction(task.TaskPtr, time, time, realtime);
                    
                    if (task.IsCompletedFunction(task.TaskPtr))
                    {
                        NiUnsafe.Free(task.TaskPtr.ToPointer());
                        _tasks.RemoveAtSwapBack(i);
                    }
                }
                catch
                {
                    NiUnsafe.Free(task.TaskPtr.ToPointer());
                    _tasks.RemoveAtSwapBack(i);
                    
                    throw;
                }
            }
        }

        [MethodImpl(256)]
        internal static ref UnsafeList<TaskWrapper> GetRunnerList()
        {
            return ref _tasks;
        }
    }
}
#endif
