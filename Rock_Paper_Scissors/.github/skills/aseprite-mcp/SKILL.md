---
name: aseprite-mcp
description: "Use when a request involves Aseprite sprite creation, pixel-art editing, animation, onion-skin checks, GIF/PNG export, or spritesheet generation through MCP tools."
---

# Aseprite MCP workflow

Use this skill for pixel-art and animation work that should be performed in Aseprite via MCP tools instead of manual file edits.

## When to use

- Creating a new sprite canvas (`.aseprite`)
- Drawing or editing pixel art in layers/frames
- Building animations (idle, walk, attack, effects)
- Exporting GIF/PNG/spritesheets for Unity assets
- Inspecting frames (`view_sprite`, `compare_frames`, `lint_sprite`)

## Project defaults

- Working output folder: `D:\Unity\repos\Rock_Paper_Scissors\Rock_Paper_Scissors\Temp`
- Aseprite executable path:
  `D:\SteamLibrary\steamapps\common\Aseprite\Aseprite.exe`
- MCP server config file:
  `D:\Unity\repos\Rock_Paper_Scissors\Rock_Paper_Scissors\.vscode\mcp.json`

## Procedure

1. Confirm MCP readiness:
   - Ensure `.vscode/mcp.json` contains an `aseprite` server entry.
   - If tools fail with `WinError 2`, verify `ASEPRITE_PATH` points to a valid `Aseprite.exe`.
2. Create or open the target sprite:
   - Use `create_canvas` for new sprites.
   - Keep dimensions explicit (for example, 32x32, 48x48, 128x128).
3. Build animation structure before detail:
   - Add frames first, then set frame durations.
   - Keep key poses readable before adding polish.
4. Draw in passes:
   - Block silhouettes with simple primitives.
   - Refine with pixel-level edits or row-based tools.
   - Keep consistent palette choices across frames.
5. Verify each pass:
   - Use `view_sprite` to inspect exact pixels.
   - Use `compare_frames`/`render_onion_skin` for motion continuity.
   - Use `lint_sprite` to catch isolated stray pixels.
6. Export deliverables:
   - `export_animation` for GIF previews.
   - `export_frame` or `export_sprite` for PNG outputs.
   - `export_spritesheet` when Unity-ready atlases are requested.
7. Confirm outputs exist at expected paths and report them back.

## Guardrails

- Prefer Aseprite MCP tools for sprite/animation tasks instead of direct binary file manipulation.
- Use explicit absolute Windows paths for output files.
- Keep changes surgical: only modify the requested sprite files and exports.
