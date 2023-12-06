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
    using System.IO;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Decompression algorithm for LZX implemented in N devices.
    /// </summary>
    public class NitroLzxDecompression :
        IConverter<IBinary, BinaryFormat>,
        IConverter<Stream, Stream>
    {
        private const byte Stamp = 0x11;
        private const byte MinSequenceLength = 2;

        /// <summary>
        /// Decompress a LZSS-compressed stream.
        /// </summary>
        /// <param name="source">The stream to convert.</param>
        /// <returns>The decompressed in-memory stream.</returns>
        public BinaryFormat Convert(IBinary source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            var decompressed = Convert(source.Stream);
            return new BinaryFormat(decompressed);
        }

        /// <summary>
        /// Decompress a LZSS-compressed stream.
        /// </summary>
        /// <param name="source">The stream to convert.</param>
        /// <returns>The decompressed in-memory stream.</returns>
        public Stream Convert(Stream source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            source.Position = 0;
            var reader = new DataReader(source);

            int outputLen = ReadHeader(reader);

            // Read and write into arrays as it's faster
            int inputLen = (int)(source.Length - 4);
            int inputPos = 0;
            byte[] input = reader.ReadBytes(inputLen);
            int outputPos = 0;
            byte[] output = new byte[outputLen];

            byte mask = 0;
            byte flags = 0;
            while (inputPos < inputLen) {
                if (mask == 0) {
                    flags = input[inputPos++];
                    mask = 0x80;
                }

                bool isRaw = (flags & mask) == 0;
                mask >>= 1;

                if (isRaw) {
                    output[outputPos++] = input[inputPos++];
                } else {
                    DecodeCopy(input, ref inputPos, output, ref outputPos);
                }
            }

            return DataStreamFactory.FromArray(output, 0, output.Length);
        }

        private static int ReadHeader(DataReader reader)
        {
            uint header = reader.ReadUInt32();
            uint id = header & 0xFF;
            uint outputLen = header >> 8; // max 16 MB
            if (id != Stamp) {
                throw new FormatException("Invalid compression ID");
            }

            return (int)outputLen;
        }

        private static void DecodeCopy(ReadOnlySpan<byte> input, ref int inputPos, Span<byte> output, ref int outputPos)
        {
            // bits 0-11: back position
            // bits 12-X: length
            int flags = input[inputPos++];
            flags = (flags << 8) | input[inputPos++];

            // If length is less than 2, then there is an extra byte
            int minLength = 1;
            int lenFlag = flags >> 12;
            if (lenFlag < MinSequenceLength) {
                flags &= 0xFFF;
                flags = (flags << 8) | input[inputPos++];
                minLength = 0x11;

                // If length was 1, then there is another extra byte
                if (lenFlag == 1) {
                    flags = (flags << 8) | input[inputPos++];
                    minLength = 0x111;
                }
            }

            int len = (flags >> 12) + minLength;
            int pos = (flags & 0xFFF) + 1;
            for (int i = 0; i < len; i++) {
                output[outputPos] = output[outputPos - pos];
                outputPos++;
            }
        }
    }
}
