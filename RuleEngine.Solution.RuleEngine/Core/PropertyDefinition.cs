namespace RuleEngine;

public class PropertyDefinition<T> : IPropertyDefinition
{
    public IDataType<T> DataType { get; }

    public PropertyDefinition(IDataType<T> dataType) => DataType = dataType;

    public Type ValueType => typeof(T);

    public object ConvertActual(object actual) => (T)Convert.ChangeType(actual, typeof(T));

    public object ConvertExpected(string operatorName, object value)
    {
        var op = DataType.GetOperator(operatorName);
        return op.ConvertValue(value);
    }

    public bool Evaluate(string operatorName, object actual, object expected)
    {
        var op = DataType.GetOperator(operatorName);
        return op.Evaluate((T)actual, expected);
    }
}
