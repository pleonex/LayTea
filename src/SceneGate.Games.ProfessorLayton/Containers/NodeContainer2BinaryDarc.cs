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
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Converter node containers into binary DARC streams.
    /// </summary>
    public class NodeContainer2BinaryDarc : IConverter<NodeContainerFormat, BinaryFormat>
    {
        private const string Stamp = "DARC";

        /// <summary>
        /// Converts a container into a binary DARC format.
        /// </summary>
        /// <param name="source">The container to pack.</param>
        /// <returns>The binary DARC format with the children.</returns>
        /// <remarks>
        /// <para>The converter expects to find children with binary formats.</para>
        /// </remarks>
        public BinaryFormat Convert(NodeContainerFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            writer.Write(Stamp, false);
            writer.Write(source.Root.Children.Count);

            // Placeholder of pointers so we can write the file data at the same time
            writer.WriteTimes(0x00, 4 * source.Root.Children.Count);
            for (int i = 0; i < source.Root.Children.Count; i++) {
                var child = source.Root.Children[i];
                if (child.Format is not IBinary) {
                    binary.Dispose();
                    throw new FormatException($"Child '{child.Name}' does NOT have binary format");
                }

                // Write pointer
                binary.Stream.Position = 8 + (i * 4);
                uint relativeOffset = (uint)(binary.Stream.Length - binary.Stream.Position);
                writer.Write(relativeOffset);

                // Write length and file data
                binary.Stream.Seek(0, SeekOrigin.End);
                writer.Write((uint)child.Stream.Length);
                child.Stream.WriteTo(binary.Stream);
                writer.WritePadding(0x00, 4);
            }

            return binary;
        }
    }
}
