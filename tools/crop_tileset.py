#!/usr/bin/env python3
"""Crop rectangular regions from a PNG tileset and save as individual PNGs.
Usage: python3 crop_tileset.py <input.png> <output_dir> <name,x,y,w,h> [name,x,y,w,h] ...

Example: python3 crop_tileset.py tileset.png out/ gravel1,0,0,64,64 grass1,80,10,32,16
"""
import struct, zlib, os, sys, binascii

def read_png_rgba(path):
    with open(path, 'rb') as f:
        data = f.read()
    assert data[:8] == b'\x89PNG\r\n\x1a\n'
    pos = 8
    width = height = 0
    idat_chunks = []
    while pos < len(data):
        length = struct.unpack('>I', data[pos:pos+4])[0]
        ct = data[pos+4:pos+8]
        cd = data[pos+8:pos+8+length]
        pos += 12 + length
        if ct == b'IHDR':
            width = struct.unpack('>I', cd[0:4])[0]
            height = struct.unpack('>I', cd[4:8])[0]
            color_type = cd[9]
        elif ct == b'IDAT': idat_chunks.append(cd)
        elif ct == b'IEND': break
    raw = zlib.decompress(b''.join(idat_chunks))
    bpp = 4 if color_type == 6 else 3
    has_alpha = color_type == 6
    pixels = bytearray(width * height * 4)
    stride = 1 + width * bpp
    for y in range(height):
        ft = raw[y * stride]
        rs = y * stride + 1
        for x in range(width * bpp):
            val = raw[rs + x]
            if ft == 1:
                a = pixels[y * width * 4 + x - bpp] if x >= bpp else 0
                val = (val + a) & 0xFF
            elif ft == 2:
                b = pixels[(y-1) * width * 4 + x] if y > 0 else 0
                val = (val + b) & 0xFF
            elif ft == 3:
                a = pixels[y * width * 4 + x - bpp] if x >= bpp else 0
                b = pixels[(y-1) * width * 4 + x] if y > 0 else 0
                val = (val + (a + b) // 2) & 0xFF
            elif ft == 4:
                a = pixels[y * width * 4 + x - bpp] if x >= bpp else 0
                b = pixels[(y-1) * width * 4 + x] if y > 0 else 0
                c = pixels[(y-1) * width * 4 + x - bpp] if y > 0 and x >= bpp else 0
                p = a + b - c
                pa, pb, pc = abs(p-a), abs(p-b), abs(p-c)
                pr = a if pa <= pb and pa <= pc else (b if pb <= pc else c)
                val = (val + pr) & 0xFF
            if has_alpha:
                pixels[y * width * 4 + x] = val
            else:
                px_idx = x // 3
                ch = x % 3
                pixels[y * width * 4 + px_idx * 4 + ch] = val
                if ch == 2:
                    pixels[y * width * 4 + px_idx * 4 + 3] = 255
    return width, height, bytes(pixels)

def write_png(path, w, h, rgba):
    def chunk(ct, d):
        c = ct + d
        return struct.pack('>I', len(d)) + c + struct.pack('>I', binascii.crc32(c) & 0xFFFFFFFF)
    raw = bytearray()
    for y in range(h):
        raw.append(0)
        raw.extend(rgba[y*w*4:(y+1)*w*4])
    with open(path, 'wb') as f:
        f.write(b'\x89PNG\r\n\x1a\n')
        f.write(chunk(b'IHDR', struct.pack('>IIBBBBB', w, h, 8, 6, 0, 0, 0)))
        f.write(chunk(b'IDAT', zlib.compress(bytes(raw))))
        f.write(chunk(b'IEND', b''))

def crop(pixels, img_w, img_h, x, y, w, h):
    out = bytearray(w * h * 4)
    for row in range(h):
        sy = y + row
        if sy >= img_h: continue
        for col in range(w):
            sx = x + col
            if sx >= img_w: continue
            si = (sy * img_w + sx) * 4
            di = (row * w + col) * 4
            out[di:di+4] = pixels[si:si+4]
    return bytes(out)

if __name__ == '__main__':
    if len(sys.argv) < 4:
        print(f"Usage: {sys.argv[0]} <input.png> <output_dir> <name,x,y,w,h> ...")
        sys.exit(1)
    img_w, img_h, pixels = read_png_rgba(sys.argv[1])
    outdir = sys.argv[2]
    os.makedirs(outdir, exist_ok=True)
    print(f"Source: {img_w}x{img_h}")
    for spec in sys.argv[3:]:
        parts = spec.split(',')
        name, x, y, w, h = parts[0], int(parts[1]), int(parts[2]), int(parts[3]), int(parts[4])
        cropped = crop(pixels, img_w, img_h, x, y, w, h)
        out_path = os.path.join(outdir, f"{name}.png")
        write_png(out_path, w, h, cropped)
        print(f"  {name}.png: {w}x{h} from ({x},{y})")
    print("Done!")
