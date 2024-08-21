using System;
using System.Runtime.CompilerServices;
using NiGames.Essentials.Unsafe;
using NiGames.Scheduling.Helpers;

namespace NiGames.Scheduling
{
    public unsafe struct TaskWrapper
    {
        public IntPtr TaskPtr;
        public int UpdaterId;
        
        internal delegate*<out double, out double, out double, void> GetTimeValuesFunction;
        internal delegate*<IntPtr, in double, in double, in double, in double, in double, in double, void> UpdateFunction;
        internal delegate*<IntPtr, bool> IsCompletedFunction;
        
        public bool IsValid => 
            UpdaterId > 0 && 
            TaskPtr.ToPointer() != null && 
            GetTimeValuesFunction != null && 
            UpdateFunction != null && 
            IsCompletedFunction != null;
        
        public static TaskWrapper Create<T>(IntPtr ptr, delegate*<out double, out double, out double, void> getTimeValuesFunction,
            int updaterId)
            where T : unmanaged, IScheduledTask
        {
            return new TaskWrapper
            {
                TaskPtr = ptr,
                UpdaterId = updaterId,
                GetTimeValuesFunction = getTimeValuesFunction,
                UpdateFunction = &SchedulerInternalHelper.UpdateTask<T>,
                IsCompletedFunction = &SchedulerInternalHelper.IsTaskCompleted<T>,
            };
        }
        
        public static TaskWrapper Create<T>(ref T task, delegate*<out double, out double, out double, void> getTimeValuesFunction,
            int updaterId)
            where T : unmanaged, IScheduledTask
        {
            SchedulerInternalHelper.AllocTask(ref task, out var ptr);
            
            return new TaskWrapper
            {
                TaskPtr = ptr,
                UpdaterId = updaterId,
                GetTimeValuesFunction = getTimeValuesFunction,
                UpdateFunction = &SchedulerInternalHelper.UpdateTask<T>,
                IsCompletedFunction = &SchedulerInternalHelper.IsTaskCompleted<T>,
            };
        }
    }
    
    public static unsafe class TaskWrapperExtensions
    {
        [MethodImpl(256)]
        public static void GetTimeValuesAsDouble(ref this TaskWrapper task, out double time, out double unscaledTime, out double realtime)
        {
            task.GetTimeValuesFunction(out time, out unscaledTime, out realtime);
        }
        
        [MethodImpl(256)]
        public static void GetTimeValues(ref this TaskWrapper task, out float time, out float unscaledTime, out float realtime)
        {
            task.GetTimeValuesFunction(out var t, out var u, out var r);
            
            time = (float)t;
            unscaledTime = (float)u;
            realtime = (float)r;
        }
        
        [MethodImpl(256)]
        public static void Free(ref this TaskWrapper task)
        {
            NiUnsafe.Free(task.TaskPtr.ToPointer());
        }
    }
}