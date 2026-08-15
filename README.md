# Rulealize.Plugin.Comparison

Equality, ordering and null tests for [Rulealize](https://github.com/reny-develop/Rulealize)
rule sets.

| | |
| --- | --- |
| Plugin id | `Rulealize.Plugin.Comparison` |
| Namespace | `cmp` |
| Reserved prefix | none |
| Depends on | `Rulealize.Abstraction` |
| Specification | [doc/specification.md](doc/specification.md) |

`cmp.eq`, `ne`, `lt`, `lte`, `gt`, `gte`, `compare`, `isNull`, `coalesce`.

Kept apart from `Rulealize.Plugin.Arithmetic` because the reach is different: comparison
applies to text, booleans and opaque values as well as numbers, and a rule set that never
does arithmetic still compares things constantly. Three-way comparison lives here for the
same reason.

The asymmetry the specification is built around, stated plainly here because it is the
point of the plugin: **equality is null-safe and ordering is not.** `cmp.eq` accepts
anything and never faults — null against null is true, null against a value is false,
different kinds are unequal. The ordering operations refuse null and refuse mismatched
kinds, both as evaluation errors. "Is the absent value equal to black" has an obvious
answer; "is the absent value less than black" has none, and rather than bury an arbitrary
choice in the specification this plugin makes the rule author write `cmp.isNull` or
`cmp.coalesce` and say what they meant.

Text ordering is ordinal, by code point, never culture-aware, so that a rule set means the
same thing on every host.

## Building

`dotnet build`. `Rulealize.Abstraction` restores from nuget.org like any other package, so
this repository builds on its own.

[`NuGet.config`](NuGet.config) also adds a folder feed named `LocalNuGet` beside the
repositories — added to nuget.org rather than replacing it — which is how a change to the
abstraction is tried out before it is published. Pack it when you have changed it:

```sh
dotnet pack path\to\Rulealize.Abstraction\src\Rulealize.Abstraction -c Release -o path\to\LocalNuGet
```

## License

Apache-2.0.
