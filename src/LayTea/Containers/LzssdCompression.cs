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
    public class LzssdCompression :
        IInitializer<DataStream>, IConverter<BinaryFormat, BinaryFormat>
    {
        private const int MaxDistance = (1 << 11) - 1;
        private const int MinSequenceLength = 2;
        private const int MaxSequenceLength = ((1 << 4) + MinSequenceLength) - 1;
        private const int MaxRawLength = (1 << 7) - 1;

        private DataStream requestedOutput;

        /// <summary>
        /// Initializes the converter with the output stream to write the
        /// compressed data.
        /// </summary>
        /// <param name="parameters">The output stream.</param>
        /// <remarks>
        /// <p>The given output stream is only used once in the following call
        /// to Convert. Following calls will use a new default (memory) stream.</p>
        /// </remarks>
        public void Initialize(DataStream parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.Disposed)
                throw new ObjectDisposedException(nameof(parameters));

            requestedOutput = parameters;
        }

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
            int inputLen = (int)source.Length;
            int inputPos = 0;
            var input = new byte[inputLen];
            source.Position = 0;
            source.Read(input);

            // Worst case is every byte encoded individually as raw (not going to happen)
            // that would take twice the size (token + byte).
            int outputPos = 0;
            var output = new byte[inputLen * 2];

            int currentRawSequence = 0;
            while (inputPos < inputLen) {
                (int sequencePos, int sequenceLen) = FindSequence(input, inputPos);
                if (sequenceLen >= MinSequenceLength) {
                    if (currentRawSequence > 0) {
                        EncodeRaw(input, inputPos, output, ref outputPos, currentRawSequence);
                        currentRawSequence = 0;
                    }

                    EncodeCopy(output, ref outputPos, sequencePos, sequenceLen);
                    inputPos += sequenceLen;
                } else {
                    currentRawSequence++;
                    inputPos++;

                    if (currentRawSequence == MaxRawLength) {
                        EncodeRaw(input, inputPos, output, ref outputPos, currentRawSequence);
                        currentRawSequence = 0;
                    }
                }
            }

            if (currentRawSequence > 0) {
                EncodeRaw(input, inputPos, output, ref outputPos, currentRawSequence);
            }

            DataStream outputStream;
            if (requestedOutput != null) {
                outputStream = requestedOutput;
                outputStream.Write(output, 0, outputPos);

                // Don't use it in the next conversion.
                requestedOutput = null;
            } else {
                outputStream = DataStreamFactory.FromArray(output, 0, outputPos);
            }

            return outputStream;
        }

        private static (int pos, int length) FindSequence(ReadOnlySpan<byte> input, int inputPos)
        {
            int inputLen = input.Length;

            int maxPattern = (inputPos + MaxSequenceLength > inputLen)
                ? (inputLen - inputPos)
                : MaxSequenceLength;
            if (maxPattern < MinSequenceLength) {
                return (-1, -1);
            }

            int windowPos = (inputPos > MaxDistance) ? inputPos - MaxDistance : 0;
            int windowLen = (windowPos + MaxDistance > inputPos)
                ? (inputPos - windowPos)
                : MaxDistance;
            if (windowLen == 0) {
                return (-1, -1);
            }

            var window = input.Slice(windowPos, windowLen + (maxPattern - 1));
            var pattern = input.Slice(inputPos, maxPattern);
            int bestLength = -1;
            int bestPos = -1;
            for (int pos = windowLen - 1; pos >= 0; pos--) {
                int length = 0;
                for (; length < maxPattern; length++) {
                    if (pattern[length] != window[pos + length]) {
                        break;
                    }
                }

                if (length > bestLength) {
                    bestLength = length;
                    bestPos = pos;
                    if (length == MaxSequenceLength) {
                        return (windowLen - bestPos, bestLength);
                    }
                }
            }

            return (windowLen - bestPos, bestLength);
        }

        private void EncodeRaw(ReadOnlySpan<byte> input, int inputPos, Span<byte> output, ref int outputPos, int sequenceLength)
        {
            output[outputPos++] = (byte)(sequenceLength << 1);
            for (int i = 0; i < sequenceLength; i++) {
                output[outputPos++] = input[inputPos + i - sequenceLength];
            }
        }

        private void EncodeCopy(Span<byte> output, ref int outputPos, int copyPos, int copyLength)
        {
            int flags = ((copyLength - MinSequenceLength) << 12) | (copyPos << 1) | 1;
            output[outputPos++] = (byte)(flags & 0xFF);
            output[outputPos++] = (byte)(flags >> 8);
        }
    }
}
