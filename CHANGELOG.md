# Changelog

## [0.1.1](https://github.com/st0o0/FFmpeg.Rx/compare/v0.1.0...v0.1.1) (2026-11-09)


### Features

* add input options builder and bitstream filter support ([e097434](https://github.com/st0o0/FFmpeg.Rx/commit/e0974342eceef8e43120c10b7a96b1f53286449c))

## [0.1.0](https://github.com/st0o0/FFmpeg.Rx/compare/v0.1.0...v0.1.0) (2026-11-08)


* archive readme-overhaul, merge 2 specs to main ([272f148](https://github.com/st0o0/FFmpeg.Rx/commit/272f1480161ea30a0983fb52289fe7d6cc7d5fb7))


### Features

* add .editorconfig with C# conventions and naming rules ([189cc42](https://github.com/st0o0/FFmpeg.Rx/commit/189cc4205bb2a58cd39d9fb8efddce9691c1f016))
* add CliWrap dependency for process management ([7971eb0](https://github.com/st0o0/FFmpeg.Rx/commit/7971eb0bf6e45acd340ef95e679b47a32665371f))
* add IFfmpegSession interface and ArgumentSerializer ([4e4f025](https://github.com/st0o0/FFmpeg.Rx/commit/4e4f0257254d7b16d4f1024cb2444678eca44d25))
* add integration test project with 21 tests and synthetic test media ([f36e5e0](https://github.com/st0o0/FFmpeg.Rx/commit/f36e5e01058db46ffb97c6923bda7f7966d7b782))
* add NuGet packaging (SourceLink, lock files, README, release workflow) ([d51ba03](https://github.com/st0o0/FFmpeg.Rx/commit/d51ba030d22ad09880f4412becbf3acf3f537665))
* add project infrastructure (AGENTS.md, CI hooks, TreatWarningsAsErrors) ([9fee634](https://github.com/st0o0/FFmpeg.Rx/commit/9fee63484081277b68b08ee2262da1b1d4cc6bef))
* add scoped builders (OutputBuilder, StreamBuilder, encoding builders) ([1322374](https://github.com/st0o0/FFmpeg.Rx/commit/132237471606d901b08e2c92999d3396eeefa5a1))
* add Snapshot, Concat, Probe shortcuts and Percentage Progress ([7841987](https://github.com/st0o0/FFmpeg.Rx/commit/7841987be451e7fa0d083347f3c6e68e06587542))
* add typed domain model (value objects, pipeline records, codec hierarchy) ([1550d7f](https://github.com/st0o0/FFmpeg.Rx/commit/1550d7f7b426efc54030d643b101dd6470fe325d))
* align README with library family pattern, add AOT example project + CI hooks ([2ea075a](https://github.com/st0o0/FFmpeg.Rx/commit/2ea075a2574ad222764cecbc100cb25efb766a9d))
* rewrite FfmpegSession and ProbeRunner on CliWrap ([a6ab8a1](https://github.com/st0o0/FFmpeg.Rx/commit/a6ab8a1391156db64c5a993d3080b92f8ca90b9f))
* rewrite tests for typed API (186 tests, 42 new) ([3362027](https://github.com/st0o0/FFmpeg.Rx/commit/3362027f8eeb2a477a8d0c14b3def08f904f78d2))
* route progress to stderr when stdout is pipe output ([292d96c](https://github.com/st0o0/FFmpeg.Rx/commit/292d96cfa5c67a33b5a1ecc5592ca6c9cc78fa11))


### Bug Fixes

* quote metadata values in ArgumentSerializer, update AGENTS.md ([6b3b989](https://github.com/st0o0/FFmpeg.Rx/commit/6b3b98960ffe62bc95050bd6a332e90824c33ab4))
* use SDK built-in SourceLink, move InternalsVisibleTo to csproj, fix README ([337a266](https://github.com/st0o0/FFmpeg.Rx/commit/337a2660a105aa8e26786d4fd908508f81df22b4))


### Documentation

* replace em dashes with hyphens in README ([12670e3](https://github.com/st0o0/FFmpeg.Rx/commit/12670e3a163a1cd05a7fd4aecbb677b7aee604fe))
* rewrite README to use only fluent API examples ([8283e31](https://github.com/st0o0/FFmpeg.Rx/commit/8283e313085caf9b7c1b853c1add05d0c0823eb0))


### Refactoring

* consolidate editorconfig to src/, fix format violations ([d620902](https://github.com/st0o0/FFmpeg.Rx/commit/d620902608ea69f6ed922f598a9a2df3afa67d50))
* delete old model, ProcessRunner, string-bag builders (8 files) ([d598193](https://github.com/st0o0/FFmpeg.Rx/commit/d598193359b23c2a62a5341ddb40a11103a6842a))
* merge execution paths, extract pure functions, add R3 extensions ([011ac4b](https://github.com/st0o0/FFmpeg.Rx/commit/011ac4bcc3e66a00181c68e306e44b76fb05240e))
* rename *Spec model types to *Config across codebase ([1faa8f0](https://github.com/st0o0/FFmpeg.Rx/commit/1faa8f09142be88d15279e2c3bdcc840baf468ce))
* rewire PipelineBuilder and all builders to produce PipelineSpec ([960327f](https://github.com/st0o0/FFmpeg.Rx/commit/960327f83634ce638d13a5a60085c0780abf41e3))
