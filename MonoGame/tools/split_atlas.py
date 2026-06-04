#!/usr/bin/env python3
"""Разрезает атлас CocosSharp/TexturePacker (.plist + .png) на отдельные PNG.

Способ C миграции спрайтов: каждый кадр атласа сохраняется отдельным файлом
с тем же именем, что и в .plist. Повёрнутые кадры (textureRotated) учитываются.
"""
import plistlib
import re
import sys
from pathlib import Path

from PIL import Image

RECT_RE = re.compile(r"\{\{(\d+),(\d+)\},\{(\d+),(\d+)\}\}")


def parse_rect(s: str):
    m = RECT_RE.fullmatch(s.strip())
    if not m:
        raise ValueError(f"Не удалось разобрать textureRect: {s!r}")
    x, y, w, h = (int(g) for g in m.groups())
    return x, y, w, h


def main(plist_path: str, png_path: str, out_dir: str):
    plist_path, png_path, out_dir = Path(plist_path), Path(png_path), Path(out_dir)
    out_dir.mkdir(parents=True, exist_ok=True)

    with open(plist_path, "rb") as f:
        data = plistlib.load(f)

    atlas = Image.open(png_path).convert("RGBA")
    frames = data["frames"]
    print(f"Кадров в атласе: {len(frames)}")

    for name, info in frames.items():
        x, y, w, h = parse_rect(info["textureRect"])
        rotated = info.get("textureRotated", False)
        # При повороте в атласе хранится w/h, повёрнутые на 90°.
        box = (x, y, x + (h if rotated else w), y + (w if rotated else h))
        sprite = atlas.crop(box)
        if rotated:
            sprite = sprite.rotate(-90, expand=True)
        out_path = out_dir / name  # имя кадра уже содержит .png
        sprite.save(out_path)
        print(f"  {name:24s} {w}x{h}{' (rotated)' if rotated else ''} -> {out_path}")

    print(f"Готово: {len(frames)} файлов в {out_dir}")


if __name__ == "__main__":
    main(*sys.argv[1:4])
