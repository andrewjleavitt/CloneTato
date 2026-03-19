#!/usr/bin/env python3
"""Split a grid-based sprite sheet into individual horizontal strip PNGs per animation.
Uses Aseprite tag data (frame ranges) + grid dimensions to extract rows of frames.

Usage: python3 split_grid_sheet.py <aseprite_file> <cell_w> <cell_h> <output_dir>
"""

import struct, os, sys, zlib

def read_aseprite_tags(path):
    tags = []
    with open(path, 'rb') as f:
        data = f.read()
    if len(data) < 128: return []
    magic = struct.unpack_from('<H', data, 4)[0]
    if magic != 0xA5E0: return []
    pos = 128
    frames = struct.unpack_from('<H', data, 6)[0]
    for _ in range(frames):
        if pos + 16 > len(data): break
        frame_size = struct.unpack_from('<I', data, pos)[0]
        frame_end = pos + frame_size
        old_chunks = struct.unpack_from('<H', data, pos + 6)[0]
        new_chunks = struct.unpack_from('<I', data, pos + 12)[0]
        num_chunks = new_chunks if new_chunks > 0 else old_chunks
        chunk_pos = pos + 16
        for _ in range(num_chunks):
            if chunk_pos + 6 > len(data): break
            chunk_size = struct.unpack_from('<I', data, chunk_pos)[0]
            chunk_type = struct.unpack_from('<H', data, chunk_pos + 4)[0]
            if chunk_type == 0x2018:
                tp = chunk_pos + 6
                num_tags = struct.unpack_from('<H', data, tp)[0]
                tp += 10
                for _ in range(num_tags):
                    if tp + 17 > len(data): break
                    from_frame = struct.unpack_from('<H', data, tp)[0]
                    to_frame = struct.unpack_from('<H', data, tp + 2)[0]
                    tp += 17
                    name_len = struct.unpack_from('<H', data, tp)[0]
                    tp += 2
                    name = data[tp:tp+name_len].decode('utf-8', errors='replace')
                    tp += name_len
                    tags.append((name, from_frame, to_frame))
            chunk_pos += chunk_size
        if tags: break
        pos = frame_end
    return tags

def read_png(path):
    """Read PNG into raw RGBA pixel data."""
    import subprocess
    # Use sips to convert to raw RGBA, or fall back to manual PNG decode
    # Actually let's just use the minimal approach with zlib
    with open(path, 'rb') as f:
        data = f.read()

    # Parse PNG
    assert data[:8] == b'\x89PNG\r\n\x1a\n', "Not a PNG file"
    pos = 8
    width = height = bit_depth = color_type = 0
    idat_chunks = []
    palette = None

    while pos < len(data):
        length = struct.unpack('>I', data[pos:pos+4])[0]
        chunk_type = data[pos+4:pos+8]
        chunk_data = data[pos+8:pos+8+length]
        pos += 12 + length

        if chunk_type == b'IHDR':
            width = struct.unpack('>I', chunk_data[0:4])[0]
            height = struct.unpack('>I', chunk_data[4:8])[0]
            bit_depth = chunk_data[8]
            color_type = chunk_data[9]
        elif chunk_type == b'PLTE':
            palette = chunk_data
        elif chunk_type == b'tRNS':
            trns = chunk_data
        elif chunk_type == b'IDAT':
            idat_chunks.append(chunk_data)
        elif chunk_type == b'IEND':
            break

    compressed = b''.join(idat_chunks)
    raw = zlib.decompress(compressed)

    # Decode scanlines
    pixels = bytearray(width * height * 4)

    if color_type == 6:  # RGBA
        bpp = 4
        stride = 1 + width * bpp
        for y in range(height):
            filter_type = raw[y * stride]
            row_start = y * stride + 1
            for x in range(width * bpp):
                val = raw[row_start + x]
                if filter_type == 1:  # Sub
                    a = pixels[(y * width * 4) + x - bpp] if x >= bpp else 0
                    val = (val + a) & 0xFF
                elif filter_type == 2:  # Up
                    b = pixels[((y - 1) * width * 4) + x] if y > 0 else 0
                    val = (val + b) & 0xFF
                elif filter_type == 3:  # Average
                    a = pixels[(y * width * 4) + x - bpp] if x >= bpp else 0
                    b = pixels[((y - 1) * width * 4) + x] if y > 0 else 0
                    val = (val + (a + b) // 2) & 0xFF
                elif filter_type == 4:  # Paeth
                    a = pixels[(y * width * 4) + x - bpp] if x >= bpp else 0
                    b = pixels[((y - 1) * width * 4) + x] if y > 0 else 0
                    c = pixels[((y - 1) * width * 4) + x - bpp] if y > 0 and x >= bpp else 0
                    p = a + b - c
                    pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
                    if pa <= pb and pa <= pc: pr = a
                    elif pb <= pc: pr = b
                    else: pr = c
                    val = (val + pr) & 0xFF
                pixels[y * width * 4 + x] = val
    else:
        raise ValueError(f"Unsupported color type {color_type}. Only RGBA (6) supported. Convert first.")

    return width, height, bytes(pixels)

def write_png(path, width, height, rgba_data):
    """Write RGBA pixel data as PNG."""
    def make_chunk(chunk_type, data):
        import binascii
        chunk = chunk_type + data
        crc = binascii.crc32(chunk) & 0xFFFFFFFF
        return struct.pack('>I', len(data)) + chunk + struct.pack('>I', crc)

    # Build raw scanlines with filter 0 (None)
    raw = bytearray()
    for y in range(height):
        raw.append(0)  # filter: None
        offset = y * width * 4
        raw.extend(rgba_data[offset:offset + width * 4])

    compressed = zlib.compress(bytes(raw))

    ihdr = struct.pack('>IIBBBBB', width, height, 8, 6, 0, 0, 0)

    with open(path, 'wb') as f:
        f.write(b'\x89PNG\r\n\x1a\n')
        f.write(make_chunk(b'IHDR', ihdr))
        f.write(make_chunk(b'IDAT', compressed))
        f.write(make_chunk(b'IEND', b''))

def extract_strip(pixels, img_w, img_h, cell_w, cell_h, cols, start_frame, num_frames):
    """Extract frames from grid into a horizontal strip."""
    strip = bytearray(cell_w * num_frames * cell_h * 4)
    for i in range(num_frames):
        frame_idx = start_frame + i
        src_col = frame_idx % cols
        src_row = frame_idx // cols

        for y in range(cell_h):
            src_y = src_row * cell_h + y
            if src_y >= img_h: continue
            for x in range(cell_w):
                src_x = src_col * cell_w + x
                if src_x >= img_w: continue
                src_off = (src_y * img_w + src_x) * 4
                dst_off = (y * cell_w * num_frames + i * cell_w + x) * 4
                strip[dst_off:dst_off+4] = pixels[src_off:src_off+4]

    return bytes(strip), cell_w * num_frames, cell_h

if __name__ == '__main__':
    if len(sys.argv) < 5:
        print(f"Usage: {sys.argv[0]} <aseprite_file> <cell_w> <cell_h> <output_dir> [png_override]")
        sys.exit(1)

    ase_path = sys.argv[1]
    cell_w = int(sys.argv[2])
    cell_h = int(sys.argv[3])
    output_dir = sys.argv[4]
    png_path = sys.argv[5] if len(sys.argv) > 5 else None

    # Get animation tags
    tags = read_aseprite_tags(ase_path)
    if not tags:
        print("No tags found in Aseprite file")
        sys.exit(1)

    # Find corresponding PNG (without shadow variant)
    if png_path is None:
        base_dir = os.path.dirname(ase_path)
        for f in os.listdir(base_dir):
            if f.endswith('without shadow.png'):
                png_path = os.path.join(base_dir, f)
                break
        if png_path is None:
            # Try any PNG that isn't a gun sprite
            for f in os.listdir(base_dir):
                if f.endswith('.png') and 'gun' not in f.lower() and 'shadow' not in f.lower():
                    png_path = os.path.join(base_dir, f)
                    break

    if png_path is None or not os.path.exists(png_path):
        print(f"No suitable PNG found for {ase_path}")
        sys.exit(1)

    print(f"Reading {png_path}...")
    img_w, img_h, pixels = read_png(png_path)
    cols = img_w // cell_w
    print(f"  {img_w}x{img_h}, {cell_w}x{cell_h} cells, {cols} columns")

    os.makedirs(output_dir, exist_ok=True)

    for tag_name, start, end in tags:
        num_frames = end - start + 1
        safe_name = tag_name.replace('/', '-').replace(' ', '_').lower()
        strip_data, strip_w, strip_h = extract_strip(pixels, img_w, img_h, cell_w, cell_h, cols, start, num_frames)
        out_path = os.path.join(output_dir, f"{safe_name}.png")
        write_png(out_path, strip_w, strip_h, strip_data)
        print(f"  {safe_name}.png: {strip_w}x{strip_h} ({num_frames} frames)")

    print("Done!")
