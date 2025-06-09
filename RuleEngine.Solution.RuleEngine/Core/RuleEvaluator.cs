namespace RuleEngine;

public class RuleEvaluator
{
    private readonly PropertyRegistry _registry;

    public RuleEvaluator(PropertyRegistry registry)
    {
        _registry = registry;
    }

    public bool Evaluate(IRuleNode rule, Dictionary<string, object> context)
    {
        return rule switch
        {
            SimpleRule simple => EvaluateSimple(simple, context),
            CompositeRule composite => EvaluateComposite(composite, context),
            _ => throw new NotSupportedException("Unknown rule type")
        };
    }

    private bool EvaluateComposite(CompositeRule rule, Dictionary<string, object> context)
    {
        return rule.Operator switch
        {
            LogicalOperator.And => rule.Children.All(child => Evaluate(child, context)),
            LogicalOperator.Or => rule.Children.Any(child => Evaluate(child, context)),
            _ => throw new NotSupportedException($"Unsupported logical operator: {rule.Operator}")
        };
    }


    private bool EvaluateSimple(SimpleRule rule, Dictionary<string, object> context)
    {
        var def = _registry.Get(rule.PropertyKey);

        if (!context.TryGetValue(rule.PropertyKey, out var actualRaw))
        {
            throw new Exception($"Missing context value for '{rule.PropertyKey}'");
        }

        var actual = def.ConvertActual(actualRaw);
        var expected = def.ConvertExpected(rule.Operator, rule.Value);

        return def.Evaluate(rule.Operator, actual, expected);
    }
}
