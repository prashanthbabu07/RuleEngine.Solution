// See https://aka.ms/new-console-template for more information

namespace RuleEngine;


public enum LogicalOperator
{
    And,
    Or
}

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



public class Program
{
    public static void Main()
    {
        var registry = new PropertyRegistry();

        var stringType = new DataType<string>("String");
        stringType.RegisterOperator(new EqualsStringOperator());
        stringType.RegisterOperator(new ContainsOperator());
        stringType.RegisterOperator(new StartsWithOperator());

        var numberType = new DataType<double>("Number");
        numberType.RegisterOperator(new EqualsNumberOperator());
        numberType.RegisterOperator(new GreaterThanOperator());

        registry.Register("country", stringType);
        registry.Register("age", numberType);

        var complexRule = new CompositeRule
        {
            Operator = LogicalOperator.And,
            Children = new List<IRuleNode>
            {
                new SimpleRule
                {
                    PropertyKey = "country",
                    Operator = "StartsWith",
                    Value = "U"
                },
                new CompositeRule
                {
                    Operator = LogicalOperator.Or,
                    Children = new List<IRuleNode>
                    {
                        new SimpleRule
                        {
                            PropertyKey = "age",
                            Operator = "GreaterThan",
                            Value = 30
                        },
                        new SimpleRule
                        {
                            PropertyKey = "age",
                            Operator = "Equals",
                            Value = 25
                        }
                    }
                }
            }
        };

        var context = new Dictionary<string, object>
        {
            ["country"] = "USA",
            ["age"] = 5
        };

        var evaluator = new RuleEvaluator(registry);
        var result = complexRule.Evaluate(context, evaluator); // returns true
        Console.WriteLine($"Rule result: {result}");
        // Console.ReadLine();

    }
}
