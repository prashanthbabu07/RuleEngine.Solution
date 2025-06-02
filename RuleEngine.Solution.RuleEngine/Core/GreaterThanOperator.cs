namespace RuleEngine;

public class GreaterThanOperator : IOperator<double>
{
    public string Name => "GreaterThan";
    public object ConvertValue(object raw) => Convert.ToDouble(raw);
    public bool Evaluate(double actual, object expected) => actual > (double)expected;
}
