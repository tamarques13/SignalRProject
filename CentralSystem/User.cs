using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CentralSystem
{
    public class User
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }

        private static ConcurrentDictionary<string, User> _users = new();

        public static void AddUser(string connectionId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;

            }

            _users.TryAdd(connectionId, new User { ConnectionId = connectionId, Name = name });
        }

        public static IEnumerable<User> GetUsers() => _users.Values;
    }
}
