using NiGames.Essentials;

namespace NiGames.Scheduling
{
    public static class Scheduler
    {
        public static readonly IScheduler Initialization = new PlayerLoopScheduler(PlayerLoopTiming.Initialization, TimeKind.Time);
        public static readonly IScheduler EarlyUpdate = new PlayerLoopScheduler(PlayerLoopTiming.EarlyUpdate, TimeKind.Time);
        public static readonly IScheduler FixedUpdate = new PlayerLoopScheduler(PlayerLoopTiming.FixedUpdate, TimeKind.Time);
        public static readonly IScheduler PreUpdate = new PlayerLoopScheduler(PlayerLoopTiming.PreUpdate, TimeKind.Time);
        public static readonly IScheduler Update = new PlayerLoopScheduler(PlayerLoopTiming.Update, TimeKind.Time);
        public static readonly IScheduler PreLateUpdate = new PlayerLoopScheduler(PlayerLoopTiming.PreLateUpdate, TimeKind.Time);
        public static readonly IScheduler PostLateUpdate = new PlayerLoopScheduler(PlayerLoopTiming.PostLateUpdate, TimeKind.Time);
        public static readonly IScheduler TimeUpdate = new PlayerLoopScheduler(PlayerLoopTiming.TimeUpdate, TimeKind.Time);
        
        public static readonly IScheduler InitializationUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.Initialization, TimeKind.UnscaledTime);
        public static readonly IScheduler EarlyUpdateUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.EarlyUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler FixedUpdateUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.FixedUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler PreUpdateUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.PreUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler UpdateUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.Update, TimeKind.UnscaledTime);
        public static readonly IScheduler PreLateUpdateUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.PreLateUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler PostLateUpdateUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.PostLateUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler TimeUpdateUnscaledTime = new PlayerLoopScheduler(PlayerLoopTiming.TimeUpdate, TimeKind.UnscaledTime);
        
        public static readonly IScheduler InitializationRealtime = new PlayerLoopScheduler(PlayerLoopTiming.Initialization, TimeKind.Realtime);
        public static readonly IScheduler EarlyUpdateRealtime = new PlayerLoopScheduler(PlayerLoopTiming.EarlyUpdate, TimeKind.Realtime);
        public static readonly IScheduler FixedUpdateRealtime = new PlayerLoopScheduler(PlayerLoopTiming.FixedUpdate, TimeKind.Realtime);
        public static readonly IScheduler PreUpdateRealtime = new PlayerLoopScheduler(PlayerLoopTiming.PreUpdate, TimeKind.Realtime);
        public static readonly IScheduler UpdateRealtime = new PlayerLoopScheduler(PlayerLoopTiming.Update, TimeKind.Realtime);
        public static readonly IScheduler PreLateUpdateRealtime = new PlayerLoopScheduler(PlayerLoopTiming.PreLateUpdate, TimeKind.Realtime);
        public static readonly IScheduler PostLateUpdateRealtime = new PlayerLoopScheduler(PlayerLoopTiming.PostLateUpdate, TimeKind.Realtime);
        public static readonly IScheduler TimeUpdateRealtime = new PlayerLoopScheduler(PlayerLoopTiming.TimeUpdate, TimeKind.Realtime);
        
        public static readonly IScheduler Manual = new ManualScheduler();
        
        /// <summary>
        /// PlayerLoop <c>Update</c> Scheduler
        /// </summary>
        public static IScheduler Default => Update;
        
        /// <summary>
        /// Returns the appropriate <c>IScheduler</c> for the passed PlayerLoop update type and TimeKind.
        /// </summary>
        public static IScheduler GetScheduler(PlayerLoopTiming timing, TimeKind timeKind)
        {
            return timing switch
            {
                PlayerLoopTiming.Initialization => timeKind switch
                {
                    TimeKind.Time => Initialization,
                    TimeKind.UnscaledTime => InitializationUnscaledTime,
                    _ => InitializationRealtime,
                },
                PlayerLoopTiming.EarlyUpdate => timeKind switch
                {
                    TimeKind.Time => EarlyUpdate,
                    TimeKind.UnscaledTime => EarlyUpdateUnscaledTime,
                    _ => EarlyUpdateRealtime,
                },
                PlayerLoopTiming.FixedUpdate => timeKind switch
                {
                    TimeKind.Time => FixedUpdate,
                    TimeKind.UnscaledTime => FixedUpdateUnscaledTime,
                    _ => FixedUpdateRealtime,
                },
                PlayerLoopTiming.PreUpdate => timeKind switch
                {
                    TimeKind.Time => PreUpdate,
                    TimeKind.UnscaledTime => PreUpdateUnscaledTime,
                    _ => PreUpdateRealtime,
                },
                PlayerLoopTiming.Update => timeKind switch
                {
                    TimeKind.Time => Update,
                    TimeKind.UnscaledTime => UpdateUnscaledTime,
                    _ => UpdateRealtime,
                },
                PlayerLoopTiming.PreLateUpdate => timeKind switch
                {
                    TimeKind.Time => PreLateUpdate,
                    TimeKind.UnscaledTime => PreLateUpdateUnscaledTime,
                    _ => PreLateUpdateRealtime,
                },
                PlayerLoopTiming.PostLateUpdate => timeKind switch
                {
                    TimeKind.Time => PostLateUpdate,
                    TimeKind.UnscaledTime => PostLateUpdateUnscaledTime,
                    _ => PostLateUpdateRealtime,
                },
                PlayerLoopTiming.TimeUpdate => timeKind switch
                {
                    TimeKind.Time => TimeUpdate,
                    TimeKind.UnscaledTime => TimeUpdateUnscaledTime,
                    _ => TimeUpdateRealtime,
                },
                _ => Manual
            };
        }
    }
}