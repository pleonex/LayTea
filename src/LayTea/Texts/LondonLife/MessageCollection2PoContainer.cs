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
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    /// <summary>
    /// Converter for a message collection into PO.
    /// </summary>
    public sealed class MessageCollection2PoContainer : IConverter<MessageCollection, NodeContainerFormat>
    {
        private const int NextBoxId = 0xF1;

        /// <summary>
        /// Convert a message collection into PO.
        /// </summary>
        /// <param name="source">The messages to convert.</param>
        /// <returns>The PO with the messages.</returns>
        public NodeContainerFormat Convert(MessageCollection source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var container = new NodeContainerFormat();

            IEnumerable<MessageSection> sections = LoadSectionInfo();

            MessageSection currentSection = sections.First();
            Po currentPo = null;
            var text = new StringBuilder();
            for (int i = 0; i < source.Messages.Count; i++) {
                var matchSection = sections.FirstOrDefault(s => s.Start == i);
                if (matchSection != null) {
                    currentSection = matchSection;
                    currentPo = new Po(new PoHeader("london-life", "SceneGate", "en"));
                    container.Root.Add(new Node(matchSection.Name, currentPo));
                }

                AddMessage(currentPo, source.Messages, i, text, currentSection);
            }

            return container;
        }

        private static IEnumerable<MessageSection> LoadSectionInfo()
        {
            string resourceName = "SceneGate.Games.ProfessorLayton.Texts.LondonLife.msg_sections.yml";
            var assembly = typeof(MessageCollection2PoContainer).Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) {
                throw new InvalidOperationException("Missing section YML");
            }

            using var reader = new StreamReader(stream);
            return new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build()
                .Deserialize<IEnumerable<MessageSection>>(reader);
        }

        private static void AddMessage(Po po, Collection<Message> messages, int index, StringBuilder text, MessageSection section)
        {
            text.Clear();
            int boxCount = 0;

            foreach (var element in messages[index].Content) {
                if (element is MessageRawText rawText) {
                    AppendRawText(text, rawText.Text);
                } else if (element is MessageFunction { Id: NextBoxId }) {
                    // Split entries with the function next box too.
                    FlushEntry(po, index, boxCount++, text, section, messages);
                } else if (element is MessageFunction function) {
                    AppendFunctions(text, function);
                }
            }

            if (messages[index].QuestionOptions != null) {
                AppendOptions(text, messages[index].QuestionOptions);
            }

            FlushEntry(po, index, boxCount, text, section, messages);
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

        private static void FlushEntry(Po po, int msgId, int boxCount, StringBuilder text, MessageSection section, Collection<Message> messages)
        {
            if (text.Length == 0) {
                return;
            }

            string entryType = null;
            if (section.Entries.Count > 0) {
                int itemIdx = (msgId - section.Start) % section.Entries.Count;
                entryType = section.Entries[itemIdx];
            }

            if (section.Start == 0) {
                int nameIdx = 10865 + ((msgId / 6) * 2);
                string name = (messages[nameIdx].Content[0] as MessageRawText).Text;
                entryType = $"Name: {name}, {entryType}";
            }

            var entry = new PoEntry {
                Context = $"{msgId}:{boxCount}",
                Original = text.ToString(),
                ExtractedComments = entryType,
            };
            po.Add(entry);
            text.Clear();
        }
    }
}
