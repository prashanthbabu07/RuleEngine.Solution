// See https://aka.ms/new-console-template for more information

using System.Text.Json;

namespace RuleEngine;

public class Program
{
    public static void Main()
    {
        var propertyRegistry = new PropertyRegistry();

        var stringType = new DataType<string>("String");
        stringType.RegisterOperator(new EqualsStringOperator());
        stringType.RegisterOperator(new ContainsOperator());
        stringType.RegisterOperator(new StartsWithOperator());

        var numberType = new DataType<double>("Number");
        numberType.RegisterOperator(new EqualsNumberOperator());
        numberType.RegisterOperator(new GreaterThanOperator());

        propertyRegistry.Register("country", stringType);
        propertyRegistry.Register("age", numberType);

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

        // convert to sql 
        // where country like 'U%' and (age > 30 or age = 25)
        // where country like @p1 and (age > @p2 or age = @p3) -- parameterized query

        var context = new Dictionary<string, object>
        {
            ["country"] = "India",
            ["age"] = 25 
        };

        var evaluator = new RuleEvaluator(propertyRegistry);
        var result = evaluator.Evaluate(complexRule, context);
        // var result = complexRule.Evaluate(context, evaluator); // returns true
        Console.WriteLine($"Rule result: {result}");

        // convert complexRule to json for console output
        // this has nested rules

        // var json = JsonSerializer.Serialize(complexRule, new System.Text.Json.JsonSerializerOptions
        // {
        //     WriteIndented = true,
        //     PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        // });
        // Console.WriteLine($"Complex Rule JSON: {json}");

        var sqlGenerator = new SqlPredicateGenerator();
        var sqlPredicates = complexRule.Accept(sqlGenerator);
        foreach (var predicate in sqlPredicates)
        {
            Console.WriteLine($"SQL: {predicate.Sql} {string.Join(", ", predicate.Parameters.Select(p => $"{p.Key} = {p.Value}"))}");
            // foreach (var param in predicate.Parameters)
            // {
            //     Console.WriteLine($"Parameter: {param.Key} = {param.Value}");
            // }
        }
    }
}
