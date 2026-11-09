using Antlr4.Runtime;
using System;

namespace Lab1Final
{
    public static class Calculator
    {
        public static double Evaluate(string expression, Func<string, double> cellValueProvider)
        {
            var lexer = new LabCalculatorLexer(CharStreams.fromString(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new LabCalculatorParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowExceptionErrorListener());

            var tree = parser.compileUnit();
            var visitor = new LabCalculatorVisitor(cellValueProvider);

            return visitor.Visit(tree);
        }
    }
}