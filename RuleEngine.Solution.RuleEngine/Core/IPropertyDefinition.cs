namespace RuleEngine;

public interface IPropertyDefinition
{
    Type ValueType { get; }
    object ConvertActual(object actual);
    object ConvertExpected(string operatorName, object value);
    bool Evaluate(string operatorName, object actual, object expected);
}
