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
    /// Converter to compress binary streams in a DENC container.
    /// </summary>
    public class DencCompression :
        IInitializer<DencCompressionKind>, IConverter<BinaryFormat, BinaryFormat>
    {
        private const string Stamp = "DENC";
        private const int HeaderLength = 0x10;
        private DencCompressionKind compressionKind;

        /// <summary>
        /// Initializes a new instance of the <see cref="DencCompression" /> class.
        /// </summary>
        public DencCompression()
        {
            compressionKind = DencCompressionKind.None;
        }

        /// <summary>
        /// Initialize the converter with the compression to use.
        /// </summary>
        /// <param name="parameters">The kind of compression to use.</param>
        public void Initialize(DencCompressionKind parameters)
        {
            compressionKind = parameters;
        }

        /// <summary>
        /// Compress a binary stream in a DENC container.
        /// </summary>
        /// <param name="source">The stream to compress.</param>
        /// <returns>The new DENC container with the compressed binary.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var compressed = new BinaryFormat();
            var writer = new DataWriter(compressed.Stream);

            writer.Write(Stamp, nullTerminator: false);
            writer.Write((uint)source.Stream.Length);
            writer.Write(GetCompressionName(), nullTerminator: false);
            writer.Write(0x00); // compression length placeholder

            CompressStream(source.Stream, compressed.Stream);

            compressed.Stream.Position = 0x0C;
            long compressedLength = compressed.Stream.Length - HeaderLength;
            writer.Write((uint)compressedLength);

            return compressed;
        }

        private string GetCompressionName() =>
            compressionKind switch {
                DencCompressionKind.None => "NULL",
                DencCompressionKind.Lzss => "LZSS",
                _ => throw new NotImplementedException("Unknown compression"),
            };

        private void CompressStream(DataStream input, DataStream output)
        {
            if (compressionKind == DencCompressionKind.None) {
                input.WriteTo(output);
            } else if (compressionKind == DencCompressionKind.Lzss) {
                var lzss = new LzssdCompression();
                lzss.Initialize(output);
                lzss.Convert(input);
            }
        }
    }
}