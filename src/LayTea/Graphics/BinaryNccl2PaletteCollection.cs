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
    using Yarhl.IO;

    /// <summary>
    /// Converter for NCCL binary palettes into a collection of palettes.
    /// </summary>
    public class BinaryNccl2PaletteCollection : NDeserializer<PaletteCollection>
    {
        /// <inheritdoc/>
        public override string Stamp => "NCCL";

        /// <inheritdoc/>
        public override int SupportedVersion => 0x01_00;

        /// <inheritdoc/>
        protected override PaletteCollection ReadSection(DataReader reader, PaletteCollection model, string id, int size)
        {
            if (id == "PALT") {
                int colorsPerPalette = reader.ReadInt32();
                int numPalettes = reader.ReadInt32();
                for (int j = 0; j < numPalettes; j++) {
                    var colors = reader.ReadColors<Bgr555>(colorsPerPalette);
                    model.Palettes.Add(new Palette(colors));
                }
            } else if (id == "CMNT") {
                var comment = reader.ReadString();
                if (!string.IsNullOrEmpty(comment)) {
                    throw new FormatException("Unknown CMNT section");
                }
            } else {
                reader.Stream.Position += size - 8;
                throw new FormatException("Unknown section: " + id);
            }

            return model;
        }
    }
}
