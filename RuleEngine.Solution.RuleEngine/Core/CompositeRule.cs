namespace RuleEngine;

public class CompositeRule : IRuleNode
{
    public LogicalOperator Operator { get; set; }
    public List<IRuleNode> Children { get; set; } = new();

    public bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator)
    {
        return Operator switch
        {
            LogicalOperator.And => Children.All(c => c.Evaluate(context, evaluator)),
            LogicalOperator.Or => Children.Any(c => c.Evaluate(context, evaluator)),
            _ => throw new NotSupportedException($"Unknown logical operator: {Operator}")
        };
    }
}
