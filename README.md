<div align="center">

![Internet Friendly Media Encoder](IFME/Resources/SplashScreen14.png)

# Internet Friendly Media Encoder

**A lightweight, plugin-based multimedia encoder for Windows and Linux.**

[![Latest release](https://img.shields.io/github/v/release/Anime4000/IFME?label=release&color=brightgreen)](https://github.com/Anime4000/IFME/releases/latest)
[![GitHub downloads](https://img.shields.io/github/downloads/Anime4000/IFME/total?label=GitHub%20downloads)](https://github.com/Anime4000/IFME/releases)
[![SourceForge downloads](https://img.shields.io/sourceforge/dt/ifme?label=SourceForge%20downloads)](https://sourceforge.net/projects/ifme/)
[![License](https://img.shields.io/badge/license-GPL--2.0-blue)](LICENSE.md)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux-lightgrey)](#system-requirements)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)

**Language:** English | [日本語](/README_ja-JP.md) | [简体中文](/README_zh-CN.md) | [Bahasa Malaysia](/README_ms-MY.md)

</div>

![IFME user interface](IFME/Resources/i18n/IFME_en-US.png)

---

## What is IFME?

IFME encodes, remuxes and repackages video and audio. It handles subtitles and
font attachments, merges multiple streams into a single file, drops the streams
you do not want, and can pull subtitles straight out of another video without
extracting them first. AviSynth scripts are supported for advanced processing.

What makes it different is that **the encoders are not baked in**.

## Encoders are plugins

Every encoder is described by a JSON manifest in the `Plugins` folder. IFME
discovers them at startup, so you can update x265, swap in a build tuned for
your own CPU, or add an encoder IFME has never heard of — without waiting for a
new IFME release and without recompiling anything.

```jsonc
{
  "GUID": "deadbeef-0265-0265-0265-026502650265",
  "Name": "x265 MultiCoreWare (GCC 14.2.0)",
  "Version": "4.1+1-32e25ff",
  "X64": true,
  "Format": [ "mp4", "mkv", "m2ts", "ts" ],
  "Author": { "Developer": "MultiCoreWare", "URL": "http://msystem.waw.pl/x265/" },
  "Video": { /* presets, tunes, rate-control modes, CLI arguments */ }
}
```

On startup IFME checks each plugin against your machine, skips ones built for
the wrong architecture, and can test-run each encoder so a broken binary is
caught before you rely on it.

Plugins shipped by default:

| Category | Encoders |
|---|---|
| **Video** | x264, x265, SVT-AV1, uvg266 and vvenc (H.266/VVC), libvpx (VP8/VP9), Xvid, MPEG-1/2, WMV 8 |
| **Audio** | Nero AAC, Fraunhofer FDK AAC, exhale (xHE-AAC/USAC), Opus, Ogg Vorbis, FLAC, AC-3, MP2 (TwoLAME), MP3 (LAME), WMA v2, WAV |
| **Hardware** | NVENC and Intel Quick Sync via [rigaya](https://github.com/rigaya); NVENC, Quick Sync and AMD AMF via FFmpeg |
| **Tools** | FFmpeg (32/64-bit), MP4Box |

## Why it still looks like this

The interface is Win32 and GDI+ on purpose. It keeps IFME's own footprint small
so RAM, CPU cache and threads go to the encoder, which is where they actually
matter. A modern UI stack would cost hundreds of megabytes while x265 is trying
to saturate every core. IFME's job is to stay out of the encoder's way.

## Download

| Source | Notes |
|---|---|
| **[SourceForge](https://sourceforge.net/projects/ifme/files/latest/download)** | Always current — the recommended download |
| [GitHub Releases](https://github.com/Anime4000/IFME/releases/latest) | Usually current |
| [VideoHelp](https://www.videohelp.com/software/Internet-Friendly-Media-Encoder) | Third-party mirror |
| [SoftPedia](https://www.softpedia.com/get/Multimedia/Video/Encoders-Converter-DIVX-Related/Internet-Friendly-Media-Encoder.shtml) | Third-party mirror |

> [!NOTE]
> SourceForge always has the newest build. GitHub Releases occasionally lags
> behind, and the third-party mirrors are not maintained by this project.

### Running it

- **Windows** — run `IFME.exe`
- **Linux** — run `ifme.sh` from a terminal

## System requirements

IFME requires a CPU with **AVX** support. It will refuse to start without it.
**AVX2** is strongly recommended; without it you will get a warning at startup
and some encoders will be unavailable.

<details>
<summary><b>Windows</b></summary>

- 64-bit Windows 10 or later
- [Microsoft Visual C++ Redistributable (all versions)](https://www.techpowerup.com/download/visual-c-redistributable-runtime-package-all-in-one/)
- [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) — only needed on Windows 7; later versions include it

</details>

<details>
<summary><b>Linux</b></summary>

- `mono-complete` (Mono runtime)

> [!IMPORTANT]
> IFME itself runs on Linux, but the bundled `Plugins` folder contains Windows
> binaries. You will need to build your own `ffmpeg`, `x264`, `x265` and so on,
> and point the plugin manifests at them.

</details>

<details>
<summary><b>Recommended hardware</b></summary>

| | QHD and below | UHD and above |
|---|---|---|
| CPU | Intel Core i9 (10th gen) or AMD Ryzen 7 3700X | same or better |
| RAM | 8 GB DDR4 | 16 GB DDR4 |
| Disk | — | ~70 GB free for UHD/HDR temporary files |

32-bit is no longer supported. High resolutions and high bit depths need more
address space than a 32-bit process can provide.

</details>

## Format support

<details>
<summary><b>Video codecs and containers</b></summary>

| Video  | .avi | .mp4 | .mkv | .ts | .m2ts | .mpg | .mpeg | .webm | .wmv |
|--------|------|------|------|-----|-------|------|-------|-------|------|
| MPEG-1 | ✅    | ✅    | ✅    | ✅   | ✅     | ✅    | ✅     |       |      |
| MPEG-2 | ✅    | ✅    | ✅    | ✅   | ✅     | ✅    | ✅     |       |      |
| WMV-2  |      |      | ✅    |     |       |      |       |       | ✅    |
| H.263  | ✅    | ✅    | ✅    | ✅   | ✅     | ✅    | ✅     |       |      |
| H.264  | ✅    | ✅    | ✅    | ✅   | ✅     |      |       |       |      |
| H.265  |      | ✅    | ✅    | ✅   | ✅     |      |       |       |      |
| H.266  |      | ✅    |      |     |       |      |       |       |      |
| AV1    |      |      | ✅    |     |       |      |       | ✅     |      |
| VP8    |      |      | ✅    |     |       |      |       | ✅     |      |
| VP9    |      |      | ✅    |     |       |      |       | ✅     |      |

</details>

<details>
<summary><b>Audio codecs and containers</b></summary>

| Audio          | .avi | .mp2 | .mp3 | .m4a | .mp4 | .mkv | .ogg | .opus | .flac | .wma | .wav |
|----------------|------|------|------|------|------|------|------|-------|-------|------|------|
| MPEG Layer II  | ✅    | ✅    |      | ✅    | ✅    | ✅    |      |       |       |      |      |
| MPEG Layer III | ✅    |      | ✅    | ✅    | ✅    | ✅    |      |       |       |      |      |
| WMA            |      |      |      |      |      | ✅    |      |       |       | ✅    |      |
| AAC            | ✅    |      |      | ✅    | ✅    | ✅    |      |       |       |      |      |
| AC-3           |      |      |      | ✅    | ✅    | ✅    |      |       |       |      |      |
| OGG            |      |      |      |      |      | ✅    | ✅    | ✅     |       |      |      |
| Opus           |      |      |      |      |      | ✅    |      | ✅     |       |      |      |
| USAC           |      |      |      | ✅    | ✅    | ✅    |      |       |       |      |      |
| FLAC           |      |      |      |      |      | ✅    |      |       | ✅     |      |      |
| WAV            | ✅    |      |      |      |      | ✅    |      |       |       |      | ✅    |

</details>

## Hardware acceleration

IFME ships hardware encoding for H.264, H.265 and AV1 through FFmpeg and
[rigaya](https://github.com/rigaya)'s NVEncC and QSVEncC.

> [!WARNING]
> Hardware encoders are much faster, but at a given bitrate a CPU encoder will
> almost always look better. If quality per bit matters more than speed, stay on
> the CPU encoders.

## Building from source

```
git clone https://github.com/Anime4000/IFME.git
```

Open `IFME.sln` in **Visual Studio 2022** and build. `Newtonsoft.Json` is
restored from NuGet automatically.

The solution targets .NET Framework 4.8 (C# 9) and contains:

| Project | Purpose |
|---|---|
| `IFME` | WinForms front end and encoding pipeline |
| `IFME.FFmpeg` | Media probing and parsing |
| `IFME.OSManager` | Platform abstraction — process control, CPU features, paths |
| `NDesk.Options` | Command-line parsing |

## Contributing

Issues and pull requests are welcome.

**Translations** are especially appreciated. Language files live in
[`IFME/i18n`](IFME/i18n) as plain JSON — copy `en-US.json`, translate the
values, and keep the keys and `{0}` placeholders exactly as they are.

Current translations: English, 日本語, 简体中文, Bahasa Malaysia.

## License

| | |
|---|---|
| **Source code and binaries** | [GPL-2.0](http://choosealicense.com/licenses/gpl-2.0/) |
| **Mascots and artwork** | [CC BY-NC 4.0](http://creativecommons.org/licenses/by-nc/4.0/) — drawn by [53C](http://53c.deviantart.com/) and [adeq](https://www.facebook.com/liyana.0426), property of the IFME Project |

> [!IMPORTANT]
> Multimedia codecs may be covered by patents in your jurisdiction, and using
> them may require royalty payments. Please read [PATENTS.md](PATENTS.md)
> before installing.

## Support the project

IFME is free and developed in spare time. If it saves you effort, a small
donation helps keep it maintained.

**[Donate via PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U)**

If you donate, let me know on
[Facebook](https://www.facebook.com/internetfriendlymediaencoder) or
[Twitter](https://twitter.com/Anime4000) and you will be credited in the
*Hall of Fame* and in the program's About screen.

## Background

IFME started in 2011, when I was a college student trying to compress FRAPS game
recordings with x264 for archiving. Friends liked how simple and light it was,
and that turned into the Internet Friendly Media Encoder.

It has been maintained ever since — the plugin system arrived so that the
encoders could keep moving forward without the application having to be rewritten
each time.
