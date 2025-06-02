namespace RuleEngine;

public class ContainsOperator : IOperator<string>
{
    public string Name => "Contains";
    public object ConvertValue(object raw) => raw?.ToString() ?? "";
    public bool Evaluate(string actual, object expected) => actual.Contains((string)expected);
}
