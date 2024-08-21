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
        
        public static readonly IScheduler InitializationUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.Initialization, TimeKind.UnscaledTime);
        public static readonly IScheduler EarlyUpdateUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.EarlyUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler FixedUpdateUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.FixedUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler PreUpdateUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.PreUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler UpdateUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.Update, TimeKind.UnscaledTime);
        public static readonly IScheduler PreLateUpdateUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.PreLateUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler PostLateUpdateUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.PostLateUpdate, TimeKind.UnscaledTime);
        public static readonly IScheduler TimeUpdateUnscaled = new PlayerLoopScheduler(PlayerLoopTiming.TimeUpdate, TimeKind.UnscaledTime);
        
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
        /// Updates only in the editor.
        /// </summary>
        public static readonly IScheduler Editor = new EditorScheduler();
        
        /// <summary>
        /// PlayerLoop <c>Update</c> Scheduler.
        /// </summary>
        public static IScheduler Default => Update;
        
        /// <summary>
        /// Returns <c>IScheduler</c> for the specified PlayerLoop and TimeKind update timings.
        /// </summary>
        public static IScheduler GetScheduler(PlayerLoopTiming timing, TimeKind timeKind)
        {
            return timing switch
            {
                PlayerLoopTiming.Initialization => timeKind switch
                {
                    TimeKind.Time => Initialization,
                    TimeKind.UnscaledTime => InitializationUnscaled,
                    _ => InitializationRealtime,
                },
                PlayerLoopTiming.EarlyUpdate => timeKind switch
                {
                    TimeKind.Time => EarlyUpdate,
                    TimeKind.UnscaledTime => EarlyUpdateUnscaled,
                    _ => EarlyUpdateRealtime,
                },
                PlayerLoopTiming.FixedUpdate => timeKind switch
                {
                    TimeKind.Time => FixedUpdate,
                    TimeKind.UnscaledTime => FixedUpdateUnscaled,
                    _ => FixedUpdateRealtime,
                },
                PlayerLoopTiming.PreUpdate => timeKind switch
                {
                    TimeKind.Time => PreUpdate,
                    TimeKind.UnscaledTime => PreUpdateUnscaled,
                    _ => PreUpdateRealtime,
                },
                PlayerLoopTiming.Update => timeKind switch
                {
                    TimeKind.Time => Update,
                    TimeKind.UnscaledTime => UpdateUnscaled,
                    _ => UpdateRealtime,
                },
                PlayerLoopTiming.PreLateUpdate => timeKind switch
                {
                    TimeKind.Time => PreLateUpdate,
                    TimeKind.UnscaledTime => PreLateUpdateUnscaled,
                    _ => PreLateUpdateRealtime,
                },
                PlayerLoopTiming.PostLateUpdate => timeKind switch
                {
                    TimeKind.Time => PostLateUpdate,
                    TimeKind.UnscaledTime => PostLateUpdateUnscaled,
                    _ => PostLateUpdateRealtime,
                },
                PlayerLoopTiming.TimeUpdate => timeKind switch
                {
                    TimeKind.Time => TimeUpdate,
                    TimeKind.UnscaledTime => TimeUpdateUnscaled,
                    _ => TimeUpdateRealtime,
                },
                _ => Manual,
            };
        }
    }
}