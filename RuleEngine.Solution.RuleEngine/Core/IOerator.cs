namespace RuleEngine;

public interface IOperator<T>
{
    string Name { get; }
    object ConvertValue(object raw);
    bool Evaluate(T actual, object expected);
}
