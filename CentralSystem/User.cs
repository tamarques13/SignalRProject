using System.Collections.Concurrent;

namespace CentralSystem
{
    public class User
    {
        // Doubt: Best Practices to handle non-nullable properties
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string Group {  get; set; }
        private static ConcurrentDictionary<string, User> Users = new();

        public static void AddUser(string connectionId, string name, string groupName)
        {
            Users.TryAdd(connectionId, new User { ConnectionId = connectionId, Name = name, Group = groupName });
        }

        public static void RemoveUser(string connectId)
        {
            Users.TryRemove(connectId, out _);
        }

        public static IEnumerable<User> GetUsers() => Users.Values;
    }
}
