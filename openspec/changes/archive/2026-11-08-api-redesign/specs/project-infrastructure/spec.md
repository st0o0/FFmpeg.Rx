## ADDED Requirements

### Requirement: AGENTS.md with guardrails
The repository SHALL contain an `AGENTS.md` file defining: project description, solution structure, architecture (layer diagram, core types table), architecture guardrails, C# conventions, test conventions with assertion rules, and build/test commands.

#### Scenario: Test assertion rules
- **WHEN** AGENTS.md is consulted for test conventions
- **THEN** it SHALL ban vacuous assertions (`Assert.NotNull(x)` as sole assertion), require inline timeouts on async tests, and require asserting concrete values

### Requirement: CLAUDE.md references AGENTS.md
`CLAUDE.md` SHALL begin with `@AGENTS.md` and add skill routing. Domain knowledge SHALL live in AGENTS.md, not duplicated in CLAUDE.md.

#### Scenario: CLAUDE.md structure
- **WHEN** CLAUDE.md is read
- **THEN** it starts with `@AGENTS.md` and the rest is skill routing

### Requirement: OpenSpec config with real context
`openspec/config.yaml` SHALL contain a `context:` block with: project description, tech stack, architecture summary, conventions, and a reference to the solution structure. It SHALL contain `rules:` with task-level rules.

#### Scenario: Config has context
- **WHEN** `openspec/config.yaml` is read
- **THEN** it has a non-empty `context:` block describing FFmpeg.Rx

### Requirement: TreatWarningsAsErrors
`Directory.Build.props` SHALL set `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.

#### Scenario: Warning breaks build
- **WHEN** code produces a compiler warning
- **THEN** the build fails

### Requirement: Fix CI workflow
The CI workflow SHALL reference the correct solution file (`src/FFmpeg.Rx.slnx`), not a template placeholder.

#### Scenario: CI builds successfully
- **WHEN** CI runs
- **THEN** it builds and tests the actual solution
