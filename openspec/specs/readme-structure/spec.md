# readme-structure Specification

## Purpose
TBD - created by archiving change readme-overhaul. Update Purpose after archive.
## Requirements
### Requirement: README follows library family structure
The README SHALL follow this section order: badges → why → features → installation → quick start → examples → architecture → building from source → contributing → license.

#### Scenario: Section ordering matches family pattern
- **WHEN** a developer opens README.md
- **THEN** sections appear in the canonical order matching WebSocket.Rx / Flickr.Net / GaudiHTTP

### Requirement: README includes positioning section
The README SHALL include a "Why FFmpeg.Rx?" section explaining differentiation from other FFmpeg .NET libraries.

#### Scenario: Why section present
- **WHEN** a developer reads the README
- **THEN** there is a section explaining the type-safe, reactive approach vs string-based alternatives

### Requirement: README includes grouped feature overview
The README SHALL include a features section with grouped bullet points covering pipeline, events, probing, and extensibility.

#### Scenario: Features section present
- **WHEN** a developer skims the README
- **THEN** features are grouped by concern (Pipeline, Events, Probing, Extensibility) with bullet descriptions

### Requirement: README includes architecture diagram
The README SHALL include an ASCII diagram showing the 3-layer architecture (Records → Builders → Serializer+CliWrap).

#### Scenario: Architecture section present
- **WHEN** a developer reads the README
- **THEN** a text-based architecture diagram shows the layer separation

### Requirement: README includes all standard badges
The README SHALL display badges for CI, NuGet version, NuGet downloads, license, and .NET version.

#### Scenario: All badges present
- **WHEN** a developer views the README header
- **THEN** CI, NuGet, Downloads, License, and Dotnet badges are visible

### Requirement: README includes contributing section
The README SHALL include a contributing section with basic guidelines and build instructions.

#### Scenario: Contributing section present
- **WHEN** a developer wants to contribute
- **THEN** the README has a contributing section with fork/branch/PR workflow

