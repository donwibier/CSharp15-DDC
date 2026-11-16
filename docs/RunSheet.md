# Run sheet — What's New in C# 15 (DDC 2026, 60 min)

Deck: https://claude.ai/artifact/KYg5cMgef9fVCDRNEt7iH4 (speaker notes on every slide)
Demo code: this repo. Every "break it live" step below was verified with SDK 11.0.100.

## Before you go on stage

- [ ] `dotnet --version` shows 11.0.100 or later (the SDK pinned in `global.json`)
- [ ] `dotnet build` in the repo root: 0 warnings, 0 errors
- [ ] Run each demo once (warms up the build): `dotnet run --project src/Demos -- 2` … `-- 7`, `dotnet run --project src/MemorySafety`
- [ ] `git status` clean, so you can `git checkout -- .` after a break-it-live change
- [ ] IDE open on the solution, font size 18+ pt, terminal font large, notifications off
- [ ] Tabs open in this order: `01_UnionsBefore.cs`, `02_UnionsAfter.cs`, `03_ClosedHierarchies.cs`, `04_CollectionArguments.cs`, `05_ExtensionIndexers.cs`, `06_LabeledJumps.cs`, `src/LegacyLib/IPricing.cs` + `LegacyLib.csproj`, `src/MemorySafety/Program.cs` + `MemorySafety.csproj`
- [ ] Offline fallback: the deck downloads as PDF/PPTX (Share › Export), and every demo's code is also on the slides

## Timeline

| Clock | Min | Slides | What | Demo |
|------:|----:|--------|------|------|
| 0:00 | 1 | 1 | Welcome. C# 15 shipped on 10 Nov with .NET 11 | – |
| 0:01 | 2 | 2 | About me. Cinema story → today's demo domain | – |
| 0:03 | 1 | 3 | Agenda; memory safety is the only preview feature | – |
| 0:04 | 1 | 4 | Unions intro: "Who uses OneOf / Result / base class?" | – |
| 0:05 | 2 | 5 | The problem in C# 14 | **1** (optional) |
| 0:07 | 2 | 6 | Declare a union | **2** |
| 0:09 | 4 | 7 | Exhaustive matching + break it live | **2** |
| 0:13 | 3 | 8 | SeatRef, Option<T>, Discount body | **2** |
| 0:16 | 2 | 9 | Under the hood: struct, Value, default | – |
| 0:18 | 3 | 10 | System.Text.Json round trip | **2** |
| 0:21 | 1 | 11 | Part 2 intro | – |
| 0:22 | 5 | 12 | Closed hierarchies + break it live | **3** |
| 0:27 | 2 | 13 | Union vs. closed | – |
| 0:29 | 6 | 14 | Collection expression arguments | **4** |
| 0:35 | 5 | 15 | Extension indexers | **5** |
| 0:40 | 4 | 16 | Labeled break/continue | **6** |
| 0:44 | 3 | 17 | Static interface members (netstandard2.0) | **7** |
| 0:47 | 2 | 18 | Memory safety: the idea | – |
| 0:49 | 4 | 19 | Memory safety code + break it live | **8** |
| 0:53 | 1 | 20 | Recap | – |
| 0:54 | 1 | 21 | Getting started, repo link | – |
| 0:55 | 5 | 22 | Q&A | – |

**Running late?** Cut in this order: demo 1 (show slide 5 only), the JSON live variant, the extension indexers demo (slide only), demo 7's break-it-live.
**Running early?** Add the JSON break-it-live (remove `[JsonUnion]` → JsonException) and show `FindAdjacentBefore` vs `FindAdjacent` side by side.

## Demo steps

### Demo 1 — Unions before (`-- 1`, optional)
- Show the abstract base class, the `_ => throw new UnreachableException()` arm, and `Book(…, object seat, …)`.
- Output line "Runtime surprise" = passing `12.5` compiled fine and failed at runtime.

### Demo 2 — Unions after (`-- 2`)
1. Show the four records (no base class) and `public union BookingResult(...)`.
2. Run it. Point at "how about A1?" (Option<Seat>), the €10.00 (20% Percentage) and €7.50 (FixedAmount) lines.
3. **Break it:** add `public record class Refunded(string TicketCode);` and add `Refunded` to the union.
   → `warning CS8509: … the pattern 'Demos.Demos.After.Refunded' is not covered.` Undo.
4. **Break it:** uncomment `// BookingService.Book(screening, 12.5, "4111");`
   → `error CS1503: Argument 2: cannot convert from 'double' to 'Demos.Demos.After.SeatRef'`. Undo.
5. JSON: the last three output lines (`JSON:`, `Back: SoldOut`, `SeatRef from "F12": String F12`).
   Optional: remove the `[JsonUnion(...)]` attribute → JsonException "JSON value type 'Object' is ambiguous … Specify a custom type classifier". Undo.

### Demo 3 — Closed hierarchies (`-- 3`)
- Show `CanBook` in the base class: shared behavior, which a union can't hold in fields.
- **Break it:** `public sealed closed record class ShowStatus` → `error CS9381: 'ShowStatus': a closed type cannot be sealed or static`. Undo.
- (Explain, don't do) Deriving from another assembly → `error CS9382: … cannot use a closed type 'ShowStatus' from another assembly as a base type.`
- Nullable: `DescribeNullable` needs a `null` arm; without it you get CS8655.

### Demo 4 — Collection expression arguments (`-- 4`)
- Promo codes: 3 literals, Count 2, `Contains("EarlyBird")` is True.
- `free` has capacity 60. Then SortedSet with a comparer, and `SeatSelection` with a required constructor argument.

### Demo 5 — Extension indexers (`-- 5`)
- `screening['F', 10]`, `screening["A3"]`.
- `SeatRow`: Count + int indexer + Slice is enough to light up `rowC[^1]`, `rowC[2..5]` and the list pattern.

### Demo 6 — Labeled break/continue (`-- 6`)
- Show the seat map, then `FindAdjacentBefore` (flag) next to `FindAdjacent` (`continue candidate;` / `break search;`). Same results.

### Demo 7 — Static interface members (`-- 7`)
- `src/LegacyLib` targets netstandard2.0 with `<LangVersion>15</LangVersion>`.
- **Break it:** set LangVersion to 14 → `error CS9327: Feature 'static members in interfaces' is not available in C# 14.0.` Undo.

### Demo 8 — Memory safety (`dotnet run --project src/MemorySafety`)
- Show the csproj: `LangVersion preview` + `Features updated-memory-safety-rules` + `AllowUnsafeBlocks`.
- **Break it** (one at a time, undo each):
  - uncomment `// Console.WriteLine(*pointer);` → `error CS9360: This operation may only be used in an unsafe context`
  - uncomment `// int number = Barcode.ReadInt32(start + 4);` → `error CS9362: 'Barcode.ReadInt32(byte*)' must be used in an unsafe context because it is marked as 'unsafe'`
  - remove `safe` from `getpid` → `error CS9389: 'extern' member must be marked 'unsafe' or 'safe'.`

## Q&A cheat sheet

| Question | Answer |
|----------|--------|
| Are unions allocation-free? | The union is a struct, but `Value` is `object?`: value-type cases get boxed, reference cases don't allocate extra. |
| Union or closed class? | Slide 13. Cases you don't own, or primitives → union. A family of types you design, with shared state → closed. |
| Unions on .NET 10/8? | They need `UnionAttribute` + `IUnion` (ship with .NET 11). Without them: CS0518/CS0656. A hand-written polyfill compiles and runs on net10.0 (tested), but it's not supported. |
| Does `closed` stop indirect subclasses? | No, only direct ones. Mark intermediate types `sealed` or `closed`. |
| Can I put fields in a union? | No instance fields, auto-properties or field-like events. Methods and computed properties are fine. |
| JSON with several object cases? | `[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]` or a custom `JsonTypeClassifierFactory`. |
| Is runtime async part of C# 15? | No. Runtime async is a .NET 11 runtime feature (still preview), not a language feature. |
| EF Core / ASP.NET Core and unions? | Not verified for this talk. "Check the docs" is the honest answer. |
