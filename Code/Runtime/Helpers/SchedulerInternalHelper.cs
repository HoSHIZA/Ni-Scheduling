using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NiGames.Essentials.Unsafe;
using Unity.Collections.LowLevel.Unsafe;

namespace NiGames.Scheduling.Helpers
{
    internal static unsafe class SchedulerInternalHelper
    {
        private static uint _nextRunnerId;
        
        [MethodImpl(256)]
        public static uint GetNextRunnerId()
        {
            return _nextRunnerId++;
        }
        
        public static void AllocTaskData<T>(ref T task, out IntPtr ptr)
            where T : unmanaged, IScheduledTask
        {
            ptr = (IntPtr)NiUnsafe.Malloc<T>();
            NiUnsafe.CopyStructureToPtr(ref task, ptr.ToPointer());
        }
        
        public static void UpdateTask<T>(IntPtr taskPtr, double time, double unscaledTime, double realtime)
            where T : unmanaged, IScheduledTask
        {
            var ptr = taskPtr.ToPointer<T>();
            ptr->Update(time, unscaledTime, realtime);
        }
        
        public static bool IsTaskCompleted<T>(IntPtr taskPtr)
            where T : unmanaged, IScheduledTask
        {
            var ptr = taskPtr.ToPointer<T>();
            return ptr->IsCompleted;
        }
        
        public static void ClearTaskList<T>(ref UnsafeList<T> taskList, [NotNull] Func<T, IntPtr> getPtr) 
            where T : unmanaged
        {
            for (int i = 0, length = taskList.Length; i < length; i++)
            {
                var ptr = getPtr.Invoke(taskList[i]).ToPointer<T>();
                NiUnsafe.Free(ptr);
            }
            
            taskList.Clear();
        }
    }
}