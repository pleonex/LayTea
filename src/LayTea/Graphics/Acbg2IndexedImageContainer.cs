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
namespace SceneGate.Games.ProfessorLayton.Graphics
{
    using System;
    using Texim.Images;
    using Texim.Pixels;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Converter for binary ACB or ACG into a container of indexed images.
    /// </summary>
    public class Acbg2IndexedImageContainer : IConverter<IBinary, NodeContainerFormat>
    {
        /// <summary>
        /// Convert the binary ACB or ACG into a container of indexed images.
        /// </summary>
        /// <param name="source">The binary stream to convert.</param>
        /// <returns>The container with indexed images.</returns>
        public NodeContainerFormat Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);

            var stamp = reader.ReadString(4);
            if (stamp != "ACB " && stamp != "ACG ") {
                throw new FormatException($"Invalid stamp: {stamp}");
            }

            int numImages = reader.ReadInt32();
            var container = new NodeContainerFormat();
            for (int i = 0; i < numImages; i++) {
                long pointerPos = 8 + (i * 4);
                reader.Stream.Position = pointerPos;
                uint pointer = reader.ReadUInt32();

                reader.Stream.Position = pointerPos + pointer;
                var image = ReadImage(reader);

                container.Root.Add(new Node($"Image{i}", image));
            }

            return container;
        }

        private IndexedImage ReadImage(DataReader reader)
        {
            uint flags = reader.ReadUInt32();
            int width = (int)(flags & 0x7FF);
            int height = (int)((flags >> 11) & 0x7FF);
            int numColors = (int)((flags >> 22) & 0x1FF);
            bool unknown = (flags >> 31) != 0;
            if (unknown) {
                throw new FormatException("Check this out!");
            }

            int size = reader.ReadInt32();
            var binaryPixels = reader.ReadBytes(size);
            IndexedPixel[] pixels = (numColors <= 16)
                ? binaryPixels.DecodePixelsAs<Indexed4Bpp>()
                : binaryPixels.DecodePixelsAs<Indexed8Bpp>();

            var tileSwizzling = new TileSwizzling<IndexedPixel>(width);
            pixels = tileSwizzling.Unswizzle(pixels);

            return new IndexedImage(width, height, pixels);
        }
    }
}
