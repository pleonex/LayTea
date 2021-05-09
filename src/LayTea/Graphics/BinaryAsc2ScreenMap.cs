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
    using Texim.Compressions.Nitro;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter for binary ASC into a screen map model.
    /// </summary>
    public class BinaryAsc2ScreenMap : IConverter<IBinary, ScreenMap>
    {
        /// <summary>
        /// Convert a binary ASC stream into a model.
        /// </summary>
        /// <param name="source">The ASC stream to convert.</param>
        /// <returns>The new screen map model.</returns>
        public ScreenMap Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);

            string stamp = reader.ReadString(4);
            if (stamp != "ASC ") {
                throw new FormatException($"Invalid stamp: {stamp}");
            }

            int width = reader.ReadInt16();
            int height = reader.ReadInt16();

            int numMaps = reader.ReadInt16();
            if (numMaps != 1) {
                throw new FormatException($"Num maps is not 1? {numMaps}");
            }

            int mapOffset = reader.ReadUInt16();
            int mapSize = reader.ReadInt32();

            reader.Stream.Position = mapOffset;
            var maps = reader.ReadMapInfos(mapSize / 2);

            return new ScreenMap(width, height) {
                Maps = maps,
            };
        }
    }
}
