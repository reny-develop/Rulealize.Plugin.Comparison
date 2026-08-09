// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Evaluation;
using Rulealize.Abstraction.Node;
using Rulealize.Abstraction.Value;

namespace Rulealize.Plugin.Comparison
{
    /// <summary>Equality of <c>left</c> and <c>right</c> under the value model's rules.</summary>
    /// <remarks>
    /// <para>
    /// Null-safe: neither side being null is an error, and values of different kinds are
    /// simply unequal rather than a type fault. That tolerance is what lets Othello's flip
    /// detection be written without a bounds check.
    /// </para>
    /// <para>
    /// Reading one square past the end of a ray gives null from <c>seq.elementAt</c>, which
    /// gives null from <c>grid.at</c>, which arrives here and answers false. Were this an
    /// error the rule author would have to compare the ray's length against the run's
    /// length first, and say in the document what the value model already knows.
    /// </para>
    /// </remarks>
    internal sealed class EqualNode(ExpressionNode left, ExpressionNode right) : ExpressionNode
    {
        public static ExpressionNode Build(INodeBuildContext context) =>
            new EqualNode(context.RequireExpression("left"), context.RequireExpression("right"));

        public override RuleValue Evaluate(IEvaluationContext context) =>
            RuleValue.Boolean(left.Evaluate(context).Equals(right.Evaluate(context)));
    }

    /// <summary>The negation of <c>cmp.eq</c>.</summary>
    internal sealed class NotEqualNode(ExpressionNode left, ExpressionNode right) : ExpressionNode
    {
        public static ExpressionNode Build(INodeBuildContext context) =>
            new NotEqualNode(context.RequireExpression("left"), context.RequireExpression("right"));

        public override RuleValue Evaluate(IEvaluationContext context) =>
            RuleValue.Boolean(!left.Evaluate(context).Equals(right.Evaluate(context)));
    }

    /// <summary>True when <c>value</c> is null.</summary>
    /// <remarks>
    /// <c>cmp.eq</c> against a JSON <c>null</c> decides the same thing. The dedicated node
    /// exists so that the intent is legible: a bare <c>null</c> in expression position
    /// makes a reader stop and work out whether it is structure or value.
    /// </remarks>
    internal sealed class IsNullNode(ExpressionNode operand) : ExpressionNode
    {
        public static ExpressionNode Build(INodeBuildContext context) =>
            new IsNullNode(context.RequireExpression("value"));

        public override RuleValue Evaluate(IEvaluationContext context) =>
            RuleValue.Boolean(operand.Evaluate(context).IsNull);
    }
}
