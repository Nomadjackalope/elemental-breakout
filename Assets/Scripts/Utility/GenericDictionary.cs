using System.Collections.Generic;

public class GenericDictionary
{
    private Dictionary<string, object> _dict = new Dictionary<string, object>();

    public void Add<T>(string key, T value) where T : class
    {
        _dict.Add(key, value);
    }

    public bool Remove(string key)
    {
        return _dict.Remove(key);
    }

    public T GetValue<T>(string key) where T : class
    {
        return _dict[key] as T;
    }
}