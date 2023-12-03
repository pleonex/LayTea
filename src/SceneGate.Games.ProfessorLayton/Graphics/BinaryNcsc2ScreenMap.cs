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
using Texim.Compressions.Nitro;
using Yarhl.IO;

namespace SceneGate.Games.ProfessorLayton.Graphics
{
    /// <summary>
    /// Converter for binary NCSC stream into screen map.
    /// </summary>
    public class BinaryNcsc2ScreenMap : NDeserializer<Ncsc>
    {
        /// <inheritdoc/>
        public override string Stamp => "NCSC";

        /// <inheritdoc/>
        public override int SupportedVersion => 0x01_02;

        /// <inheritdoc/>
        protected override Ncsc ReadSection(DataReader reader, Ncsc model, string id, int size)
        {
            if (id == "SCRN") {
                int width = reader.ReadInt32() * 8;
                int height = reader.ReadInt32() * 8;
                reader.ReadUInt64(); // 2x unknown uint
                var maps = reader.ReadMapInfos(width * height / 64);

                return new Ncsc(width, height, maps);
            } else if (id == "LINK") {
                model.ImageName = reader.ReadString();
            }

            return model;
        }
    }
}
