# What's New in C# 15 — DDC 2026 demos

Demo code for Don Wibier's session at the DDC .NET Developer Conference, Cologne (23–26 Nov 2026).
Everything uses a small cinema-ticketing domain.

## Requirements
- .NET 11 SDK (11.0.100 or later; pinned via `global.json`)

See [docs/RunSheet.md](docs/RunSheet.md) for the session timeline and demo script.
The slide deck is in [docs/WhatsNewInCSharp15-DDC2026.pptx](docs/WhatsNewInCSharp15-DDC2026.pptx). It uses the Space Grotesk, Inter and JetBrains Mono fonts.

## Run
```bash
dotnet run --project src/Demos          # menu
dotnet run --project src/Demos -- 2     # run one demo directly
dotnet run --project src/MemorySafety   # preview feature, separate project
```

| # | Demo | File |
|---|------|------|
| 1 | Unions – before (C# 14) | `src/Demos/Demos/01_UnionsBefore.cs` |
| 2 | Unions – after (C# 15), incl. System.Text.Json round trip | `src/Demos/Demos/02_UnionsAfter.cs` |
| 3 | Closed hierarchies | `src/Demos/Demos/03_ClosedHierarchies.cs` |
| 4 | Collection expression arguments | `src/Demos/Demos/04_CollectionArguments.cs` |
| 5 | Extension indexers | `src/Demos/Demos/05_ExtensionIndexers.cs` |
| 6 | Labeled break & continue | `src/Demos/Demos/06_LabeledJumps.cs` |
| 7 | Static interface members on older runtimes | `src/LegacyLib/IPricing.cs` (netstandard2.0) |
| 8 | Memory safety (preview) | `src/MemorySafety/Program.cs` |

## "Break it live" moments (verified with SDK 11.0.100)
| Demo | Change | Result |
|------|--------|--------|
| 2 | Add `Refunded` to `BookingResult` | warning CS8509 – pattern 'Refunded' is not covered |
| 2 | Uncomment `Book(screening, 12.5, …)` | error CS1503 – cannot convert from 'double' to 'SeatRef' |
| 3 | Derive from `ShowStatus` in another assembly | error CS9382 – cannot use a closed type from another assembly as a base type |
| 3 | `sealed closed` | error CS9381 – a closed type cannot be sealed or static |
| 3 | Switch on `ShowStatus?` without `null` arm | warning CS8655 – pattern 'null' is not covered |
| 7 | LegacyLib `LangVersion` 15 → 14 | error CS9327 – 'static members in interfaces' not available in C# 14.0 |
| 8 | Uncomment `*pointer` outside `unsafe` | error CS9360 – only in an unsafe context |
| 8 | Uncomment direct `Barcode.ReadInt32` call | error CS9362 – must be used in an unsafe context |
| 8 | Remove `safe` from `getpid` | error CS9389 – 'extern' member must be marked 'unsafe' or 'safe' |
| 8 | Remove preview flags | error CS8652 – feature is in Preview |
