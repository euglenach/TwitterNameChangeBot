using System;
using System.Collections.Generic;
using System.Linq;
using CoreTweet;

namespace TwitterNameBot {
    public static class Extension {
        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this Dictionary<TKey, TValue> self) {
            foreach (var item in self.Keys) {
                yield return item;
            }
        }
        
        public static T Random<T>(this IEnumerable<T> self) {
            if (!self.Any()) {
                return default;
            }
            Random random = new Random();
            var idx = random.Next(0, self.Count());
            return self.ElementAt(idx);
        }

        // public static IEnumerable<Status> GetNotFavoTweets(this IEnumerable<Status> tweets,Tokens token) =>
        //     tweets.Where(tweet => (bool)!token.Statuses.Show(id => tweet.Id).IsFavorited);
            
    }
}
