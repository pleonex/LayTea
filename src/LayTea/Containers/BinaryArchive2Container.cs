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
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Converter for binary ARCHIVE into containers.
    /// </summary>
    public class BinaryArchive2Container : IConverter<IBinary, NodeContainerFormat>
    {
        /// <summary>
        /// Convert a binary ARCHIVE into a container.
        /// </summary>
        /// <param name="source">The binary stream to convert.</param>
        /// <returns>The new container.</returns>
        public NodeContainerFormat Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);
            var container = new NodeContainerFormat();

            // The offset to the first file give us the number of entries
            int numFiles = reader.ReadInt32() / 8;
            source.Stream.Position = 0;

            for (int i = 0; i < numFiles; i++) {
                string name = $"file{i}";
                uint offset = reader.ReadUInt32();
                uint size = reader.ReadUInt32();
                container.Root.Add(NodeFactory.FromSubstream(name, source.Stream, offset, size));
            }

            return container;
        }
    }
}
