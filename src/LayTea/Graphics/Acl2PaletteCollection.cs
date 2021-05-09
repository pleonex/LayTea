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
namespace LayTea.Graphics
{
    using System;
    using Texim.Colors;
    using Texim.Palettes;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter for binary ACL palettes into a model.
    /// </summary>
    public class Acl2PaletteCollection : IConverter<IBinary, IPaletteCollection>
    {
        private const string Stamp = "ACL ";

        /// <summary>
        /// Convert a binary ACL palette into a model.
        /// </summary>
        /// <param name="source">The binary ACL palette stream.</param>
        /// <returns>The new palette collection model.</returns>
        public IPaletteCollection Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);

            string stamp = reader.ReadString(4);
            if (stamp != Stamp) {
                throw new FormatException($"Invalid stamp: {stamp}");
            }

            var collection = new PaletteCollection();
            int numPalettes = reader.ReadInt32();
            for (int i = 0; i < numPalettes; i++) {
                long pointerPos = source.Stream.Position;
                long pointer = reader.ReadUInt16() + pointerPos;

                source.Stream.PushToPosition(pointer);
                int numColors = reader.ReadInt32();
                var colors = reader.ReadColors<Bgr555>(numColors);
                source.Stream.PopPosition();

                var palette = new Palette(colors);
                collection.Palettes.Add(palette);
            }

            return collection;
        }
    }
}
