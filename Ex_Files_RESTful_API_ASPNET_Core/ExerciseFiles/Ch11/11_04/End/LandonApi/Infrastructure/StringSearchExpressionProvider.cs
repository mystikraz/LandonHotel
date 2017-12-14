using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LandonApi.Infrastructure
{
    public class StringSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        private const string StartsWithOperator = "sw";
        private const string ContainsOperator = "co";

        private static readonly MethodInfo StartsWithMethod = typeof(string)
            .GetMethods()
            .First(m => m.Name == "StartsWith" && m.GetParameters().Length == 2);

        private static readonly MethodInfo StringEqualsMethod = typeof(string)
            .GetMethods()
            .First(m => m.Name == "Equals" && m.GetParameters().Length == 2);

        private static readonly MethodInfo ContainsMethod = typeof(string)
            .GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 1);

        private static readonly ConstantExpression IgnoreCase
            = Expression.Constant(StringComparison.OrdinalIgnoreCase);

        public override IEnumerable<string> GetOperators()
            => base.GetOperators()
            .Concat(new[]
            {
                StartsWithOperator,
                ContainsOperator
            });

        public override Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            switch (op.ToLower())
            {
                case StartsWithOperator:
                    return Expression.Call(left, StartsWithMethod, right, IgnoreCase);

                // TODO: This may or may not be case-insensitive, depending
                // on how your database translates Contains()
                case ContainsOperator:
                    return Expression.Call(left, ContainsMethod, right);

                // Handle the "eq" operator ourselves (with a case-insensitive compare)
                case EqualsOperator:
                    return Expression.Call(left, StringEqualsMethod, right, IgnoreCase);

                default: return base.GetComparison(left, op, right);
            }
        }
    }
}
