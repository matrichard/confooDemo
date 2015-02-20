using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace ConfooDemo
{
    public class OptionHub : Hub
    {
        public void IncrementOption(string option)
        {
            var optionCount = OptionIncrementer.Increment(option);
            if (optionCount > 0)
            {
                Clients.All.updateOption(new { option, optionCount });
            }
        }
    }

    public static class OptionIncrementer
    {
        private static readonly ConcurrentDictionary<string, int> Cache =
            new ConcurrentDictionary<string, int>(new[]
            {
                new KeyValuePair<string, int>("a", 0),
                new KeyValuePair<string, int>("b", 0),
                new KeyValuePair<string, int>("c", 0)
            });

        public static int Increment(string option)
        {
            int optionCount;
            if (!Cache.TryGetValue(option, out optionCount))
            {
                return optionCount;
            }

            ++optionCount;
            Cache.AddOrUpdate(option, s => 0, (s, i) => optionCount);

            return optionCount;
        }

        public static int Count(string option)
        {
            int optionCount;
            Cache.TryGetValue(option, out optionCount);
            return optionCount;
        }

    }
}