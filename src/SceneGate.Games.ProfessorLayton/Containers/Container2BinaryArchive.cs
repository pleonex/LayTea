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
    /// Converter for node containers into a binary ARCHIVE.
    /// </summary>
    public class Container2BinaryArchive : IConverter<NodeContainerFormat, BinaryFormat>
    {
        /// <summary>
        /// Convert a node container into a binary ARCHIVE.
        /// </summary>
        /// <param name="source">The container of nodes to convert.</param>
        /// <returns>The new in-memory binary ARCHIVE.</returns>
        public BinaryFormat Convert(NodeContainerFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            // write the placeholders of the file entry so we can write the file data too.
            writer.WriteTimes(0, source.Root.Children.Count * 8);
            for (int i = 0; i < source.Root.Children.Count; i++) {
                var child = source.Root.Children[i];
                if (child.Format is not IBinary) {
                    binary.Dispose();
                    throw new FormatException($"The child '{child.Name}' has not a binary format");
                }

                binary.Stream.Position = i * 8;
                writer.Write((uint)binary.Stream.Length);
                writer.Write((uint)child.Stream.Length);

                binary.Stream.Seek(0, SeekOrigin.End);
                child.Stream.WriteTo(binary.Stream);
            }

            return binary;
        }
    }
}
