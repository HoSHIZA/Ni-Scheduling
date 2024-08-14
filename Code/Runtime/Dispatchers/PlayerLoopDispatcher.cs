using System.Runtime.CompilerServices;
using NiGames.Essentials;
using NiGames.Essentials.Helpers;
using NiGames.Essentials.Unsafe;
using NiGames.Scheduling.Dispatchers.PlayerLoop;
using NiGames.Scheduling.Helpers;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace NiGames.Scheduling.Dispatchers
{
    namespace PlayerLoop
    {
        public struct NiSchedulerInitialization { }
        public struct NiSchedulerEarlyUpdate { }
        public struct NiSchedulerFixedUpdate { }
        public struct NiSchedulerPreUpdate { }
        public struct NiSchedulerUpdate { }
        public struct NiSchedulerPreLateUpdate { }
        public struct NiSchedulerPostLateUpdate { }
        public struct NiSchedulerTimeUpdate { }
    }
    
    public static unsafe class PlayerLoopDispatcher
    {
        public static class RunnerId
        {
            public static readonly uint Initialization = SchedulerInternalHelper.GetNextRunnerId();
            public static readonly uint EarlyUpdate = SchedulerInternalHelper.GetNextRunnerId();
            public static readonly uint FixedUpdate = SchedulerInternalHelper.GetNextRunnerId();
            public static readonly uint PreUpdate = SchedulerInternalHelper.GetNextRunnerId();
            public static readonly uint Update = SchedulerInternalHelper.GetNextRunnerId();
            public static readonly uint PreLateUpdate = SchedulerInternalHelper.GetNextRunnerId();
            public static readonly uint PostLateUpdate = SchedulerInternalHelper.GetNextRunnerId();
            public static readonly uint TimeUpdate = SchedulerInternalHelper.GetNextRunnerId();
        }
        
        private static bool _init;
        
        private static UnsafeList<TaskWrapper> _empty = new(0, Allocator.None);
        private static UnsafeList<TaskWrapper> _initialization = new(2, Allocator.Persistent);
        private static UnsafeList<TaskWrapper> _earlyUpdate = new(8, Allocator.Persistent);
        private static UnsafeList<TaskWrapper> _fixedUpdate = new(16, Allocator.Persistent);
        private static UnsafeList<TaskWrapper> _preUpdate = new(8, Allocator.Persistent);
        private static UnsafeList<TaskWrapper> _update = new(16, Allocator.Persistent);
        private static UnsafeList<TaskWrapper> _preLateUpdate = new(8, Allocator.Persistent);
        private static UnsafeList<TaskWrapper> _postLateUpdate = new(8, Allocator.Persistent);
        private static UnsafeList<TaskWrapper> _timeUpdate = new(4, Allocator.Persistent);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitLoops() => InitHelper.DomainSafeInit(ref _init, () =>
        {
            PlayerLoopHelper.ModifyLoop(systems =>
            {
                systems.InsertLoop<Initialization, NiSchedulerInitialization>(static () => Update(PlayerLoopTiming.Initialization));
                systems.InsertLoop<EarlyUpdate, NiSchedulerEarlyUpdate>(static () => Update(PlayerLoopTiming.EarlyUpdate));
                systems.InsertLoop<FixedUpdate, NiSchedulerFixedUpdate>(static () => Update(PlayerLoopTiming.FixedUpdate));
                systems.InsertLoop<PreUpdate, NiSchedulerPreUpdate>(static () => Update(PlayerLoopTiming.PreUpdate));
                systems.InsertLoop<Update, NiSchedulerUpdate>(static () => Update(PlayerLoopTiming.Update));
                systems.InsertLoop<PreLateUpdate, NiSchedulerPreLateUpdate>(static () => Update(PlayerLoopTiming.PreLateUpdate));
                systems.InsertLoop<PostLateUpdate, NiSchedulerPostLateUpdate>(static () => Update(PlayerLoopTiming.PostLateUpdate));
                systems.InsertLoop<TimeUpdate, NiSchedulerTimeUpdate>(static () => Update(PlayerLoopTiming.TimeUpdate));
            });
        });
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Clear();
        }
        
        [MethodImpl(256)]
        public static uint GetTaskRunnerId(PlayerLoopTiming timing)
        {
            return timing switch
            {
                PlayerLoopTiming.Initialization => RunnerId.Initialization,
                PlayerLoopTiming.EarlyUpdate => RunnerId.EarlyUpdate,
                PlayerLoopTiming.FixedUpdate => RunnerId.FixedUpdate,
                PlayerLoopTiming.PreUpdate => RunnerId.PreUpdate,
                PlayerLoopTiming.Update => RunnerId.Update,
                PlayerLoopTiming.PreLateUpdate => RunnerId.PreLateUpdate,
                PlayerLoopTiming.PostLateUpdate => RunnerId.PostLateUpdate,
                PlayerLoopTiming.TimeUpdate => RunnerId.TimeUpdate,
                _ => uint.MaxValue,
            };
        }
        
        public static TaskWrapper Schedule<T>(ref T task, PlayerLoopScheduler scheduler)
            where T : unmanaged, IScheduledTask
        {
            delegate*<out double, out double, out double, void> getTimeValuesFunction =
                scheduler.Timing is PlayerLoopTiming.FixedUpdate
                    ? &SchedulerTimeHelper.GetTimeValuesFixed
                    : &SchedulerTimeHelper.GetTimeValues;
            
            var wrapper = TaskWrapper.Create(ref task, getTimeValuesFunction, GetTaskRunnerId(scheduler.Timing));
            
            var ptr = wrapper.TaskPtr.ToPointer<T>();
            ptr->Init(ref wrapper, scheduler.TimeKind);
            
            GetTaskList(scheduler.Timing).Add(wrapper); 
            
            return wrapper;
        }
        
        private static void Update(PlayerLoopTiming timing)
        {
            ref var tasks = ref GetTaskList(timing);
            
            SchedulerTimeHelper.GetPlayerLoopTimeValues(timing, out var time, out var unscaledTime, out var realtime);
            
            for (var i = 0; i < tasks.Length; i++)
            {
                ref var task = ref tasks.ElementAt(i);
                
                try
                {
                    task.UpdateFunction(task.TaskPtr, time, unscaledTime, realtime);
                
                    if (task.IsCompletedFunction(task.TaskPtr))
                    {
                        NiUnsafe.Free(task.TaskPtr.ToPointer());
                        tasks.RemoveAtSwapBack(i);
                    }
                }
                catch
                {
                    NiUnsafe.Free(task.TaskPtr.ToPointer());
                    tasks.RemoveAtSwapBack(i);
                }
            }
        }
        
        [MethodImpl(256)]
        internal static ref UnsafeList<TaskWrapper> GetTaskList(PlayerLoopTiming timing)
        {
            switch (timing)
            {
                case PlayerLoopTiming.Initialization:  return ref _initialization;
                case PlayerLoopTiming.EarlyUpdate:     return ref _earlyUpdate;
                case PlayerLoopTiming.FixedUpdate:     return ref _fixedUpdate;
                case PlayerLoopTiming.PreUpdate:       return ref _preUpdate;
                case PlayerLoopTiming.Update:          return ref _update;
                case PlayerLoopTiming.PreLateUpdate:   return ref _preLateUpdate;
                case PlayerLoopTiming.PostLateUpdate:  return ref _postLateUpdate;
                case PlayerLoopTiming.TimeUpdate:      return ref _timeUpdate;
            }

            return ref _empty;
        }

        [MethodImpl(256)]
        private static void Clear()
        {
            SchedulerInternalHelper.ClearTaskList(ref _empty, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _initialization, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _earlyUpdate, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _fixedUpdate, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _preUpdate, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _update, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _preLateUpdate, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _postLateUpdate, wrapper => wrapper.TaskPtr);
            SchedulerInternalHelper.ClearTaskList(ref _timeUpdate, wrapper => wrapper.TaskPtr);
        }
    }
}