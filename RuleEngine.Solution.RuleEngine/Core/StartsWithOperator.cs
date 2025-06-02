namespace RuleEngine;

public class StartsWithOperator : IOperator<string>
{
    public string Name => "StartsWith";
    public object ConvertValue(object raw) => raw?.ToString() ?? "";
    public bool Evaluate(string actual, object expected) => actual.StartsWith((string)expected);
}
