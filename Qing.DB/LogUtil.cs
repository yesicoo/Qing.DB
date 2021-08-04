using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    public static class SqlLogUtil
    {
        public static Action<LogType, string> PrintOut;
        public static decimal SqlLongTime = 0;
        internal static void SendLog(LogType Type, string Log, decimal interval = 0)
        {
            if (Type == LogType.SQL)
            {
                if (interval >= SqlLongTime)
                    PrintOut?.Invoke(Type, $"[{interval}] {Log}");
            }
            else
            {
                PrintOut?.Invoke(Type, Log);
            }
        }
    }
    public enum LogType
    {
        Error,
        SQL,
        Msg

    }
}
