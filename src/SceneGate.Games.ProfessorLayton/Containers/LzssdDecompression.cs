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
    /// Decompression algorithm for LZSS variant found in DENC formats.
    /// </summary>
    public class LzssdDecompression :
        IConverter<IBinary, BinaryFormat>,
        IConverter<DataStream, DataStream>
    {
        /// <summary>
        /// Decompress a LZSS-DENC compressed stream.
        /// </summary>
        /// <param name="source">The compressed stream with LZSS-DENC.</param>
        /// <returns>The decompressed stream.</returns>
        public BinaryFormat Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var decompressed = Convert(source.Stream);
            return new BinaryFormat(decompressed);
        }

        /// <summary>
        /// Decompress a LZSS-DENC compressed stream.
        /// </summary>
        /// <param name="source">The compressed stream with LZSS-DENC.</param>
        /// <returns>The decompressed stream.</returns>
        public DataStream Convert(DataStream source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataStream output = new DataStream();

            source.Position = 0;
            var reader = new DataReader(source);
            while (!source.EndOfStream) {
                byte token = reader.ReadByte();
                if ((token & 0x01) == 0) {
                    // Copy from input a sequence of bytes.
                    int length = token >> 1;
                    CopyRaw(reader, length, output);
                } else {
                    // Repeat a sequence of bytes from output.
                    int flags = token | (reader.ReadByte() << 8);
                    int pos = (flags >> 1) & 0x7FF;
                    int length = (flags >> 12) + 2;
                    CopyOutput(pos, length, output);
                }
            }

            return output;
        }

        private void CopyRaw(DataReader reader, int length, DataStream output)
        {
            output.Write(reader.ReadBytes(length));
        }

        private void CopyOutput(int startBackPos, int length, DataStream output)
        {
            if (startBackPos == 0) {
                throw new InvalidOperationException("Invalid copy position 0");
            }

            long startCopyPos = output.Position - startBackPos;
            long startOutPos = output.Position;
            int pastDataLength = (int)(startOutPos - startCopyPos);
            byte[] buffer = new byte[1024];

            int written = 0;
            while (written < length) {
                int toCopy = (length - written < pastDataLength) ? length - written : pastDataLength;

                output.Position = startCopyPos + written;
                output.Read(buffer, 0, toCopy);

                output.Position = startOutPos + written;
                output.Write(buffer, 0, toCopy);

                written += toCopy;
            }
        }
    }
}
