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
namespace SceneGate.Games.ProfessorLayton.Tool.LondonLife
{
    using System;
    using System.IO;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Texts;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    /// <summary>
    /// Commands related to text files.
    /// </summary>
    public static class TextCommands
    {
        /// <summary>
        /// Export the message file into a directory of PO files.
        /// </summary>
        /// <param name="input">The input binary message file.</param>
        /// <param name="table">The optional path to the table file.</param>
        /// <param name="format">The format of the message file.</param>
        /// <param name="output">The output directory.</param>
        public static void Export(string input, string table, LondonLifeTextFormat format, string output)
        {
            var region = LondonLifeRegion.Usa;
            using Node node = NodeFactory.FromFile(input, FileOpenMode.Read);

            Node messageNode = null;
            switch (format) {
                case LondonLifeTextFormat.CommonDarc:
                    messageNode = node.TransformWith<BinaryDarc2Container>()
                        .Children[2]
                        .TransformWith<DencDecompression>();
                    break;

                case LondonLifeTextFormat.MessageBinary:
                    // do nothing as we convert below
                    messageNode = node;
                    break;

                default:
                    throw new NotSupportedException("Unsupported format");
            }

            messageNode.TransformWith<Binary2MessageCollection>()
                .TransformWith<MessageCollection2PoContainer, LondonLifeRegion>(region);

            Replacer replacer;
            if (string.IsNullOrEmpty(table)) {
                replacer = ReplacerFactory.GetReplacer(region);
            } else {
                using var tableNode = NodeFactory.FromFile(table, FileOpenMode.Read);
                replacer = new ReplacerDeserializer().Convert(tableNode.GetFormatAs<BinaryFormat>());
            }

            var replacerParams = new PoTableReplacerParams {
                Replacer = replacer,
                TransformForward = true,
            };
            foreach (var children in messageNode.Children) {
                children.TransformWith<PoTableReplacer, PoTableReplacerParams>(replacerParams)
                    .TransformWith<Po2Binary>()
                    .Stream.WriteTo(Path.Combine(output, $"{children.Name}.po"));
            }
        }

        /// <summary>
        /// Import a directory of PO files into a binary message file.
        /// </summary>
        /// <param name="input">The input directory of PO files.</param>
        /// <param name="table">The optional path to the table file.</param>
        /// <param name="originalDarc">The path to the original ll_common.darc container when the output format is DARC.</param>
        /// <param name="output">The new binary message file.</param>
        /// <param name="format">The format of the output file.</param>
        /// <returns>A return code of the operation.</returns>
        public static int Import(
            string input,
            string table,
            string originalDarc,
            string output,
            LondonLifeTextFormat format)
        {
            if (format == LondonLifeTextFormat.CommonDarc && !File.Exists(originalDarc)) {
                Console.WriteLine(
                    $"The format '{nameof(LondonLifeTextFormat.CommonDarc)}' " +
                    "requires the argument --original-darc");
                return 1;
            }

            var region = LondonLifeRegion.Usa;

            Replacer replacer;
            if (string.IsNullOrEmpty(table)) {
                replacer = ReplacerFactory.GetReplacer(region);
            } else {
                using var tableNode = NodeFactory.FromFile(table, FileOpenMode.Read);
                replacer = new ReplacerDeserializer().Convert(tableNode.GetFormatAs<BinaryFormat>());
            }

            var replacerParams = new PoTableReplacerParams {
                Replacer = replacer,
                TransformForward = false,
            };

            using var node = NodeFactory.FromDirectory(input, "*.po", FileOpenMode.Read);
            foreach (var child in node.Children) {
                child.TransformWith<Binary2Po>()
                    .TransformWith<PoTableReplacer, PoTableReplacerParams>(replacerParams);
            }

            node.TransformWith<PoContainer2MessageCollection>()
                .TransformWith<MessageCollection2Binary>();

            switch (format) {
                case LondonLifeTextFormat.CommonDarc:
                    // Compress message with LZSS in DENC
                    node.TransformWith<DencCompression, DencCompressionKind>(DencCompressionKind.Lzss);

                    // Open container
                    var darc = NodeFactory.FromFile(originalDarc, FileOpenMode.Read)
                        .TransformWith<BinaryDarc2Container>();

                    // Replace and write
                    darc.Children[2].ChangeFormat(node.Format);
                    darc.TransformWith<NodeContainer2BinaryDarc>()
                        .Stream.WriteTo(output);
                    darc.Dispose();
                    break;

                case LondonLifeTextFormat.MessageBinary:
                    // already in binary - just write
                    node.Stream.WriteTo(output);
                    break;
            }

            return 0;
        }
    }
}
