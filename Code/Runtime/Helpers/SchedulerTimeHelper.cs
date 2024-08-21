using System.Runtime.CompilerServices;
using NiGames.Essentials;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NiGames.Scheduling.Helpers
{
    public static class SchedulerTimeHelper
    {
#if UNITY_EDITOR
        public static void GetEditorTimeValues(out double time, out double unscaledTime, out double realtime)
        {
            time = EditorApplication.timeSinceStartup;
            unscaledTime = EditorApplication.timeSinceStartup;
            realtime = Time.realtimeSinceStartupAsDouble;
        }
        
        [MethodImpl(256)]
        public static void GetEditorTimeValues(out double time, out double realtime)
        {
            time = EditorApplication.timeSinceStartup;
            realtime = Time.realtimeSinceStartupAsDouble;
        }
#endif
        
        public static void GetTimeValues(out double time, out double unscaledTime, out double realtime)
        {
            time = Time.timeAsDouble;
            unscaledTime = Time.unscaledTimeAsDouble;
            realtime = Time.realtimeSinceStartupAsDouble;
        }
        
        public static void GetTimeValuesFixed(out double time, out double unscaledTime, out double realtime)
        {
            time = Time.fixedTimeAsDouble;
            unscaledTime = Time.fixedUnscaledTimeAsDouble;
            realtime = Time.realtimeSinceStartupAsDouble;
        }
        
        [MethodImpl(256)]
        public static void GetPlayerLoopTimeValues(PlayerLoopTiming timing, out double time, out double unscaledTime, out double realtime)
        {
            if (timing is PlayerLoopTiming.FixedUpdate)
            {
                GetTimeValuesFixed(out time, out unscaledTime, out realtime);
            }
            else
            {
                GetTimeValues(out time, out unscaledTime, out realtime);
            }
        }
    }
}