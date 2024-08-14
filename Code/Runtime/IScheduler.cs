using System;
using Unity.Collections.LowLevel.Unsafe;

namespace NiGames.Scheduling
{
    public interface IScheduler
    {
        uint GetRunnerId();
        ref UnsafeList<TaskWrapper> GetTaskListRef();
        TaskWrapper Schedule<T>(T task) where T : unmanaged, IScheduledTask;
        (THandle Handle, TaskWrapper Wrapper) Schedule<T, THandle>(T task, Func<TaskWrapper, THandle> handleFactory) where T : unmanaged, IScheduledTask;
    }
}