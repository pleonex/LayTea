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
namespace SceneGate.Games.ProfessorLayton.Containers
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter for binary DENC compressed streams into uncompressed streams.
    /// </summary>
    /// <remarks>
    /// <para>Supported compressions: NULL, LZSSD.</para>
    /// </remarks>
    public class DencDecompression : IConverter<BinaryFormat, BinaryFormat>
    {
        private const string Stamp = "DENC";
        private const int HeaderLength = 0x10;

        /// <summary>
        /// Decompress a binary DENC stream.
        /// </summary>
        /// <param name="source">The compressed stream with DENC.</param>
        /// <returns>The decompressed stream.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);

            string stamp = reader.ReadString(4);
            if (stamp != Stamp) {
                throw new FormatException($"Invalid stamp: {stamp}");
            }

            reader.ReadUInt32(); // decompressed length
            string algorithm = reader.ReadString(4);
            uint compressedLength = reader.ReadUInt32();

            using var substream = new DataStream(source.Stream, HeaderLength, compressedLength);
            return GetDecompressedBinary(substream, algorithm);
        }

        private BinaryFormat GetDecompressedBinary(DataStream stream, string algorithm)
        {
            DataStream decompressed = algorithm switch {
                "NULL" => new DataStream(stream, 0, stream.Length),
                "LZSS" => new LzssdDecompression().Convert(stream),
                _ => throw new NotImplementedException($"'{algorithm}' not implemented")
            };

            return new BinaryFormat(decompressed);
        }
    }
}
