namespace RuleEngine;

public class SimpleRule : IRuleNode
{
    public string PropertyKey { get; set; } = default!;
    public string Operator { get; set; } = default!;
    public object Value { get; set; } = default!;

    public bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator)
        => evaluator.Evaluate(this, context);
}
