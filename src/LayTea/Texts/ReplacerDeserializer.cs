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
namespace SceneGate.Games.ProfessorLayton.Texts
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    /// <summary>
    /// Deserializer for replacer text tables.
    /// </summary>
    public class ReplacerDeserializer : IConverter<IBinary, Replacer>
    {
        /// <summary>
        /// Convert a binary replacer table into a Replacer.
        /// </summary>
        /// <param name="source">The binary text table.</param>
        /// <returns>The replacer object.</returns>
        public Replacer Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var replacer = new Replacer();

            source.Stream.Position = 0;
            var reader = new TextDataReader(source.Stream);
            while (reader.Stream.Position < reader.Stream.Length) {
                string line = reader.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(line) || line[0] == '#') {
                    continue;
                }

                if (line.Length != 3) {
                    throw new FormatException("Invalid line: " + line);
                }

                replacer.Add(line[0..1], line[2..3]);
            }

            return replacer;
        }
    }
}
