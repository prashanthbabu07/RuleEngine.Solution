namespace RuleEngine;

public interface IRuleNodeVisitor<T>
{
    T Visit(SimpleRule r);
    T Visit(CompositeRule r);
}

public class SqlPredicateResult
{
    public required string Sql { get; init; }
    public Dictionary<string, object> Parameters { get; init; } = new();
}

public class SqlPredicateGenerator : IRuleNodeVisitor<List<SqlPredicateResult>>
{
    private int _counter = 0;

    public List<SqlPredicateResult> Visit(SimpleRule r)
    {
        // implement this to return sql predicate based on the SimpleRule
        var sql = $"{r.PropertyKey} {r.Operator} @p{_counter}";
        _counter++;
        var p = new SqlPredicateResult
        {
            Sql = sql,
            Parameters = { { $"p{_counter - 1}", r.Value } },
        };
        return new List<SqlPredicateResult> { p };
    }

    public List<SqlPredicateResult> Visit(CompositeRule r)
    {
        var predicates = new List<SqlPredicateResult>();
        foreach (var child in r.Children)
        {
            var result = child.Accept(this);
            predicates.AddRange(result);
        }
        return predicates;
    }
}
