# Rulealize.Plugin.Comparison

Equality, ordering and null tests for [Rulealize](https://github.com/reny-develop/Rulealize)
rule sets.

| | |
| --- | --- |
| Plugin id | `Rulealize.Plugin.Comparison` |
| Namespace | `cmp` |
| Reserved prefix | none |
| Depends on | `Rulealize.Abstraction` |

Kept apart from `Rulealize.Plugin.Arithmetic` because the reach is different: comparison
applies to text, booleans and opaque values as well as numbers, and a rule set that never
does arithmetic still compares things constantly. Three-way comparison lives here for the
same reason.

## Operations

| Operation | Shape |
| --- | --- |
| `cmp.eq` / `cmp.ne` | `{ "op": "cmp.eq", "left": …, "right": … }` |
| `cmp.lt` / `cmp.lte` / `cmp.gt` / `cmp.gte` | `{ "op": "cmp.gte", "left": …, "right": … }` |
| `cmp.compare` | `{ "op": "cmp.compare", "left": …, "right": … }` → `"lt"` / `"eq"` / `"gt"` |
| `cmp.isNull` | `{ "op": "cmp.isNull", "value": … }` |
| `cmp.coalesce` | `{ "op": "cmp.coalesce", "of": [ … ] }` |

## Equality is null-safe; ordering is not

The asymmetry is the point of this plugin, so it is worth stating plainly.

`cmp.eq` accepts anything. Null against null is true, null against a value is false, and
values of different kinds are unequal rather than a type error. Nothing here throws.

The ordering operations refuse null, and refuse operands of different kinds. Both are
evaluation errors.

The reason is that the two questions have different answers available to them. "Is the
absent value equal to black" has an obvious answer — no. "Is the absent value less than
black" has none, which is why SQL makes you write `NULLS FIRST` and most languages decline
to compare at all. Rather than bury an arbitrary choice in the specification, this plugin
makes the rule author write `cmp.isNull` or `cmp.coalesce` and say what they meant.

### What the tolerance buys

Othello's flip detection reads one square past the end of a run of opposing stones:

```jsonc
{
  "op": "cmp.eq",
  "left": { "op": "grid.at", "grid": "$board",
            "coord": { "op": "seq.elementAt", "source": "@ray",
                       "index": { "op": "seq.count", "source": "@run" } } },
  "right": "#me"
}
```

When the ray runs off the edge of the board, `seq.elementAt` gives null, `grid.at` gives
null, and this comparison answers false — which is exactly right, because there is no
stone of the mover's colour closing the run. Had equality been strict, the rule author
would have had to compare the ray's length against the run's length first, and write into
the document a bounds check the value model already performs.

## Ordering rules

| Kind | Order |
| --- | --- |
| Number | numeric |
| Text | ordinal by code point, never culture-aware |
| Bool | false before true |
| anything else | evaluation error |

Text is ordinal so that a rule set means the same thing on every host. A culture-sensitive
comparison would make the outcome depend on where the process happens to be running.

## Building

`Rulealize.Abstraction` is not on nuget.org yet, so `NuGet.config` points at a folder
feed. Produce it from the abstraction repository first:

```
dotnet pack path\to\Rulealize.Abstraction\src\Rulealize.Abstraction -c Release -o path\to\LocalNuGet
```

with `LocalNuGet` a sibling of this repository. Then `dotnet build`.

## License

Apache-2.0.
