namespace RuleEngine;

public class PropertyRegistry
{
    private readonly Dictionary<string, IPropertyDefinition> _map = new();
    public void Register<T>(string key, IDataType<T> dataType)
        => _map[key] = new PropertyDefinition<T>(dataType);

    public IPropertyDefinition Get(string key)
    {
        if (!_map.TryGetValue(key, out var def))
        {
            throw new KeyNotFoundException($"Property '{key}' not registered.");
        }
        return def;
    }
}
