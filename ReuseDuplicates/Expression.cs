namespace ReuseDuplicates;

public sealed record ConstantExpression(int Value) : IConstantExpression
{
    public override string ToString() => Value.ToString();
} 

public sealed record VariableExpression(string Name) : IVariableExpression
{
    public override string ToString() => Name;
}

public sealed record BinaryExpression : IBinaryExpression
{
    public IExpression Left { get; init; }
    public IExpression Right { get; init; }
    public OperatorSign Sign { get; init; }
    
    public BinaryExpression(IExpression Left, IExpression Right, OperatorSign Sign)
    {
        if ((Sign == OperatorSign.Plus || Sign == OperatorSign.Multiply) &&
            CompareExpressions(Left, Right) > 0)
        {
            (Left, Right) = (Right, Left);
        }
        
        this.Left = Left;
        this.Right = Right;
        this.Sign = Sign;
    }
    
    public override string ToString() => $"({Left} {Sign} {Right})";

    private static int CompareExpressions(IExpression a, IExpression b)
    {
        int typeCompare = a.GetType().Name.CompareTo(b.GetType().Name);
        if (typeCompare != 0) return typeCompare;
        
        switch (a)
        {
            case IConstantExpression cA:
                return cA.Value.CompareTo(((IConstantExpression)b).Value);
            case IVariableExpression vA:
                return vA.Name.CompareTo(((IVariableExpression)b).Name);
            case IBinaryExpression bA:
                return bA.GetHashCode().CompareTo(b.GetHashCode()); 
            case IFunction fA:
                return fA.GetHashCode().CompareTo(b.GetHashCode());
            default:
                return 0;
        }
    }
}

public sealed record Function(FunctionKind Kind, IExpression Argument) : IFunction
{
    public override string ToString() => $"{Kind}({Argument})";
}