using System.Collections.Concurrent;

public class UserData
{
    // Originally used Dictionary which Copilot identified not thread safe.
    // This is a class representing the data storage for users, which could be extended
    //.    to use a database for more realistic scenarios.
    public static ConcurrentDictionary<int, User> Users { get; set; } = new ConcurrentDictionary<int, User>();
}