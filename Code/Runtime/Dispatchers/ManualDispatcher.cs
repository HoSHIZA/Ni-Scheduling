using System;
using System.Runtime.CompilerServices;
using NiGames.Essentials.Unsafe;
using NiGames.Scheduling.Helpers;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace NiGames.Scheduling.Dispatchers
{
    public static unsafe class ManualDispatcher
    {
        public static double Time { get; set; }
        
        public static readonly uint RunnerId = SchedulerInternalHelper.GetNextRunnerId();
        
        private static UnsafeList<TaskWrapper> _tasks = new(8, Allocator.Persistent); 
        
        public static TaskWrapper Schedule<T>(ref T task)
            where T : unmanaged, IScheduledTask
        {
            var wrapper = TaskWrapper.Create(ref task, &GetTime, RunnerId);
            
            var ptr = wrapper.TaskPtr.ToPointer<T>();
            ptr->Init(ref wrapper);
            
            _tasks.Add(wrapper);
            
            return wrapper;
        }
        
        public static void GetTime(out double time, out double unscaledTime, out double realtime)
        {
            time = Time;
            unscaledTime = Time;
            realtime = Time;
        }
        
        public static void Update(double deltaTime)
        {
            Time += Math.Max(0, deltaTime);
            
            for (var i = 0; i < _tasks.Length; i++)
            {
                ref var task = ref _tasks.ElementAt(i);
                
                try
                {
                    task.UpdateFunction(task.TaskPtr, Time, Time, Time);
                    
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