// Copyright (c) 2021 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using Texim.Colors;
using Texim.Images;
using Texim.Pixels;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace SceneGate.Games.ProfessorLayton.Graphics
{
    /// <summary>
    /// Converter for binary NCCG streams into a container of indexed images.
    /// </summary>
    public class BinaryNccg2IndexedImage : NDeserializer<IndexedImage>
    {
        /// <inheritdoc/>
        public override string Stamp => "NCCG";

        /// <inheritdoc/>
        public override int SupportedVersion => 0x01_00;

        /// <inheritdoc/>
        protected override IndexedImage ReadSection(DataReader reader, IndexedImage model, string id, int size)
        {
            if (id == "CHAR") {
                int width = reader.ReadInt32() * 8;
                int height = reader.ReadInt32() * 8;
                bool is8bpp = reader.ReadInt32() != 0;
                var pixelBytes = reader.ReadBytes(size - 0x14);

                IIndexedPixelEncoding colorEncoding = is8bpp ? Indexed8Bpp.Instance : Indexed4Bpp.Instance;
                var pixels = colorEncoding.Decode(pixelBytes);

                var swizzling = new TileSwizzling<IndexedPixel>(width);
                pixels = swizzling.Unswizzle(pixels);

                model = new IndexedImage(width, height, pixels);
            } else if (id == "ATTR") {
                // no need to read, same info
            } else if (id == "LINK") {
                // string with palette name
            } else if (id == "CMNT") {
                // I guess some dev comments
            }

            return model;
        }
    }
}
