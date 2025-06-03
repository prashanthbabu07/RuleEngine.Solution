namespace RuleEngine;

public class DataType<T> : IDataType<T>
{
    public string Name { get; }
    private readonly Dictionary<string, IOperator<T>> _operators = new();

    public DataType(string name) => Name = name;

    public void RegisterOperator(IOperator<T> op) => _operators[op.Name] = op;

    public IEnumerable<string> GetSupportedOperators() => _operators.Keys;

    public IOperator<T> GetOperator(string name)
    {
        if (!_operators.TryGetValue(name, out var op))
        {
            throw new NotSupportedException($"Operator '{name}' not found for type '{Name}'");
        }
        return op;
    }
}
