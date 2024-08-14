using System;
using System.Runtime.CompilerServices;
using NiGames.Scheduling.Helpers;

namespace NiGames.Scheduling
{
    /// <summary>
    /// Структура представляющая задачу. Содержит указатель на задачу, а так же указатели на делегаты 
    /// </summary>
    public unsafe struct TaskWrapper
    {
        public IntPtr TaskPtr;
        public uint UpdaterId;
        
        internal delegate*<out double, out double, out double, void> GetTimeValuesFunction;
        internal delegate*<IntPtr, double, double, double, void> UpdateFunction;
        internal delegate*<IntPtr, bool> IsCompletedFunction;
        
        public static TaskWrapper Create<T>(IntPtr ptr, delegate*<out double, out double, out double, void> getTimeValuesFunction,
            uint updaterId)
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
            uint updaterId)
            where T : unmanaged, IScheduledTask
        {
            SchedulerInternalHelper.AllocTaskData(ref task, out var ptr);
            
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
        public static void GetTimeValues(ref this TaskWrapper task, out double time, out double unscaledTime, out double realtime)
        {
            task.GetTimeValuesFunction(out time, out unscaledTime, out realtime);
        }
    }
}