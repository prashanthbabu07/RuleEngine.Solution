namespace RuleEngine;


public interface IRuleNode
{
    // bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator);
    T Accept<T>(IRuleNodeVisitor<T> visitor);
}
