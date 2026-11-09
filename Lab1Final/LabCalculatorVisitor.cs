using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1Final
{
    class LabCalculatorVisitor : LabCalculatorBaseVisitor<double>
    {
        private readonly Func<string, double> _cellValueProvider;

        public LabCalculatorVisitor(Func<string, double> cellValueProvider)
        {
            _cellValueProvider = cellValueProvider;
        }

        public override double VisitCompileUnit(LabCalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(LabCalculatorParser.NumberExprContext context)
        {
            return double.Parse(context.GetText(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public override double VisitIdentifierExpr(LabCalculatorParser.IdentifierExprContext context)
        {
            var cellName = context.GetText().ToUpper();
            return _cellValueProvider(cellName);
        }

        public override double VisitParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitUnaryExpr(LabCalculatorParser.UnaryExprContext context)
        {
            var value = Visit(context.expression());
            if (context.operatorToken.Type == LabCalculatorLexer.SUBTRACT)
                return -value;
            return value;
        }

        public override double VisitNotExpr(LabCalculatorParser.NotExprContext context)
        {
            var value = Visit(context.expression());
            return value == 0.0 ? 1.0 : 0.0;
        }

        public override double VisitMmaxExpr(LabCalculatorParser.MmaxExprContext context)
        {
            var expressions = context.expression();
            return expressions.Select(Visit).Max();
        }

        public override double VisitMminExpr(LabCalculatorParser.MminExprContext context)
        {
            var expressions = context.expression();
            return expressions.Select(Visit).Min();
        }

        public override double VisitMultiplicativeExpr(LabCalculatorParser.MultiplicativeExprContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            if (context.operatorToken.Type == LabCalculatorLexer.MULTIPLY)
                return left * right;

            if (right == 0.0) throw new DivideByZeroException("Ділення на нуль");
            return left / right;
        }

        public override double VisitAdditiveExpr(LabCalculatorParser.AdditiveExprContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            if (context.operatorToken.Type == LabCalculatorLexer.ADD)
                return left + right;
            return left - right;
        }

        public override double VisitRelationalExpr(LabCalculatorParser.RelationalExprContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            bool result = false;
            switch (context.operatorToken.Type)
            {
                case LabCalculatorLexer.EQUAL: result = left == right; break;
                case LabCalculatorLexer.LESS: result = left < right; break;
                case LabCalculatorLexer.GREATER: result = left > right; break;
            }
            return result ? 1.0 : 0.0;
        }
    }
}