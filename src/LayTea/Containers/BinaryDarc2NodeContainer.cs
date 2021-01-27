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
    /// Converter for binary DARC streams into node containers.
    /// The DARC streams contains DENC (un)compressed files.
    /// </summary>
    public class BinaryDarc2NodeContainer : IConverter<BinaryFormat, NodeContainerFormat>
    {
        private const string Stamp = "DARC";

        /// <summary>
        /// Converts a binary DARC format into a container.
        /// </summary>
        /// <param name="source">The binary DARC format.</param>
        /// <returns>The container with the DARC content.</returns>
        /// <remarks>
        /// <para>The format does not provide names for the files, so the
        /// converter sets name as "fileX.denc".</para>
        /// </remarks>
        public NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);
            var container = new NodeContainerFormat();

            string stamp = reader.ReadString(4);
            if (stamp != Stamp) {
                throw new FormatException($"Invalid stamp: '{stamp}'");
            }

            int numFiles = reader.ReadInt32();
            for (int i = 0; i < numFiles; i++) {
                long offset = reader.Stream.Position + reader.ReadUInt32();
                source.Stream.PushToPosition(offset);
                uint length = reader.ReadUInt32();
                source.Stream.PopPosition();

                var child = NodeFactory.FromSubstream(
                    $"file{i}.denc",
                    source.Stream,
                    offset + 4,
                    length);
                container.Root.Add(child);
            }

            return container;
        }
    }
}
