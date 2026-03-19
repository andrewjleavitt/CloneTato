#!/usr/bin/env python3
"""Check PNG sprite strip dimensions and calculate frame counts.
Usage: python3 sprite_dimensions.py <directory> [frame_width]
Scans for .png files and shows dimensions. If frame_width given, shows frame count."""

import struct, os, sys

def get_png_dimensions(path):
    with open(path, 'rb') as fp:
        fp.read(16)
        w = struct.unpack('>I', fp.read(4))[0]
        h = struct.unpack('>I', fp.read(4))[0]
    return w, h

if __name__ == '__main__':
    target = sys.argv[1] if len(sys.argv) > 1 else '.'
    frame_w = int(sys.argv[2]) if len(sys.argv) > 2 else None

    if os.path.isfile(target):
        files = [target]
    else:
        files = []
        for root, dirs, fnames in sorted(os.walk(target)):
            for f in sorted(fnames):
                if f.endswith('.png'):
                    files.append(os.path.join(root, f))

    for path in files:
        try:
            w, h = get_png_dimensions(path)
            name = os.path.basename(path)
            if frame_w:
                fc = w / frame_w
                clean = "ok" if w % frame_w == 0 else "REMAINDER"
                print(f"  {w:5d}x{h:<4d}  {fc:5.1f} frames  {clean:10s}  {name}")
            else:
                print(f"  {w:5d}x{h:<4d}  {name}")
        except Exception as e:
            print(f"  ERROR: {e}  {path}")
