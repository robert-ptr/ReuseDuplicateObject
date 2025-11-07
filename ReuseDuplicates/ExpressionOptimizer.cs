namespace ReuseDuplicates;

public class ExpressionOptimizer
{
    private HashSet<IExpression> savedExpressions = new HashSet<IExpression>();

    public IExpression Optimize(IExpression node)
    {
        savedExpressions.Clear();

        return OptimizeRecursive(node);
    }

    private IExpression OptimizeRecursive(IExpression node)
    {
        IExpression newNode;
        
        switch (node)
        {
            case IConstantExpression:
            case IVariableExpression:
                newNode = node;
                break;
            case IBinaryExpression expr:
                var left = OptimizeRecursive(expr.Left);
                var right = OptimizeRecursive(expr.Right);

                if (left == expr.Left && right == expr.Right) // nothing changed
                    newNode = expr;
                else
                    newNode = new BinaryExpression(left, right, expr.Sign);
                break;
            case IFunction func:
                var arg = OptimizeRecursive(func.Argument);
                if (arg == func.Argument)
                    newNode = func;
                else
                    newNode = new Function(func.Kind, arg);
                break;
            default:
                throw new Exception("Unknown expression type!");
        }
        
        if (savedExpressions.TryGetValue(newNode, out var existingNode))
        {
            return existingNode;
        }

        savedExpressions.Add(newNode);
        return newNode;
    }
}