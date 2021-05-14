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
using System;
using System.IO;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace SceneGate.Games.ProfessorLayton.Graphics
{
    /// <summary>
    /// Generic binary deserializer for graphic formats starting with N.
    /// </summary>
    /// <typeparam name="T">The output type of the deserialization.</typeparam>
    public abstract class NDeserializer<T> : IConverter<IBinary, T>
        where T : new()
    {
        private const ushort LittleEndian = 0xFEFF;
        private const ushort BigEndian = 0xFFFE;

        /// <summary>
        /// Gets the stamp of the binary format.
        /// </summary>
        public abstract string Stamp { get; }

        /// <summary>
        /// Gets the supported version of the deserializer.
        /// </summary>
        public abstract int SupportedVersion { get; }

        /// <summary>
        /// Converts a binary stream into a model.
        /// </summary>
        /// <param name="source">The stream to deserialize.</param>
        /// <returns>The new model.</returns>
        public T Convert(IBinary source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);

            int numSections = ReadHeader(reader);

            T model = new T();
            for (int i = 0; i < numSections; i++) {
                source.Stream.PushCurrentPosition();

                string id = reader.ReadString(4);
                int size = reader.ReadInt32();
                model = ReadSection(reader, model, id, size);

                source.Stream.PopPosition();
                source.Stream.Seek(size, SeekOrigin.Current);
            }

            return model;
        }

        /// <summary>
        /// Read a section of the binary stream.
        /// </summary>
        /// <param name="reader">The reader of the binary stream.</param>
        /// <param name="model">The new model.</param>
        /// <param name="id">The ID of the section.</param>
        /// <param name="size">The size of the section.</param>
        /// <returns>The same or new model after reading a section.</returns>
        protected abstract T ReadSection(DataReader reader, T model, string id, int size);

        private int ReadHeader(DataReader reader)
        {
            string stamp = reader.ReadString(4);
            if (stamp != Stamp) {
                throw new FormatException($"Invalid stamp {stamp}");
            }

            ushort endianness = reader.ReadUInt16();
            if (endianness == BigEndian) {
                reader.Endianness = EndiannessMode.BigEndian;
            } else if (endianness != LittleEndian) {
                throw new FormatException($"Unknown endianness: {endianness:X4}");
            }

            ushort version = reader.ReadUInt16();
            if (version != SupportedVersion) {
                throw new FormatException($"Unknown version: {version:X4}");
            }

            reader.Stream.Position += 6; // file size (uint) + header size (ushort)

            ushort numSections = reader.ReadUInt16();
            return numSections;
        }
    }
}
