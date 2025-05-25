// See https://aka.ms/new-console-template for more information
//

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

        var expected = def.ValueType.IsInstanceOfType(rule.Value)
            ? rule.Value
            : def.ConvertValue(rule.Value);

        if (!context.TryGetValue(rule.PropertyKey, out var actualRaw))
            throw new Exception($"Missing value for property '{rule.PropertyKey}'");

        var actual = def.ValueType.IsInstanceOfType(actualRaw)
            ? actualRaw
            : def.ConvertValue(actualRaw);

        return def.Evaluate(rule.Operator, actual, expected);
    }
}

public interface IRuleNode
{
    bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator);
}

public interface IOperator<T>
{
    string Name { get; }
    bool Evaluate(T actual, T expected);
}

public class EqualsStringOperator : IOperator<string>
{
    public string Name => "Equals";
    public bool Evaluate(string actual, string expected) => actual == expected;
}

public class ContainsOperator : IOperator<string>
{
    public string Name => "Contains";
    public bool Evaluate(string actual, string expected) => actual.Contains(expected);
}

public class StartsWithOperator : IOperator<string>
{
    public string Name => "StartsWith";
    public bool Evaluate(string actual, string expected) => actual.StartsWith(expected);
}

public class EqualsNumberOperator : IOperator<double>
{
    public string Name => "Equals";
    public bool Evaluate(double actual, double expected) => actual == expected;
}

public class GreaterThanOperator : IOperator<double>
{
    public string Name => "GreaterThan";
    public bool Evaluate(double actual, double expected) => actual > expected;
}


public interface IDataType<T>
{
    string Name { get; }
    IEnumerable<string> GetSupportedOperators();
    bool Evaluate(string operatorName, T actual, T expected);
    T ConvertValue(object value);
}

public interface IPropertyDefinition
{
    Type ValueType { get; }
    object ConvertValue(object value);
    bool Evaluate(string operatorName, object actual, object expected);
}

public class DataType<T> : IDataType<T>
{
    public string Name { get; }
    private readonly Dictionary<string, IOperator<T>> _operators = new();

    public DataType(string name)
    {
        Name = name;
    }

    public void RegisterOperator(IOperator<T> op)
    {
        _operators[op.Name] = op;
    }

    public IEnumerable<string> GetSupportedOperators() => _operators.Keys;

    public bool Evaluate(string operatorName, T actual, T expected)
    {
        if (!_operators.TryGetValue(operatorName, out var op))
            throw new NotSupportedException($"Operator '{operatorName}' is not supported for data type '{Name}'");

        return op.Evaluate(actual, expected);
    }

    public T ConvertValue(object value) => (T)Convert.ChangeType(value, typeof(T));
}


public class PropertyDefinition<T> : IPropertyDefinition
{
    public string Key { get; }
    public IDataType<T> DataType { get; }

    public PropertyDefinition(string key, IDataType<T> dataType)
    {
        Key = key;
        DataType = dataType;
    }

    public Type ValueType => typeof(T);

    public object ConvertValue(object value) => DataType.ConvertValue(value);

    public bool Evaluate(string op, object actual, object expected)
    {
        return DataType.Evaluate(op, (T)actual, (T)expected);
    }
}

public class PropertyRegistry
{
    private readonly Dictionary<string, IPropertyDefinition> _definitions = new();

    public void Register<T>(string key, IDataType<T> dataType)
    {
        _definitions[key] = new PropertyDefinition<T>(key, dataType);
    }

    public IPropertyDefinition Get(string key)
    {
        if (!_definitions.TryGetValue(key, out var def))
            throw new KeyNotFoundException($"Property '{key}' is not registered.");
        return def;
    }
}

public class SimpleRule : IRuleNode
{
    public string PropertyKey { get; set; } = default!;
    public string Operator { get; set; } = default!;
    public object Value { get; set; } = default!;

    public bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator)
    {
        return evaluator.Evaluate(this, context);
    }
}

public enum LogicalOperator
{
    And,
    Or,
}

public class CompositeRule : IRuleNode
{
    public LogicalOperator Operator { get; set; }
    public List<IRuleNode> Children { get; set; } = new();

    public bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator)
    {
        return Operator switch
        {
            LogicalOperator.And => Children.All(child => child.Evaluate(context, evaluator)),
            LogicalOperator.Or => Children.Any(child => child.Evaluate(context, evaluator)),
            _ => throw new NotSupportedException($"Logical operator '{Operator}' is not supported"),
        };
    }
}

public class Program
{
    public static void Main()
    {
        var registry = new PropertyRegistry();
        // registry.Register<double>("age", new NumberDataType());
        // registry.Register<string>("country", new StringDataType());

        var stringType = new DataType<string>("String");
        stringType.RegisterOperator(new EqualsStringOperator());
        stringType.RegisterOperator(new ContainsOperator());
        stringType.RegisterOperator(new StartsWithOperator());

        var numberType = new DataType<double>("Number");
        numberType.RegisterOperator(new EqualsNumberOperator());
        numberType.RegisterOperator(new GreaterThanOperator());

        registry.Register("age", numberType);
        registry.Register("country", stringType);


        var rule1 = new SimpleRule
        {
            PropertyKey = "age",
            Operator = "GreaterThan",
            Value = 25,
        };
        var rule2 = new SimpleRule
        {
            PropertyKey = "country",
            Operator = "StartsWith",
            Value = "U",
        };

        var rootRule = new CompositeRule
        {
            Operator = LogicalOperator.And,
            Children = new List<IRuleNode> { rule1, rule2 },
        };

        var context = new Dictionary<string, object> { ["age"] = 30, ["country"] = "USA" };

        var evaluator = new RuleEvaluator(registry);
        bool result = rootRule.Evaluate(context, evaluator); // true
        Console.WriteLine(result);
        Console.ReadLine();
    }
}
