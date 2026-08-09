// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction;
using Rulealize.Abstraction.Value;

namespace Rulealize.Plugin.Comparison
{
    /// <summary>The order the comparison operations share.</summary>
    /// <remarks>
    /// <para>
    /// Numbers compare numerically, text ordinally, and false sorts before true. Nothing
    /// else has an order, and nothing compares across kinds.
    /// </para>
    /// <para>
    /// Text is ordinal rather than culture-aware on purpose. A rule set that sorted
    /// differently in Istanbul than in London would not be one rule set.
    /// </para>
    /// </remarks>
    internal static class ValueOrdering
    {
        /// <summary>Compares two values, or throws when they have no order.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <param name="origin">Where the values came from, for the error message.</param>
        /// <returns>Negative, zero or positive.</returns>
        public static int Compare(RuleValue left, RuleValue right, string origin)
        {
            if (left.IsNull || right.IsNull)
            {
                throw new RuleEvaluationException(
                    origin,
                    "Ordering is not defined for null. Test with cmp.isNull or supply a default with cmp.coalesce.");
            }

            if (left.Kind != right.Kind)
            {
                throw new RuleEvaluationException(
                    origin,
                    $"Cannot order {RuleValue.Describe(left)} against {RuleValue.Describe(right)}; they are of different kinds.");
            }

            return left switch
            {
                NumberValue number => number.Value.CompareTo(((NumberValue)right).Value),
                TextValue text => string.CompareOrdinal(text.Value, ((TextValue)right).Value),
                BooleanValue boolean => boolean.Value.CompareTo(((BooleanValue)right).Value),
                _ => throw new RuleEvaluationException(
                    origin,
                    $"{RuleValue.Describe(left)} has no ordering.")
            };
        }
    }
}
