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
    using System.Linq;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    /// <summary>
    /// Converter for a container with PO formats into a single message collection object.
    /// </summary>
    public class PoContainer2MessageCollection : IConverter<NodeContainerFormat, MessageCollection>
    {
        private readonly MessageTextSerializer textParser = new MessageTextSerializer();

        /// <summary>
        /// Convert a container with PO formats into a message collection.
        /// </summary>
        /// <param name="source">The container with POs.</param>
        /// <returns>The new message collection.</returns>
        public MessageCollection Convert(NodeContainerFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Root.Children.Any(c => c.Format is not Po))
                throw new FormatException("Container contains non-PO formats");

            var entries = source.Root
                .Children
                .Select(n => n.GetFormatAs<Po>())
                .SelectMany(p => p.Entries)
                .Select(e => new {
                    Context = e.Context.Split(':'),
                    Text = e.Text,
                })
                .Select(e => new {
                    Id = int.Parse(e.Context[0]),
                    Box = int.Parse(e.Context[1]),
                    EndBox = bool.Parse(e.Context[2]),
                    Text = e.Text,
                })
                .GroupBy(e => e.Id)
                .Select(e => new {
                    Id = e.Key,
                    EndBox = e.OrderBy(x => x.Box).Last().EndBox,
                    Text = e.OrderBy(x => x.Box).Select(x => x.Text),
                })
                .Select(e => new {
                    Id = e.Id,
                    EndBox = e.EndBox,
                    Message = textParser.Deserialize(e.Text),
                })
                .OrderBy(e => e.Id);

            var collection = new MessageCollection();
            int lastId = -1;
            foreach (var entry in entries) {
                if (lastId + 1 < entry.Id) {
                    for (int i = lastId + 1; i < entry.Id; i++) {
                        collection.Messages.Add(new Message());
                    }
                }

                if (entry.EndBox) {
                    entry.Message.Add(MessageFunction.FromMnemonic("next_box", null));
                }

                collection.Messages.Add(entry.Message);
                lastId = entry.Id;
            }

            return collection;
        }
    }
}
