namespace FarmManagement.Web.Services;

// Singleton Pattern — one shared in-memory cache instance for the entire application.
// Uses Lazy<T> for thread-safe lazy initialisation without explicit locking.
// Registered as AddSingleton in Program.cs so the DI container manages the lifetime.
public class FarmCacheService
{
    // Classic GoF Singleton — one instance, created on first access, thread-safe via Lazy<T>
    private static readonly Lazy<FarmCacheService> _instance =
        new(() => new FarmCacheService(), isThreadSafe: true);

    public static FarmCacheService Instance => _instance.Value;

    private readonly Dictionary<string, (object Value, DateTime ExpiresAt)> _store = new();
    private readonly object _lock = new();

    // Private constructor — prevents external instantiation
    private FarmCacheService() { }

    public void Set(string key, object value, TimeSpan duration)
    {
        lock (_lock)
        {
            _store[key] = (value, DateTime.UtcNow.Add(duration));
        }
    }

    public bool TryGet<T>(string key, out T? value)
    {
        lock (_lock)
        {
            if (_store.TryGetValue(key, out var entry) && entry.ExpiresAt > DateTime.UtcNow)
            {
                value = (T)entry.Value;
                return true;
            }

            _store.Remove(key);
            value = default;
            return false;
        }
    }

    public void Invalidate(string key)
    {
        lock (_lock) { _store.Remove(key); }
    }

    public void InvalidateAll()
    {
        lock (_lock) { _store.Clear(); }
    }
}
