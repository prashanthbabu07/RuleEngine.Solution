﻿// See https://aka.ms/new-console-template for more information

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

        var context = new Dictionary<string, object>
        {
            ["country"] = "India",
            ["age"] = 25 
        };

        var evaluator = new RuleEvaluator(propertyRegistry);
        var result = evaluator.Evaluate(complexRule, context);
        // var result = complexRule.Evaluate(context, evaluator); // returns true
        Console.WriteLine($"Rule result: {result}");
        // Console.ReadLine();

    }
}
