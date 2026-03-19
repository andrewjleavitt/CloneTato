#!/usr/bin/env python3
"""Parse Aseprite files to extract animation tag names and frame ranges.
Usage: python3 read_aseprite_tags.py [path_or_directory]
If no path given, scans assets/stranded/"""

import struct, os, sys

def read_aseprite_tags(path):
    """Read animation tag names from an Aseprite file."""
    tags = []
    try:
        with open(path, 'rb') as f:
            data = f.read()

        if len(data) < 128:
            return None

        magic = struct.unpack_from('<H', data, 4)[0]
        if magic != 0xA5E0:
            return None

        frames = struct.unpack_from('<H', data, 6)[0]
        width = struct.unpack_from('<H', data, 8)[0]
        height = struct.unpack_from('<H', data, 10)[0]

        pos = 128  # skip header
        for frame_idx in range(frames):
            if pos + 16 > len(data):
                break
            frame_size = struct.unpack_from('<I', data, pos)[0]
            frame_end = pos + frame_size
            old_chunks = struct.unpack_from('<H', data, pos + 6)[0]
            new_chunks = struct.unpack_from('<I', data, pos + 12)[0]
            num_chunks = new_chunks if new_chunks > 0 else old_chunks

            chunk_pos = pos + 16
            for _ in range(num_chunks):
                if chunk_pos + 6 > len(data):
                    break
                chunk_size = struct.unpack_from('<I', data, chunk_pos)[0]
                chunk_type = struct.unpack_from('<H', data, chunk_pos + 4)[0]

                if chunk_type == 0x2018:  # Tags
                    tp = chunk_pos + 6
                    num_tags = struct.unpack_from('<H', data, tp)[0]
                    tp += 2 + 8
                    for _ in range(num_tags):
                        if tp + 17 > len(data):
                            break
                        from_frame = struct.unpack_from('<H', data, tp)[0]
                        to_frame = struct.unpack_from('<H', data, tp + 2)[0]
                        tp += 4 + 1 + 2 + 6 + 1 + 3
                        name_len = struct.unpack_from('<H', data, tp)[0]
                        tp += 2
                        name = data[tp:tp+name_len].decode('utf-8', errors='replace')
                        tp += name_len
                        tags.append((name, from_frame, to_frame))

                chunk_pos += chunk_size

            if tags:
                break
            pos = frame_end

    except Exception as e:
        return f"Error: {e}"

    return tags

def scan_directory(base_path):
    for root, dirs, files in sorted(os.walk(base_path)):
        for fname in sorted(files):
            if fname.endswith('.aseprite'):
                path = os.path.join(root, fname)
                rel = os.path.relpath(path, base_path)
                result = read_aseprite_tags(path)
                if result and isinstance(result, list) and len(result) > 0:
                    print(f"\n=== {rel} ===")
                    for tag_name, start, end in result:
                        frames = end - start + 1
                        print(f"  [{start:3d}-{end:3d}] ({frames:2d}f) {tag_name}")

if __name__ == '__main__':
    target = sys.argv[1] if len(sys.argv) > 1 else 'assets/stranded'
    if os.path.isfile(target):
        result = read_aseprite_tags(target)
        if result and isinstance(result, list):
            for name, start, end in result:
                print(f"  [{start:3d}-{end:3d}] ({end-start+1:2d}f) {name}")
    else:
        scan_directory(target)
