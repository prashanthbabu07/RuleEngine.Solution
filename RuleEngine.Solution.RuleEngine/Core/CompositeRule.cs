namespace RuleEngine;

public class CompositeRule : IRuleNode
{
    public LogicalOperator Operator { get; set; }
    public List<IRuleNode> Children { get; set; } = new();

    public T Accept<T>(IRuleNodeVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
