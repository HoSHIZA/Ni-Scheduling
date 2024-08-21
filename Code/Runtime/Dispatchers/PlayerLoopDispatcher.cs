using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("ReSharper", "UnusedTypeParameter")]
        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        private static class Cache<T> where T : struct
        {
            public static UnsafeList<TaskWrapper> Tasks;
            public static PlayerLoopTiming Timing;
            public static readonly int RunnerId = SchedulerInternalHelper.GetNextRunnerId();
            
            private static double _lastTime;
            private static double _lastUnscaledTime;
            private static double _lastRealtime;
            
            [MethodImpl(256)]
            public static void Init(PlayerLoopTiming timing, int capacity, Allocator allocator = Allocator.Persistent)
            {
                Timing = timing;
                Tasks = new UnsafeList<TaskWrapper>(capacity, allocator);
            }
            
            [MethodImpl(256)]
            public static void Update()
            {
                SchedulerTimeHelper.GetPlayerLoopTimeValues(Timing, out var time, out var unscaledTime, out var realtime);
                
                if (!Tasks.IsEmpty)
                {
                    var deltaTime = time - _lastTime;
                    var deltaUnscaledTime = unscaledTime - _lastUnscaledTime;
                    var deltaRealtime = realtime - _lastRealtime;
                    
                    SchedulerInternalHelper.ProcessTasks(ref Tasks, time, unscaledTime, realtime, deltaTime, deltaUnscaledTime, deltaRealtime);
                }
                
                _lastTime = time;
                _lastUnscaledTime = time;
                _lastRealtime = time;
            }
            
            [MethodImpl(256)]
            public static void ClearTaskList()
            {
                SchedulerInternalHelper.ClearTaskList(ref Tasks);
            }
        }
        
        private static bool _init;
        
        private static UnsafeList<TaskWrapper> _empty = new(0, Allocator.None);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitLoops() => InitHelper.DomainSafeInit(ref _init, () =>
        {
            Cache<Initialization>.Init(PlayerLoopTiming.Initialization, 2);
            Cache<EarlyUpdate>.Init(PlayerLoopTiming.EarlyUpdate, 2);
            Cache<FixedUpdate>.Init(PlayerLoopTiming.FixedUpdate, 4);
            Cache<PreUpdate>.Init(PlayerLoopTiming.PreUpdate, 2);
            Cache<Update>.Init(PlayerLoopTiming.Update, 4);
            Cache<PreLateUpdate>.Init(PlayerLoopTiming.PreLateUpdate, 2);
            Cache<PostLateUpdate>.Init(PlayerLoopTiming.PostLateUpdate, 2);
            Cache<TimeUpdate>.Init(PlayerLoopTiming.TimeUpdate, 2);
            
            PlayerLoopHelper.ModifyLoop(systems =>
            {
                systems.InsertLoop<Initialization, NiSchedulerInitialization>(static () => Cache<Initialization>.Update());
                systems.InsertLoop<EarlyUpdate, NiSchedulerEarlyUpdate>(static () => Cache<EarlyUpdate>.Update());
                systems.InsertLoop<FixedUpdate, NiSchedulerFixedUpdate>(static () => Cache<FixedUpdate>.Update());
                systems.InsertLoop<PreUpdate, NiSchedulerPreUpdate>(static () => Cache<PreUpdate>.Update());
                systems.InsertLoop<Update, NiSchedulerUpdate>(static () => Cache<Update>.Update());
                systems.InsertLoop<PreLateUpdate, NiSchedulerPreLateUpdate>(static () => Cache<PreLateUpdate>.Update());
                systems.InsertLoop<PostLateUpdate, NiSchedulerPostLateUpdate>(static () => Cache<PostLateUpdate>.Update());
                systems.InsertLoop<TimeUpdate, NiSchedulerTimeUpdate>(static () => Cache<TimeUpdate>.Update());
            });
        });
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Clear();
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
        
        [MethodImpl(256)]
        public static void Clear()
        {
            Cache<Initialization>.ClearTaskList();
            Cache<EarlyUpdate>.ClearTaskList();
            Cache<FixedUpdate>.ClearTaskList();
            Cache<PreUpdate>.ClearTaskList();
            Cache<Update>.ClearTaskList();
            Cache<PreLateUpdate>.ClearTaskList();
            Cache<PostLateUpdate>.ClearTaskList();
            Cache<TimeUpdate>.ClearTaskList();
        }
        
        [MethodImpl(256)]
        public static int GetTaskRunnerId(PlayerLoopTiming timing)
        {
            return timing switch
            {
                PlayerLoopTiming.Initialization => Cache<Initialization>.RunnerId,
                PlayerLoopTiming.EarlyUpdate => Cache<EarlyUpdate>.RunnerId,
                PlayerLoopTiming.FixedUpdate => Cache<FixedUpdate>.RunnerId,
                PlayerLoopTiming.PreUpdate => Cache<PreUpdate>.RunnerId,
                PlayerLoopTiming.Update => Cache<Update>.RunnerId,
                PlayerLoopTiming.PreLateUpdate => Cache<PreLateUpdate>.RunnerId,
                PlayerLoopTiming.PostLateUpdate => Cache<PostLateUpdate>.RunnerId,
                PlayerLoopTiming.TimeUpdate => Cache<TimeUpdate>.RunnerId,
                _ => -1
            };
        }
        
        [MethodImpl(256)]
        public static ref UnsafeList<TaskWrapper> GetTaskList(PlayerLoopTiming timing)
        {
            switch (timing)
            {
                case PlayerLoopTiming.Initialization:  return ref Cache<Initialization>.Tasks;
                case PlayerLoopTiming.EarlyUpdate:     return ref Cache<EarlyUpdate>.Tasks;
                case PlayerLoopTiming.FixedUpdate:     return ref Cache<FixedUpdate>.Tasks;
                case PlayerLoopTiming.PreUpdate:       return ref Cache<PreUpdate>.Tasks;
                case PlayerLoopTiming.Update:          return ref Cache<Update>.Tasks;
                case PlayerLoopTiming.PreLateUpdate:   return ref Cache<PreLateUpdate>.Tasks;
                case PlayerLoopTiming.PostLateUpdate:  return ref Cache<PostLateUpdate>.Tasks;
                case PlayerLoopTiming.TimeUpdate:      return ref Cache<TimeUpdate>.Tasks;
            }

            return ref _empty;
        }
    }
}