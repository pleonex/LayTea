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
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.Media.Text;

    /// <summary>
    /// Converter for a message collection into PO.
    /// </summary>
    public sealed class MessageCollection2Po : IConverter<MessageCollection, Po>
    {
        private const int NextBoxId = 0xF1;

        /// <summary>
        /// Convert a message collection into PO.
        /// </summary>
        /// <param name="source">The messages to convert.</param>
        /// <returns>The PO with the messages.</returns>
        public Po Convert(MessageCollection source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var po = new Po(new PoHeader("london-life", "SceneGate", "en"));

            var text = new StringBuilder();
            for (int i = 0; i < source.Messages.Count; i++) {
                Message msg = source.Messages[i];
                int boxCount = 0;
                foreach (var element in msg.Content) {
                    if (element is MessageRawText rawText) {
                        AppendRawText(text, rawText.Text);
                    } else if (element is MessageFunction { Id: NextBoxId }) {
                        // Split entries with the function next box too.
                        FlushEntry(po, i, boxCount++, text);
                    } else if (element is MessageFunction function) {
                        AppendFunctions(text, function);
                    }
                }

                if (msg.QuestionOptions != null) {
                    AppendOptions(text, msg.QuestionOptions);
                }

                FlushEntry(po, i, boxCount, text);
            }

            return po;
        }

        private static void AppendRawText(StringBuilder builder, string raw)
        {
            // Escape the tokens we use for function definitions
            // And invalid PO chars.
            raw = raw
                .Replace("{", "{{").Replace("}", "}}")
                .Replace(@"\", @"\\");
            builder.Append(raw);
        }

        private static void AppendFunctions(StringBuilder builder, MessageFunction function)
        {
            if (function.Argument.HasValue) {
                builder.AppendFormat("{{{0}({1})}}", function.Mnemonic, function.Argument.Value);
            } else {
                builder.AppendFormat("{{{0}()}}", function.Mnemonic);
            }
        }

        private static void AppendOptions(StringBuilder builder, QuestionOptions options)
        {
            builder.AppendFormat(
                "{{options(default:{0},selected={1})}}",
                options.DefaultIndex,
                options.PreSelectedIndex);
            builder.AppendLine();

            foreach (var option in options.Options) {
                builder.AppendFormat("- {0}: {1}", option.Item1, option.Item2);
                builder.AppendLine();
            }
        }

        private static void FlushEntry(Po po, int msgId, int boxCount, StringBuilder text)
        {
            if (text.Length == 0) {
                return;
            }

            var entry = new PoEntry {
                Context = $"{msgId}:{boxCount}",
                Original = text.ToString(),
            };
            po.Add(entry);
            text.Clear();
        }
    }
}
