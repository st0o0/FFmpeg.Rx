## Why

The spec inventory grew to 38 files across multiple archived changes. 13 of those are refactoring-task leftovers, dependency-named fragments, or vague grab-bags that should be merged into their parent capability. The README Features section is also overlong (16 bullets across 5 H3 groups) — it duplicates what the code examples already show.

## What Changes

**Spec renames/merges (38 → ~25):**
- Merge task-named fragments into parent specs: `builder-rewire` → `scoped-builders`, `model-cleanup` → `pipeline-model`, `session-cleanup` → `session-interface`, `integration-gaps` → `integration-tests`, `pipe-io-modes` → `pipe-io`
- Merge same-domain specs: `process-runner` + `cliwrap-integration` → `process-execution`, `progress-parser` + `percentage-progress` → `progress`, `test-infrastructure` + `test-media` → `test-infrastructure`
- Rename verbose/vague: `typed-pipeline-model` → `pipeline-model`, `ecosystem-extras` + `dx-polish` → `future-features`, `snapshot-concat` → `convenience-apis`

**README Features section:**
- Replace 5 H3 groups (Pipeline, Events, Probing, Convenience, Extensibility) with a flat 7-line list
- Keep only the differentiating "selling points" — details live in code examples

## Goals

- Every spec name describes a capability, not a refactoring task or dependency
- No orphaned fragments — each merge preserves all requirements from both sources
- README Features scannable in 5 seconds

## Non-Goals

- Rewriting spec content or requirements (just reorganize and merge)
- Changing code examples or other README sections
- Adding new specs

## Capabilities

### New Capabilities

_None — this is a reorganization of existing specs and README content._

### Modified Capabilities

- `readme-structure`: Updated requirements for the Features section layout

## Impact

- `openspec/specs/` directory: ~13 specs removed, ~5 renamed, ~8 specs gain merged requirements
- `README.md`: Features section rewritten (lines 20-52)
- No code changes
