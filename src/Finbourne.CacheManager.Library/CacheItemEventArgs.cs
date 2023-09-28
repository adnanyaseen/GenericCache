namespace Finbourne.CacheManager.Library
{
    public class CacheItemEventArgs : EventArgs
    {
        public string Key { get; }

        public CacheItemEventArgs(string key)
        {
            Key = key;
        }
    }
}
