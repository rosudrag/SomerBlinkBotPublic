using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.TimeHandler;
using Core.Logging;

namespace Core.Time
{
    public static class TimedActionRunner
    {
        /// <summary>
        /// Runs the action returning the time it took.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static TimeSpan RunAction(Action action)
        {
            var begin = DateTime.Now;

            action.Invoke();

            var end = DateTime.Now;

            return end - begin;

        }

        /// <summary>
        /// Runs the action with random wait.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static TimeSpan RunActionWithRandomWait(Action action)
        {
            var begin = DateTime.Now;

            action.Invoke();

            var timeToSleep = WaitTimeGenerator.RandomiseTime();

            Logger.LogMessage("Sleeping for: {0} ms", timeToSleep);

            Thread.Sleep(timeToSleep);

            var end = DateTime.Now;

            return end - begin;
        }
    }
}
