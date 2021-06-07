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
    using Yarhl.Media.Text;

    /// <summary>
    /// In-place converter for text replacement in PO formats.
    /// </summary>
    public class PoTableReplacer :
        IInitializer<PoTableReplacerParams>, IConverter<Po, Po>
    {
        private PoTableReplacerParams parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoTableReplacer" /> class.
        /// </summary>
        /// <remarks>
        /// By default it does not apply any replacement. Call <see cref="Initialize(PoTableReplacerParams)" />
        /// to specify the replacements.
        /// </remarks>
        public PoTableReplacer()
        {
            parameters = new PoTableReplacerParams {
                Replacer = new Replacer(),
                TransformForward = true,
            };
        }

        /// <summary>
        /// Initializes the converter.
        /// </summary>
        /// <param name="parameters">The converter parameters.</param>
        public void Initialize(PoTableReplacerParams parameters)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Replaces the Original and Translated text in the PO format with the provided table.
        /// </summary>
        /// <param name="source">The PO to replace text.</param>
        /// <returns>The same PO instance after text replacement.</returns>
        public Po Convert(Po source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var entry in source.Entries) {
                if (parameters.TransformForward) {
                    entry.Original = parameters.Replacer.TransformForward(entry.Original);
                    entry.Translated = parameters.Replacer.TransformForward(entry.Translated);
                } else {
                    entry.Original = parameters.Replacer.TransformBackward(entry.Original);
                    entry.Translated = parameters.Replacer.TransformBackward(entry.Translated);
                }
            }

            return source;
        }
    }
}
