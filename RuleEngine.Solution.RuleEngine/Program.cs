// See https://aka.ms/new-console-template for more information

namespace RuleEngine;


public interface IDataType<T>
{
    string Name { get; }
    IEnumerable<string> GetSupportedOperators();
    IOperator<T> GetOperator(string name);
}

public class DataType<T> : IDataType<T>
{
    public string Name { get; }
    private readonly Dictionary<string, IOperator<T>> _operators = new();

    public DataType(string name) => Name = name;

    public void RegisterOperator(IOperator<T> op) => _operators[op.Name] = op;

    public IEnumerable<string> GetSupportedOperators() => _operators.Keys;

    public IOperator<T> GetOperator(string name)
    {
        if (!_operators.TryGetValue(name, out var op))
            throw new NotSupportedException($"Operator '{name}' not found for type '{Name}'");
        return op;
    }
}

public interface IPropertyDefinition
{
    Type ValueType { get; }
    object ConvertActual(object actual);
    object ConvertExpected(string operatorName, object value);
    bool Evaluate(string operatorName, object actual, object expected);
}

public class PropertyDefinition<T> : IPropertyDefinition
{
    public IDataType<T> DataType { get; }

    public PropertyDefinition(IDataType<T> dataType) => DataType = dataType;

    public Type ValueType => typeof(T);

    public object ConvertActual(object actual) => (T)Convert.ChangeType(actual, typeof(T));

    public object ConvertExpected(string operatorName, object value)
    {
        var op = DataType.GetOperator(operatorName);
        return op.ConvertValue(value);
    }

    public bool Evaluate(string operatorName, object actual, object expected)
    {
        var op = DataType.GetOperator(operatorName);
        return op.Evaluate((T)actual, expected);
    }
}

public class PropertyRegistry
{
    private readonly Dictionary<string, IPropertyDefinition> _map = new();
    public void Register<T>(string key, IDataType<T> dataType)
        => _map[key] = new PropertyDefinition<T>(dataType);

    public IPropertyDefinition Get(string key)
    {
        if (!_map.TryGetValue(key, out var def))
            throw new KeyNotFoundException($"Property '{key}' not registered.");
        return def;
    }
}

public class SimpleRule : IRuleNode
{
    public string PropertyKey { get; set; } = default!;
    public string Operator { get; set; } = default!;
    public object Value { get; set; } = default!;

    public bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator)
        => evaluator.Evaluate(this, context);
}

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
