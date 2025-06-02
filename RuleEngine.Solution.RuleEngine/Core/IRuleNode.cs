namespace RuleEngine;

public interface IRuleNode
{
    bool Evaluate(Dictionary<string, object> context, RuleEvaluator evaluator);
}
