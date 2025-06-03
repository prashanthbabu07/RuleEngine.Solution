namespace RuleEngine;

public class RuleEvaluator
{
    private readonly PropertyRegistry _registry;

    public RuleEvaluator(PropertyRegistry registry)
    {
        _registry = registry;
    }

    public bool Evaluate(SimpleRule rule, Dictionary<string, object> context)
    {
        var def = _registry.Get(rule.PropertyKey);

        if (!context.TryGetValue(rule.PropertyKey, out var actualRaw))
            throw new Exception($"Missing context value for '{rule.PropertyKey}'");

        var actual = def.ConvertActual(actualRaw);
        var expected = def.ConvertExpected(rule.Operator, rule.Value);

        return def.Evaluate(rule.Operator, actual, expected);
    }
}
