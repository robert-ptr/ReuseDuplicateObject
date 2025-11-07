using System;

namespace ReuseDuplicates;

public class Program
{
    public static void Main()
    {
        var x1 = new VariableExpression("x");
        var x2 = new VariableExpression("x");
        
        var subExpr1 = new BinaryExpression(
            new ConstantExpression(7),
            new BinaryExpression(new ConstantExpression(2), x1, OperatorSign.Plus),
            OperatorSign.Multiply
        );

        var subExpr2 = new BinaryExpression(
            new ConstantExpression(7),
            new BinaryExpression(x2, new ConstantExpression(2), OperatorSign.Plus),
            OperatorSign.Multiply
        );

        var originalExpr = new BinaryExpression(
            new BinaryExpression(
                new Function(FunctionKind.Sin, subExpr1),
                subExpr2,
                OperatorSign.Minus
            ),
            new Function(FunctionKind.Cos, x2), 
            OperatorSign.Plus
        );

        Console.WriteLine("Original Expression:");
        Console.WriteLine(originalExpr);
        Console.WriteLine("---");
        
        var originalSinArg = ((originalExpr.Left as IBinaryExpression).Left as IFunction).Argument;
        var originalSubArg = (originalExpr.Left as IBinaryExpression).Right;
        var originalCosArg = (originalExpr.Right as IFunction).Argument;

        Console.WriteLine("Are the two (7 * (2 + x)) instances the same object? " +
            $"{Object.ReferenceEquals(originalSinArg, originalSubArg)}");

        Console.WriteLine("Are the two (2 + x) instances the same object? " +
                          $"{Object.ReferenceEquals((originalSinArg as IBinaryExpression).Right, 
                              (originalSubArg as IBinaryExpression).Right)}");
        
        Console.WriteLine("Are the two 'x' instances the same object? " +
            $"{Object.ReferenceEquals(x1, x2)}");
        
        Console.WriteLine("---");
        
        var optimizer = new ExpressionOptimizer();
        var optimizedExpr = optimizer.Optimize(originalExpr);

        Console.WriteLine("Optimized Expression:");
        Console.WriteLine(optimizedExpr);
        Console.WriteLine("---");

        var optimizedSinArg = (((optimizedExpr as IBinaryExpression).Left as IBinaryExpression).Left as IFunction).Argument;
        var optimizedSubArg = ((optimizedExpr as IBinaryExpression).Left as IBinaryExpression).Right;
        var optimizedCosArg = ((optimizedExpr as IBinaryExpression).Right as IFunction).Argument;

        Console.WriteLine("Are the two (7 * (2 + x)) instances the same object? " +
            $"{Object.ReferenceEquals(optimizedSinArg, optimizedSubArg)}");
        
        Console.WriteLine("Are the two (2 + x) instances the same object? " +
                          $"{Object.ReferenceEquals((optimizedSinArg as IBinaryExpression).Right, 
                                                    (optimizedSubArg as IBinaryExpression).Right)}");
        
        var sin_arg_binary = optimizedSinArg as IBinaryExpression;
        var two_plus_x = sin_arg_binary.Left as IBinaryExpression;
        var x_in_sub = two_plus_x.Right; 

        Console.WriteLine("Are the 'x' in (2 + x) and the 'x' in cos(x) the same object? " +
                          $"{Object.ReferenceEquals(x_in_sub, optimizedCosArg)}");
    }
}