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
    /// Compression algorithm for LZSS variant found in DENC formats.
    /// </summary>
    public class LzssdCompression : IConverter<BinaryFormat, BinaryFormat>
    {
        /// <summary>
        /// Compress a LZSS-DENC compressed stream.
        /// </summary>
        /// <param name="source">The decompressed stream.</param>
        /// <returns>The compressed stream with LZSS-DENC.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var compressed = Convert(source.Stream);
            return new BinaryFormat(compressed);
        }

        /// <summary>
        /// Compress a LZSS-DENC compressed stream.
        /// </summary>
        /// <param name="source">The decompressed stream.</param>
        /// <returns>The compressed stream with LZSS-DENC.</returns>
        public DataStream Convert(DataStream source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // So far, only small files use this encoding (few KB).
            // For performance reasons, it will be faster if we load the full file into memory.
            // Worst case is every byte encoded individually as raw (not going to happen)
            // that would take twice the size (token + byte).
            int outPos = 0;
            byte[] output = new byte[source.Length * 2];

            int inPos = 0;
            byte[] input = new byte[source.Length];
            source.Position = 0;
            source.Read(input, 0, input.Length);

            int currentRawSequence = 0;
            while (inPos < input.Length) {
                (int index, int length) = FindSequence(input, inPos);
                if (length >= 2) {
                    if (currentRawSequence > 0) {
                        output[outPos++] = (byte)(currentRawSequence << 1);
                        for (int i = 0; i < currentRawSequence; i++) {
                            output[outPos++] = input[inPos - currentRawSequence + i];
                        }

                        currentRawSequence = 0;
                    }

                    int flags = ((length - 2) << 12) | (index << 1) | 1;
                    output[outPos++] = (byte)(flags & 0xFF);
                    output[outPos++] = (byte)(flags >> 8);
                    inPos += length;
                } else {
                    currentRawSequence++;
                    inPos++;

                    if (currentRawSequence == 127) {
                        output[outPos++] = (byte)(currentRawSequence << 1);
                        for (int i = 0; i < currentRawSequence; i++) {
                            output[outPos++] = input[inPos - currentRawSequence + i];
                        }

                        currentRawSequence = 0;
                    }
                }
            }

            if (currentRawSequence > 0) {
                output[outPos++] = (byte)(currentRawSequence << 1);
                for (int i = 0; i < currentRawSequence; i++) {
                    output[outPos++] = input[inPos - currentRawSequence + i];
                }
            }

            return DataStreamFactory.FromArray(output, 0, outPos);
        }

        private (int, int) FindSequence(byte[] input, int inPos)
        {
            int distance = (inPos > 2047) ? 2047 : inPos;
            int bestLength = 0;
            int bestPos = 0;
            for (int i = 1; i <= distance; i++) {
                int j = 0;
                for (; j < 17 && inPos + j < input.Length; j++) {
                    if (input[inPos + j] != input[inPos - i + j]) {
                        break;
                    }
                }

                if (j > bestLength) {
                    bestLength = j;
                    bestPos = i;
                }
            }

            return (bestPos, bestLength);
        }
    }
}
