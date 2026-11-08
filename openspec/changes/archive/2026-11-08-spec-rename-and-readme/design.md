## Context

38 specs accumulated across 9 archived changes. 13 are task-named fragments, dependency-named leftovers, or vague grab-bags. The README Features section uses 5 H3 groups with 16 bullets for content that could be 7 lines.

## Goals / Non-Goals

**Goals:**
- Merge fragment specs into parent capability specs (preserving all requirements)
- Rename verbose/vague specs to capability names
- Flatten README Features to a scannable list

**Non-Goals:**
- Rewriting requirement content (copy-paste merge only)
- Touching code examples or other README sections
- Adding or removing requirements

## Decisions

### 1. Merge strategy: append, don't rewrite

When merging two specs (e.g. `progress-parser` + `percentage-progress` → `progress`), all requirements from both source files are concatenated into the target. No requirement text is edited — only the file name and Purpose header change.

**Alternative considered:** Deduplicate overlapping requirements during merge. Rejected because it risks silently dropping a requirement, and the overlap is minimal.

### 2. Rename via directory move, not new file

Renames (e.g. `typed-pipeline-model` → `pipeline-model`) are `git mv` of the spec directory. This preserves git history.

**Alternative considered:** Delete old + create new. Rejected because it loses blame history.

### 3. README Features as flat list

Replace the 5 H3-grouped sections with a single flat bullet list. Each bullet: `**Name** -- one-line description`. Seven bullets covering: typed model, disjoint codecs, R3 observables, ffprobe, convenience APIs, escape hatches, mockability.

**Alternative considered:** Keep H3 groups but trim bullets. Rejected because the grouping itself is the problem — it fragments what should be one scannable block.

## Risks / Trade-offs

- [Merge might miss a requirement] → Mitigation: read both source files fully before merging; validate with `openspec validate` after.
- [README might lose discoverability of minor features] → Mitigation: code examples below Features already cover probing, filters, pipe I/O in detail.

## Open Questions

None — this is a mechanical reorganization.
