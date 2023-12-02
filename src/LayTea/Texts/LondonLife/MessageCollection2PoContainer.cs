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
    using System.Linq;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    /// <summary>
    /// Converter for a message collection into PO.
    /// </summary>
    public sealed class MessageCollection2PoContainer :
        IInitializer<LondonLifeRegion>, IConverter<MessageCollection, NodeContainerFormat>
    {
        private readonly StringBuilder contextBuilder = new StringBuilder();
        private readonly MessageTextSerializer textParser = new MessageTextSerializer();
        private MessageContextProvider contextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCollection2PoContainer" /> class.
        /// </summary>
        /// <remarks>
        /// By default uses the USA region. Call <see cref="Initialize(LondonLifeRegion)" /> to change the region.
        /// </remarks>
        public MessageCollection2PoContainer()
        {
            contextProvider = new MessageContextProvider(LondonLifeRegion.Usa);
        }

        /// <summary>
        /// Initializes the converter with the game region.
        /// </summary>
        /// <param name="parameters">The game region.</param>
        public void Initialize(LondonLifeRegion parameters)
        {
            contextProvider = new MessageContextProvider(parameters);
        }

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

            var sections = contextProvider.Sections;
            int sectionIdx = -1;
            int endDialog = -1;
            Po currentPo = null;
            for (int i = 0; i < source.Messages.Count; i++) {
                if (i > endDialog) {
                    sectionIdx++;

                    bool isLastSection = (sectionIdx + 1) == sections.Count;
                    endDialog = isLastSection
                        ? source.Messages.Count
                        : sections[sectionIdx + 1].Start - 1;

                    currentPo = new Po(new PoHeader("london-life", "SceneGate", "en"));
                    container.Root.Add(new Node(sections[sectionIdx].Name, currentPo));
                }

                var entries = CreateEntries(source.Messages, i, sections[sectionIdx]);
                currentPo.Add(entries);
            }

            return container;
        }

        private IEnumerable<PoEntry> CreateEntries(Collection<Message> messages, int index, MessageSection section)
        {
            // Some dialogs ends with "next_box" even when there aren't more boxes
            // in the dialog. As we split by "next_box" we need to preseve that command
            // somehow, so we set a flag in the context.
            bool hasEndStop = (messages[index].Content.Count > 0)
                && (messages[index].QuestionOptions == null)
                && (messages[index].Content[^1] is MessageFunction { Id: 0xF1 });

            var textEntries = textParser.Serialize(messages[index]).ToArray();
            int boxCount = 0;
            for (int i = 0; i < textEntries.Length; i++) {
                var text = textEntries[i];
                if (text.Length == 0) {
                    continue;
                }

                yield return new PoEntry {
                    Context = $"{index}:{boxCount++}:{hasEndStop}",
                    Original = text,
                    ExtractedComments = GetEntryContext(index, section, messages),
                };
            }
        }

        private string GetEntryContext(int msgId, MessageSection section, Collection<Message> messages)
        {
            contextBuilder.Clear();

            int numEntries = section.Entries.Count > 0 ? section.Entries.Count : 1;
            int textIdx = (msgId - section.Start) / numEntries;
            contextBuilder.AppendFormat("Text: {0}", textIdx);

            int nameIdx = contextProvider.GetNameIndex(msgId);
            if (nameIdx != -1) {
                string name = (messages[nameIdx].Content[0] as MessageRawText).Text;
                contextBuilder.AppendFormat(", Name: {0}", name);
            }

            if (section.Entries.Count > 0) {
                int itemIdx = (msgId - section.Start) % section.Entries.Count;
                contextBuilder.AppendFormat(", {0}", section.Entries[itemIdx]);
            }

            return contextBuilder.ToString();
        }
    }
}
