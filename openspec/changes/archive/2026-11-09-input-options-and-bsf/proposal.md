## Why

The fluent builder API has an asymmetry: outputs have a full `Action<OutputBuilder>` configure pattern, but inputs offer no way to set options. Users who need per-input flags (user-agent, proxy, seek, HW accel) are forced to drop down to raw record construction, losing the builder entirely. The `pipeline-builder` spec already requires `Input(url, i => i.UserAgent(...).Proxy(...))` — this was specified but never implemented.

Separately, bitstream filters (`-bsf:v`, `-bsf:a`) are supported on `TeeOutput` but not on the regular `StreamConfig`, forcing `CustomArg` workarounds for common remux scenarios like `aac_adtstoasc`.

## Goals

- Close the input options gap so the builder API covers the full FFmpeg input flag surface
- Add typed HTTP input properties (`UserAgent`, `HttpProxy`, `Headers`) to `InputOptionConfig`
- Add bitstream filter support to `StreamConfig` / `StreamBuilder`
- Expose `AddInput(InputConfig)` as public escape hatch

## Non-Goals

- Multi-TFM support (separate concern)
- Progress reporting duality cleanup (separate concern)
- Typed `-map` without stream index (cosmetic, works today with `null`)

## What Changes

- **New `InputOptionsBuilder`** — fluent builder mirroring all `InputOptionConfig` properties, plus new HTTP properties
- **`InputOptionConfig` extended** — add `UserAgent`, `HttpProxy`, `Headers` (flat, not nested — FFmpeg doesn't differentiate input types for these flags)
- **`PipelineBuilder` overloads** — `Input(Uri, Action<InputOptionsBuilder>)`, `Input(string, Action<InputOptionsBuilder>)`, `Input(Stream, string, Action<InputOptionsBuilder>)`
- **`Ffmpeg` static overloads** — matching overloads on the entry-point class
- **`AddInput(InputConfig)` made public** — escape hatch for pre-built configs
- **`StreamConfig` + `BitstreamFilter`** — new `string?` property, consistent with existing `TeeOutput.BitstreamFilter`
- **`StreamBuilder.BitstreamFilter(string)`** — fluent method
- **`ArgumentSerializer`** — serialize per-stream BSF as `-bsf:<type> <filter>`, serialize HTTP input options before `-i`

## Capabilities

### New Capabilities
- `input-options-builder`: Fluent builder for per-input options with typed HTTP properties and configure-pattern overloads on `PipelineBuilder` / `Ffmpeg`
- `bitstream-filters`: Per-stream bitstream filter support on `StreamConfig` with builder and serializer integration

### Modified Capabilities
- `pipeline-builder`: Add `Input(...)` overloads with `Action<InputOptionsBuilder>`, make `AddInput` public
- `scoped-builders`: Add `InputOptionsBuilder` to the builder layer
- `pipeline-model`: Extend `InputOptionConfig` with HTTP properties, extend `StreamConfig` with `BitstreamFilter`

## Impact

- **Public API surface**: New overloads (additive, non-breaking), `AddInput` visibility change (additive), new properties on records (non-breaking due to optional params)
- **Serializer**: Two new serialization paths (HTTP input flags, per-stream BSF)
- **Tests**: New Layer 1 (record), Layer 2 (builder), Layer 3 (serializer) tests for both features
