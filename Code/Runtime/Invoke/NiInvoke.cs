using System;
using System.Runtime.CompilerServices;
using NiGames.Scheduling.Invoke;

namespace NiGames.Scheduling
{
    public static class NiInvoke
    {
        [MethodImpl(256)]
        public static NiInvokeBuilder Create(Action callback)
        {
            return NiInvokeBuilder.Create(callback);
        }
    }
}