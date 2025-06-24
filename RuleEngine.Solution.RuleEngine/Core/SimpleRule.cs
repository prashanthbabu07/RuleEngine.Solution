namespace RuleEngine;

public class SimpleRule : IRuleNode
{
    public string PropertyKey { get; set; } = default!;
    public string Operator { get; set; } = default!;
    public object Value { get; set; } = default!;

    public T Accept<T>(IRuleNodeVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
