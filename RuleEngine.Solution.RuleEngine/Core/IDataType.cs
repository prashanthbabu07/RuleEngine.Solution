namespace RuleEngine;

public interface IDataType<T>
{
    string Name { get; }
    IEnumerable<string> GetSupportedOperators();
    IOperator<T> GetOperator(string name);
}
