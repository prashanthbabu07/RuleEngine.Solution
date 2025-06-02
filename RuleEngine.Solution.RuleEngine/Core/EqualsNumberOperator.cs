namespace RuleEngine;

public class EqualsNumberOperator : IOperator<double>
{
    public string Name => "Equals";
    public object ConvertValue(object raw) => Convert.ToDouble(raw);
    public bool Evaluate(double actual, object expected) => actual == (double)expected;
}
