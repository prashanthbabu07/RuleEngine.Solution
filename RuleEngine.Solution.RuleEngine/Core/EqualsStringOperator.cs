namespace RuleEngine;

public class EqualsStringOperator : IOperator<string>
{
    public string Name => "Equals";
    public object ConvertValue(object raw) => raw?.ToString() ?? "";
    public bool Evaluate(string actual, object expected) => actual == (string)expected;
}
