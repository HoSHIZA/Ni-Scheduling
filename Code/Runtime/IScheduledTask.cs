using System;
using NiGames.Essentials;

namespace NiGames.Scheduling
{
    public interface IScheduledTask : IDisposable
    {
        bool IsCompleted { get; }
        int UpdaterId { get; }
        TimeKind TimeKind { get; }
        
        /// <summary>
        /// Initializes the task using the given data.
        /// </summary>
        /// <param name="wrapper">Wrapper object of a task that is ready to be executed.</param>
        /// <param name="timeKind">TimeKind used in <c>Update</c></param>
        public void Init(ref TaskWrapper wrapper, TimeKind timeKind = TimeKind.Time);
        
        /// <summary>
        /// Method that is called every update.
        /// </summary>
        public void Update(in double time, in double unscaledTime, in double realtime, in double delta);
    }
}