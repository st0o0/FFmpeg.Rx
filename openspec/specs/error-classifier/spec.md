## ADDED Requirements

### Requirement: Classify FFmpeg errors as Transient or Permanent
`ErrorClassifier` SHALL classify FFmpeg stderr error strings into `FailureKind.Transient` (retryable) or `FailureKind.Permanent` (not retryable). Unknown errors SHALL default to `FailureKind.Permanent`.

#### Scenario: HTTP 503 is transient
- **WHEN** the error string contains "503"
- **THEN** `FailureKind` SHALL be `Transient`

#### Scenario: HTTP 502 is transient
- **WHEN** the error string contains "502"
- **THEN** `FailureKind` SHALL be `Transient`

#### Scenario: Connection reset is transient
- **WHEN** the error string contains "Connection reset"
- **THEN** `FailureKind` SHALL be `Transient`

#### Scenario: Timed out is transient
- **WHEN** the error string contains "timed out"
- **THEN** `FailureKind` SHALL be `Transient`

#### Scenario: Connection refused is transient
- **WHEN** the error string contains "Connection refused"
- **THEN** `FailureKind` SHALL be `Transient`

#### Scenario: Network unreachable is transient
- **WHEN** the error string contains "Network is unreachable"
- **THEN** `FailureKind` SHALL be `Transient`

#### Scenario: HTTP 403 is permanent
- **WHEN** the error string contains "403" but not "503"
- **THEN** `FailureKind` SHALL be `Permanent`

#### Scenario: Invalid data is permanent
- **WHEN** the error string contains "Invalid data found"
- **THEN** `FailureKind` SHALL be `Permanent`

#### Scenario: Unknown error is permanent
- **WHEN** the error string does not match any known pattern
- **THEN** `FailureKind` SHALL be `Permanent`

#### Scenario: Null or empty error
- **WHEN** the error string is null or empty
- **THEN** `FailureKind` SHALL be `Permanent`

### Requirement: Extract the most relevant error line from stderr
`ErrorClassifier.ExtractError(string stderr)` SHALL scan stderr from the bottom up and return the first line matching a known error pattern (HTTP error, Server returned, Error opening input, Invalid data found). If no pattern matches, it SHALL return the last non-empty line.

#### Scenario: HTTP error in stderr
- **WHEN** stderr contains multiple lines and one contains "HTTP error 403 Forbidden"
- **THEN** `ExtractError` SHALL return that specific line

#### Scenario: No known pattern
- **WHEN** stderr contains only generic output without known error patterns
- **THEN** `ExtractError` SHALL return the last non-empty line

#### Scenario: Empty stderr
- **WHEN** stderr is null or whitespace
- **THEN** `ExtractError` SHALL return an empty string

### Requirement: Classification is a pure function
`ErrorClassifier.Classify(string? error)` and `ErrorClassifier.ExtractError(string? stderr)` SHALL be static pure functions with no side effects or external dependencies.

#### Scenario: Thread safety
- **WHEN** multiple threads call `Classify` concurrently
- **THEN** all calls SHALL return correct results without interference
