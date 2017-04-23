using System;
using System.Collections.Generic;
using System.Linq;

namespace Hermes.Services.Helpers.Collections
{
    public class CachedDictionary<TKey, TValue>
    {
        private struct CachedEntry
        {
            public readonly DateTime EntryCreation;
            public readonly TValue EntryValue;

            public CachedEntry(TValue entryValue)
            {
                EntryCreation = DateTime.UtcNow;
                EntryValue = entryValue;
            }
        }

        private readonly Dictionary<TKey, CachedEntry> _cache;
        private DateTime? _lastFullReset = null;

        public TimeSpan CacheEntryRefreshDelay { get; set; }
        public TimeSpan CacheFullResetDelay { get; set; }

        public Func<Dictionary<TKey, TValue>> GetAllCallback { get; set; }
        public Func<TKey, TValue> GetByKeyCallback { get; set; }

        private readonly object _lockObj;

        public CachedDictionary()
        {
            _cache = new Dictionary<TKey, CachedEntry>();
            _lockObj = new object();

            CacheEntryRefreshDelay = TimeSpan.FromMinutes(60);
            CacheFullResetDelay = TimeSpan.FromMinutes(60);
        }

        public void Add(TKey key, TValue value)
        {
            lock (_lockObj)
                _cache[key] = new CachedEntry(value);
        }

        public TValue GetByKey(TKey key)
        {
            lock (_lockObj)
            {
                if (GetAllCallback != null && (_lastFullReset == null || DateTime.UtcNow - _lastFullReset.Value >= CacheFullResetDelay))
                    ResetFullCache();

                CachedEntry entry;
                bool found = false;

                // Key is in the cache
                if (_cache.TryGetValue(key, out entry))
                {
                    // Entry is not too old or there is no way to refresh an entry
                    if (GetByKeyCallback == null || DateTime.UtcNow - entry.EntryCreation < CacheEntryRefreshDelay)
                        return entry.EntryValue;

                    found = true;
                }

                // We can fetch a particular value
                if (GetByKeyCallback != null)
                {
                    // Remove old entry if any
                    if (found)
                        _cache.Remove(key);

                    try
                    {
                        TValue value = GetByKeyCallback(key);
                        if (value == null)
                            return default(TValue);

                        _cache[key] = new CachedEntry(value);
                        return value;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                return default(TValue);
            }
        }

        public List<TKey> GetAllKeys(Func<TKey, bool> keySelector = null)
        {
            lock (_lockObj)
            {
                if (GetAllCallback != null && (_lastFullReset == null || DateTime.UtcNow - _lastFullReset.Value >= CacheFullResetDelay))
                    ResetFullCache();

                List<TKey> keys = keySelector != null ? _cache.Keys.Where(keySelector).ToList() : _cache.Keys.ToList();
                return keys;
            }
        }

        public List<TValue> GetAllValues(Func<TKey, bool> keySelector = null)
        {
            lock (_lockObj)
            {
                if (GetAllCallback != null && (_lastFullReset == null || DateTime.UtcNow - _lastFullReset.Value >= CacheFullResetDelay))
                    ResetFullCache();

                List<TKey> keys = keySelector != null ? _cache.Keys.Where(keySelector).ToList() : _cache.Keys.ToList();
                return keys.Select(key => _cache[key].EntryValue).ToList();
            }
        }

        private void ResetFullCache()
        {

            try
            {
                Dictionary<TKey, TValue> values = GetAllCallback();

                // Reset the cache only if we were able to get the new values
                _cache.Clear();

                foreach (KeyValuePair<TKey, TValue> pair in values)
                    _cache.Add(pair.Key, new CachedEntry(pair.Value));
            }
            catch (Exception)
            {
                // ignored
            }

            _lastFullReset = DateTime.UtcNow;
        }
    }
}
