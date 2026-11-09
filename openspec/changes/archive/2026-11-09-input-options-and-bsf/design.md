## Context

The builder API has a complete configure-pattern for outputs (`Output(path, Action<OutputBuilder>)`) but none for inputs. Users needing per-input flags (user-agent, proxy, seek, HW accel) must construct raw records and cannot use the fluent API. The `pipeline-builder` spec already requires input option support but it was never implemented.

Separately, bitstream filters exist on `TeeOutput.BitstreamFilter` but not on the regular `StreamConfig`, forcing `CustomArg` workarounds for common remux operations.

## Goals / Non-Goals

**Goals:**
- Symmetric builder API: inputs get the same `Action<Builder>` configure pattern as outputs
- Typed HTTP input properties on `InputOptionConfig` for the most common URL flags
- Public escape hatch (`AddInput`) for pre-built `InputConfig` records
- Per-stream bitstream filter on `StreamConfig` with builder and serializer support

**Non-Goals:**
- Nested `HttpInputOptions` type (flat on `InputOptionConfig` — FFmpeg itself doesn't differentiate)
- Per-input-type `InputOptionConfig` subtypes (overengineered for the FFmpeg domain)
- Removing string-based `CustomArgs` (remains as escape hatch for uncovered flags)

## Decisions

### D1: Flat HTTP properties on `InputOptionConfig`

`UserAgent`, `HttpProxy`, `Headers` go directly on `InputOptionConfig` as `string?` properties. They are semantically input-level flags in FFmpeg (placed before `-i`), regardless of whether the input is a file or URL. The serializer emits them only when non-null. This avoids a separate nested type while keeping the most common HTTP flags discoverable.

### D2: `InputOptionsBuilder` mirrors `InputOptionConfig` 1:1

Every property on `InputOptionConfig` gets a corresponding fluent method on `InputOptionsBuilder`. The builder's `Build()` returns `InputOptionConfig`. This keeps the builder as pure sugar over the record — no hidden state.

```
InputOptionsBuilder
  .Seek(TimeSpan)
  .HWAccel(HWAccel)
  .HWAccelDevice(string)
  .HWAccelOutputFormat(string)
  .Decoder(VideoCodec)
  .Framerate(Framerate)
  .Loop()
  .StreamLoop(int)
  .UserAgent(string)        ← new
  .HttpProxy(string)        ← new
  .Headers(string)          ← new
  .CustomArg(string)
```

### D3: Overloads on both `Ffmpeg` and `PipelineBuilder`

Both the static entry point and the chainable builder get `Action<InputOptionsBuilder>` overloads:

```
// Entry point
Ffmpeg.Input(Uri url, Action<InputOptionsBuilder> configure)
Ffmpeg.Input(string path, Action<InputOptionsBuilder> configure)
Ffmpeg.Input(Stream source, string format, Action<InputOptionsBuilder> configure)

// Chained
builder.Input(Uri url, Action<InputOptionsBuilder> configure)
builder.Input(string path, Action<InputOptionsBuilder> configure)
builder.Input(Stream source, string format, Action<InputOptionsBuilder> configure)
```

### D4: `AddInput(InputConfig)` made public

The existing `internal` method becomes `public`. This is the escape hatch for users who build `InputConfig` records directly (e.g., with complex `with` expressions or custom subclasses in tests). Consistent with the architecture guardrail that records are the testable truth.

### D5: BSF on `StreamConfig` (not `OutputConfig`)

FFmpeg's `-bsf:v` / `-bsf:a` / `-bsf:s` are stream-type-scoped. `StreamConfig` already carries `Type`, making it the natural home. This is consistent with `TeeOutput.BitstreamFilter`. The serializer emits `-bsf:<type-char> <filter>` per stream that has it set.

### D6: Serialization order for input options

HTTP options (`-user_agent`, `-http_proxy`, `-headers`) serialize **before** `-i` in the same position as existing input options (seek, hwaccel). Order: hwaccel flags → http flags → seek → custom args → `-i <url>`.

## Risks / Trade-offs

- **Flat HTTP props on a generic type**: `UserAgent` on a `FileInputConfig` is semantically meaningless. Accepted trade-off: the serializer ignores it, and the builder makes it discoverable only via the URL overload's IntelliSense. Simpler than a type hierarchy.
- **`AddInput` public**: Exposes the record layer directly. Accepted: this is already the project's stated architecture ("records are the testable truth"). Users who need it are advanced and know what they're doing.
- **BSF as `string?`**: No typed BSF enum. FFmpeg has dozens of bitstream filters with codec-specific applicability. A string with well-known constants (like `VideoCodec.Custom()` pattern) could come later but is not needed for v1.
