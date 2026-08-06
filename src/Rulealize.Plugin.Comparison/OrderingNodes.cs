// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Collections.Immutable;
using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Evaluation;
using Rulealize.Abstraction.Nodes;
using Rulealize.Abstraction.Values;

namespace Rulealize.Plugin.Comparison
{
    /// <summary>Which side of zero the comparison result has to fall on.</summary>
    internal enum OrderingRelation
    {
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    /// <summary>The four ordering predicates, which differ only in the test they apply.</summary>
    /// <remarks>
    /// Unlike equality these are strict about null and about mismatched kinds. There is no
    /// natural answer to whether an absent value sorts high or low — SQL makes you say, and
    /// most languages refuse — and inventing one would bury a choice in the specification
    /// that rule authors would then have to remember.
    /// </remarks>
    internal sealed class OrderingNode(OrderingRelation relation, ExpressionNode left, ExpressionNode right)
        : ExpressionNode
    {
        public static ExpressionNode BuildLessThan(INodeBuildContext context) =>
            Build(context, OrderingRelation.LessThan);

        public static ExpressionNode BuildLessThanOrEqual(INodeBuildContext context) =>
            Build(context, OrderingRelation.LessThanOrEqual);

        public static ExpressionNode BuildGreaterThan(INodeBuildContext context) =>
            Build(context, OrderingRelation.GreaterThan);

        public static ExpressionNode BuildGreaterThanOrEqual(INodeBuildContext context) =>
            Build(context, OrderingRelation.GreaterThanOrEqual);

        public override RuleValue Evaluate(IEvaluationContext context)
        {
            int order = ValueOrdering.Compare(
                left.Evaluate(context),
                right.Evaluate(context),
                $"cmp.{OperationName(relation)}");

            return RuleValue.Boolean(relation switch
            {
                OrderingRelation.LessThan => order < 0,
                OrderingRelation.LessThanOrEqual => order <= 0,
                OrderingRelation.GreaterThan => order > 0,
                _ => order >= 0
            });
        }

        private static ExpressionNode Build(INodeBuildContext context, OrderingRelation relation) =>
            new OrderingNode(relation, context.RequireExpression("left"), context.RequireExpression("right"));

        private static string OperationName(OrderingRelation relation) => relation switch
        {
            OrderingRelation.LessThan => "lt",
            OrderingRelation.LessThanOrEqual => "lte",
            OrderingRelation.GreaterThan => "gt",
            _ => "gte"
        };
    }

    /// <summary>Three-way comparison, reported as the text <c>lt</c>, <c>eq</c> or <c>gt</c>.</summary>
    /// <remarks>
    /// Text rather than -1/0/1 so that the result drops straight into
    /// <c>branch.match</c>'s cases. A rule set that decides a winner reads
    /// <c>{ "gt": "black", "lt": "white", "eq": "draw" }</c>, where the numeric form would
    /// read <c>{ "1": …, "-1": … }</c> and say nothing.
    /// </remarks>
    internal sealed class CompareNode(ExpressionNode left, ExpressionNode right) : ExpressionNode
    {
        public static ExpressionNode Build(INodeBuildContext context) =>
            new CompareNode(context.RequireExpression("left"), context.RequireExpression("right"));

        public override RuleValue Evaluate(IEvaluationContext context)
        {
            int order = ValueOrdering.Compare(left.Evaluate(context), right.Evaluate(context), "cmp.compare");
            return RuleValue.Text(order < 0 ? "lt" : order > 0 ? "gt" : "eq");
        }
    }

    /// <summary>The first non-null value in <c>of</c>, or null when there is none.</summary>
    /// <remarks>
    /// Short-circuits. Mostly used to flatten a null into a default before handing the
    /// value to an ordering comparison or to arithmetic, both of which refuse it.
    /// </remarks>
    internal sealed class CoalesceNode(ImmutableArray<ExpressionNode> operands) : ExpressionNode
    {
        public static ExpressionNode Build(INodeBuildContext context) =>
            new CoalesceNode(context.RequireExpressionArray("of"));

        public override RuleValue Evaluate(IEvaluationContext context)
        {
            foreach (ExpressionNode operand in operands)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                RuleValue value = operand.Evaluate(context);
                if (!value.IsNull)
                {
                    return value;
                }
            }

            return RuleValue.Null;
        }
    }
}
