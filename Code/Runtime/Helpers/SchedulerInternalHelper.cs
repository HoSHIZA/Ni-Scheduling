using System;
using System.Runtime.CompilerServices;
using NiGames.Essentials;
using NiGames.Essentials.Unsafe;
using Unity.Collections.LowLevel.Unsafe;

namespace NiGames.Scheduling.Helpers
{
    internal static unsafe class SchedulerInternalHelper
    {
        private static int _nextRunnerId;
        
        public static int GetNextRunnerId()
        {
            return _nextRunnerId++;
        }
        
        #region Task WorkFlow
        
        public static void UpdateTask<T>(IntPtr taskPtr, 
            in double time, in double unscaledTime, in double realtime, 
            in double deltaTime, in double deltaUnscaledTime, in double deltaRealtime)
            where T : unmanaged, IScheduledTask
        {
            var ptr = taskPtr.ToPointer<T>();
            
            var delta = ptr->TimeKind switch
            {
                TimeKind.Time => deltaTime,
                TimeKind.UnscaledTime => deltaUnscaledTime,
                _ => deltaRealtime,
            };
            
            ptr->Update(time, unscaledTime, realtime, delta);
        }
        
        public static bool IsTaskCompleted<T>(IntPtr taskPtr)
            where T : unmanaged, IScheduledTask
        {
            var ptr = taskPtr.ToPointer<T>();
            return ptr->IsCompleted;
        }
        
        #endregion
        
        [MethodImpl(256)]
        public static void AllocTask<T>(ref T task, out IntPtr ptr)
            where T : unmanaged, IScheduledTask
        {
            ptr = (IntPtr)NiUnsafe.Malloc<T>();
            NiUnsafe.CopyStructureToPtr(ref task, ptr.ToPointer());
        }
        
        [MethodImpl(256)]
        public static void ClearTaskList(ref UnsafeList<TaskWrapper> taskList) 
        {
            for (int i = 0, length = taskList.Length; i < length; i++)
            {
                taskList.ElementAt(i).Free();
            }
            
            taskList.Clear();
        }
        
        [MethodImpl(256)]
        public static void ProcessTasks(ref UnsafeList<TaskWrapper> tasks, 
            in double time, in double unscaledTime, in double realtime, 
            in double deltaTime, in double deltaUnscaledTime, in double deltaRealtime)
        {
            for (var i = 0; i < tasks.Length; i++)
            {
                ref var task = ref tasks.ElementAt(i);
                
                try
                {
                    task.UpdateFunction(task.TaskPtr, time, unscaledTime, realtime, deltaTime, deltaUnscaledTime, deltaRealtime);
                    
                    if (task.IsCompletedFunction(task.TaskPtr))
                    {
                        tasks.ElementAt(i).Free();
                        tasks.RemoveAtSwapBack(i);
                    }
                }
                catch
                {
                    tasks.ElementAt(i).Free();
                    tasks.RemoveAtSwapBack(i);

                    throw;
                }
            }
        }
    }
}