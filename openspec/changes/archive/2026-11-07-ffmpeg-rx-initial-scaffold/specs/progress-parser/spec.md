## ADDED Requirements

### Requirement: Parse FFmpeg progress blocks from stdout
`ProgressParser` SHALL parse FFmpeg's `-progress pipe:1` output format: key=value lines terminated by a `progress=continue` or `progress=end` line. Each complete block SHALL produce a `ProgressEvent`.

#### Scenario: Complete progress block
- **WHEN** stdout contains `total_size=5000000\nout_time_us=3000000\nspeed=1.5x\nprogress=continue\n`
- **THEN** a `ProgressEvent(BytesWritten: 5000000, TimeUs: 3000000, Speed: 1.5)` SHALL be produced

#### Scenario: Partial block (no progress= line yet)
- **WHEN** stdout contains `total_size=5000000\nout_time_us=3000000\n` without a `progress=` terminator
- **THEN** no `ProgressEvent` SHALL be produced until the block is complete

#### Scenario: Speed value "N/A"
- **WHEN** the `speed` field is `N/A`
- **THEN** `Speed` SHALL be `0.0`

#### Scenario: Missing fields
- **WHEN** a progress block is missing `total_size` or `out_time_us`
- **THEN** the missing values SHALL default to `0`

### Requirement: Parser is stateful per process and pure per block
`ProgressParser` SHALL accumulate key=value pairs across lines until a `progress=` line completes the block. After emitting, the accumulator SHALL be cleared for the next block. The block-to-event conversion SHALL be a pure function.

#### Scenario: Multiple consecutive blocks
- **WHEN** two complete progress blocks are received sequentially
- **THEN** two separate `ProgressEvent` instances SHALL be produced with independent values

### Requirement: Parser handles malformed input gracefully
Lines without `=` or with empty keys SHALL be ignored. The parser SHALL never throw.

#### Scenario: Malformed line
- **WHEN** stdout contains a line without `=` (e.g. a warning message mixed into stdout)
- **THEN** the line SHALL be ignored and parsing SHALL continue
