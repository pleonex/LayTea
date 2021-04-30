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
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Text serializer and deserializer for messages.
    /// </summary>
    internal class MessageTextSerializer
    {
        private const int NextBoxId = 0xF1;
        private readonly StringBuilder textBuilder = new StringBuilder();

        /// <summary>
        /// Serializes a message into a collection of messages.
        /// </summary>
        /// <param name="message">The message to serialize.</param>
        /// <returns>The collection of serialized text dialogs.</returns>
        public IEnumerable<string> Serialize(Message message)
        {
            textBuilder.Clear();
            foreach (var element in message.Content) {
                if (element is MessageRawText rawText) {
                    AppendRawText(rawText.Text);
                } else if (element is MessageFunction { Id: NextBoxId }) {
                    // Split entries with the function next box too.
                    yield return textBuilder.ToString();
                    textBuilder.Clear();
                } else if (element is MessageFunction function) {
                    AppendFunctions(function);
                }
            }

            if (message.QuestionOptions != null) {
                AppendOptions(message.QuestionOptions);
            }

            yield return textBuilder.ToString();
        }

        /// <summary>
        /// Deserialize a text into a message.
        /// </summary>
        /// <param name="text">The text to deserialize.</param>
        /// <returns>The deserialized message.</returns>
        public Message Deserialize(IEnumerable<string> text)
        {
            var message = new Message();

            var textList = text.ToArray();
            for (int i = 0; i < textList.Length; i++) {
                Deserialize(message, textList[i]);

                if (i + 1 < textList.Length) {
                    message.Add(MessageFunction.FromMnemonic("next_box", null));
                }
            }

            return message;
        }

        private void AppendRawText(string raw)
        {
            // Escape the tokens we use for function definitions
            // And invalid PO chars.
            raw = raw.Replace("{", "{{").Replace("}", "}}");
            textBuilder.Append(raw);
        }

        private void AppendFunctions(MessageFunction function)
        {
            if (function.Argument.HasValue) {
                textBuilder.AppendFormat("{{@{0}({1})}}", function.Mnemonic, function.Argument.Value);
            } else {
                textBuilder.AppendFormat("{{@{0}()}}", function.Mnemonic);
            }
        }

        private void AppendOptions(QuestionOptions options)
        {
            textBuilder.AppendFormat(
                "{{@options(default:{0},selected:{1})",
                options.DefaultIndex,
                options.PreSelectedIndex);
            textBuilder.AppendLine();

            foreach (var option in options.Options) {
                textBuilder.AppendFormat("- {0}: {1}", option.Item1, option.Item2);
                textBuilder.AppendLine();
            }

            textBuilder.Append("}");
        }

        private void Deserialize(Message msg, ReadOnlySpan<char> text)
        {
            while (text.Length > 0) {
                int read = DeserializeSegment(msg, text);
                text = text.Slice(read);
            }
        }

        private int DeserializeSegment(Message msg, ReadOnlySpan<char> text)
        {
            int startFunction = text.IndexOf("{@");
            if (startFunction == 0) {
                int endFunction = text.Slice(startFunction + 2).IndexOf("}");
                var function = text.Slice(startFunction + 2, endFunction);
                ParseFunction(msg, function);
                return startFunction + 2 + endFunction + 1;
            }

            int rawLength = (startFunction == -1) ? text.Length : startFunction;
            var raw = text.Slice(0, rawLength).ToString();
            string rawText = raw.Replace("{{", "{").Replace("}}", "}");
            msg.Add(rawText);

            return rawLength;
        }

        private void ParseFunction(Message msg, ReadOnlySpan<char> text)
        {
            int nameIndex = text.IndexOf('(');
            var name = text.Slice(0, nameIndex);
            if (name.Equals("options", StringComparison.InvariantCulture)) {
                msg.QuestionOptions = ParseOption(text.Slice(nameIndex));
                return;
            }

            short? arg = null;
            if (text.Length > name.Length + 2) {
                var argText = text.Slice(nameIndex + 1)[..^1];
                arg = short.Parse(argText);
            }

            msg.Add(MessageFunction.FromMnemonic(name.ToString(), arg));
        }

        private QuestionOptions ParseOption(ReadOnlySpan<char> text)
        {
            var options = new QuestionOptions();

            text = text.Slice("(default:".Length);
            options.DefaultIndex = text[0] - 0x30;

            text = text.Slice(1 + ",selected:".Length);
            options.PreSelectedIndex = text[0] - 0x30;

            string[] lines = text.Slice(2).ToString()
                .Replace("\r", string.Empty)
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++) {
                int index = lines[i][2] - 0x30;
                string line = lines[i][5..];
                options.Options.Add((index, line));
            }

            return options;
        }
    }
}
