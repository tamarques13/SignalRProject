using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;

namespace CentralSystem
{
    public class User
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }

        private static ConcurrentDictionary<string, User> Users = new();

        public static void AddUser(string connectionId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;

            }

            Users.TryAdd(connectionId, new User { ConnectionId = connectionId, Name = name });
        }

        public static IEnumerable<User> GetUsers() => Users.Values;
    }
}
