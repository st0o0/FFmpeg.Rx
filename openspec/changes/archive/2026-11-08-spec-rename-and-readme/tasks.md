## 1. Merge task-named fragments into parent specs

- [x] 1.1 Merge `builder-rewire` requirements into `scoped-builders`, delete `builder-rewire`
- [x] 1.2 Merge `model-cleanup` requirements into `typed-pipeline-model`, delete `model-cleanup`
- [x] 1.3 Merge `session-cleanup` requirements into `session-interface`, delete `session-cleanup`
- [x] 1.4 Merge `integration-gaps` requirements into `integration-tests`, delete `integration-gaps`
- [x] 1.5 Merge `pipe-io-modes` requirements into `pipe-io`, delete `pipe-io-modes`

## 2. Merge same-domain specs

- [x] 2.1 Merge `process-runner` + `cliwrap-integration` → rename to `process-execution`
- [x] 2.2 Merge `progress-parser` + `percentage-progress` → rename to `progress`
- [x] 2.3 Merge `test-infrastructure` + `test-media` → keep as `test-infrastructure`

## 3. Rename verbose/vague specs

- [x] 3.1 Rename `typed-pipeline-model` → `pipeline-model` (after 1.2 merge is done)
- [x] 3.2 Merge `ecosystem-extras` + `dx-polish` → `future-features`
- [x] 3.3 Rename `snapshot-concat` → `convenience-apis`

## 4. Update README Features section

- [x] 4.1 Replace H3-grouped Features section (lines 20-52) with flat 7-line bullet list

## 5. Validate

- [x] 5.1 Run `openspec validate` to check spec consistency
- [x] 5.2 Update AGENTS.md if it references renamed spec names
