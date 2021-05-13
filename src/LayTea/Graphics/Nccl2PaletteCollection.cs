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
    using Texim.Colors;
    using Texim.Palettes;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter for NCCL binary palettes into a model.
    /// </summary>
    public class Nccl2PaletteCollection : IConverter<IBinary, PaletteCollection>
    {
        private const string Stamp = "NCCL";
        private const ushort LittleEndian = 0xFEFF;
        private const ushort BigEndian = 0xFFFE;
        private const ushort Version = 0x01_00;

        /// <summary>
        /// Convert a binary palette into a palette collection model.
        /// </summary>
        /// <param name="source">The binary palette stream.</param>
        /// <returns>The new palette collection model.</returns>
        public PaletteCollection Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);

            string stamp = reader.ReadString(4);
            if (stamp != Stamp) {
                throw new FormatException($"Invalid stamp {stamp}");
            }

            ushort endianness = reader.ReadUInt16();
            if (endianness == BigEndian) {
                reader.Endianness = EndiannessMode.BigEndian;
            } else if (endianness != LittleEndian) {
                throw new FormatException($"Unknown endianness: {endianness:X4}");
            }

            ushort version = reader.ReadUInt16();
            if (version != Version) {
                throw new FormatException($"Unknown version: {version:X4}");
            }

            source.Stream.Position += 6; // file size (uint) + header size (ushort)

            var collection = new PaletteCollection();
            ushort numSections = reader.ReadUInt16();
            for (int i = 0; i < numSections; i++) {
                string id = reader.ReadString(4);
                uint size = reader.ReadUInt32();

                if (id == "PALT") {
                    int colorsPerPalette = reader.ReadInt32();
                    int numPalettes = reader.ReadInt32();
                    for (int j = 0; j < numPalettes; j++) {
                        var colors = reader.ReadColors<Bgr555>(colorsPerPalette);
                        collection.Palettes.Add(new Palette(colors));
                    }
                } else if (id == "CMNT") {
                    var comment = reader.ReadString();
                    if (!string.IsNullOrEmpty(comment)) {
                        throw new FormatException("Oh check this out!");
                    }
                } else {
                    reader.Stream.Position += size - 8;
                    throw new FormatException("Oh check this out!");
                }
            }

            return collection;
        }
    }
}
