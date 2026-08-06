// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Plugins;

namespace Rulealize.Plugin.Comparison
{
    /// <summary>
    /// Equality, ordering and null tests over the <c>cmp</c> namespace.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Separate from the arithmetic plugin because comparison reaches further: text,
    /// booleans and opaque values are all compared, while arithmetic applies to numbers
    /// alone. Three-way comparison lives here for the same reason.
    /// </para>
    /// <para>
    /// Equality is null-safe and ordering is not. The asymmetry is deliberate — see
    /// the readme.
    /// </para>
    /// </remarks>
    public sealed class ComparisonPlugin : IRulealizePlugin
    {
        /// <inheritdoc />
        public PluginManifest Manifest { get; } =
            new("Rulealize.Plugin.Comparison", new Version(1, 0, 0), "cmp");

        /// <inheritdoc />
        public void Register(IPluginRegistry registry)
        {
            ArgumentNullException.ThrowIfNull(registry);

            registry.AddExpression("eq", EqualNode.Build);
            registry.AddExpression("ne", NotEqualNode.Build);
            registry.AddExpression("lt", OrderingNode.BuildLessThan);
            registry.AddExpression("lte", OrderingNode.BuildLessThanOrEqual);
            registry.AddExpression("gt", OrderingNode.BuildGreaterThan);
            registry.AddExpression("gte", OrderingNode.BuildGreaterThanOrEqual);
            registry.AddExpression("compare", CompareNode.Build);
            registry.AddExpression("isNull", IsNullNode.Build);
            registry.AddExpression("coalesce", CoalesceNode.Build);
        }
    }
}
