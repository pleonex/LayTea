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
namespace SceneGate.Games.ProfessorLayton.Texts.LondonLife
{
    using System;
    using System.Collections.Generic;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    /// <summary>
    /// Factory for replacer tables of London Life.
    /// </summary>
    public static class ReplacerFactory
    {
        private static readonly Dictionary<LondonLifeRegion, string> ResourceInfo =
            new Dictionary<LondonLifeRegion, string> {
                { LondonLifeRegion.Usa, "table_us.txt" },
            };

        /// <summary>
        /// Get the replacer table resource.
        /// </summary>
        /// <param name="region">The game region.</param>
        /// <returns>The replacer table.</returns>
        public static Replacer GetReplacer(LondonLifeRegion region)
        {
            if (!ResourceInfo.ContainsKey(region)) {
                throw new NotSupportedException($"Unsupported region: {region}");
            }

            string resourceName = ResourceInfo[region];
            string resourcePath = $"{typeof(ReplacerFactory).Namespace}.{resourceName}";
            var assembly = typeof(ReplacerFactory).Assembly;

            var stream = assembly.GetManifestResourceStream(resourcePath);
            using var binary = new BinaryFormat(stream);

            var deserializer = new ReplacerDeserializer();
            return deserializer.Convert(binary);
        }
    }
}
