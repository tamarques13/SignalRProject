using System.Collections.Concurrent;

namespace CentralSystem
{
    public class Group
    {
        private static ConcurrentDictionary<Guid, string> Groups = new();

        public static void AddGroup(string groupName)
        {
            if (Groups.Values.ToList().Any(g => g.Contains(groupName)))
                return;

            Groups.TryAdd(Guid.NewGuid(), groupName);
        }

        public static IEnumerable<string> GetGroups() => Groups.Values;
    }
}