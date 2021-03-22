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
    using Yarhl.IO;

    /// <summary>
    /// Converter for the London Life binary MSG file into a model.
    /// </summary>
    public sealed class Binary2MessageCollection : IConverter<BinaryFormat, MessageCollection>
    {
        private const string Stamp = "MSG ";
        private const byte FirstFunction = 0xF0;
        private const byte NewLineId = 0xF0;
        private const byte QuestionId = 0xF5;
        private const byte BlockPadding = 0xFF;
        private const byte BlockEnd = 0x00;

        /// <summary>
        /// Convert a binary MSG stream into a collection of messages.
        /// </summary>
        /// <param name="source">The binary MSG format.</param>
        /// <returns>The new message collection.</returns>
        public MessageCollection Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var book = new MessageCollection();
            var reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            string stamp = reader.ReadString(4);
            if (stamp != Stamp) {
                throw new FormatException($"Invalid stamp: {stamp}");
            }

            int numEntries = reader.ReadInt32();
            for (int i = 0; i < numEntries; i++) {
                long relativeOffset = reader.Stream.Position;
                uint pointer = reader.ReadUInt32();
                source.Stream.RunInPosition(
                    () => book.Messages.Add(ReadEntry(reader)),
                    relativeOffset + pointer);
            }

            return book;
        }

        private Message ReadEntry(DataReader reader)
        {
            var entry = new Message();

            var currentText = new StringBuilder();
            byte[] block = new byte[4];
            bool foundEnd = false;
            while (!foundEnd) {
                reader.Stream.Read(block, 0, block.Length);

                if (block[0] == BlockEnd) {
                    foundEnd = true;
                } else if (block[0] < FirstFunction || block[0] == BlockPadding) {
                    foundEnd = AppendText(block, currentText);
                } else if (block[0] == NewLineId) {
                    currentText.AppendLine();
                } else {
                    // Flush current raw text
                    if (currentText.Length > 0) {
                        entry.Add(currentText.ToString());
                        currentText.Clear();
                    }

                    foundEnd = ReadFunction(reader, block, entry, currentText);
                }
            }

            if (currentText.Length > 0) {
                entry.Add(currentText.ToString());
            }

            return entry;
        }

        private bool AppendText(byte[] block, StringBuilder currentText)
        {
            for (int i = 0; i < block.Length; i++) {
                // The script end may happen inside a text block.
                if (block[i] == BlockEnd) {
                    return true;
                }

                // If the code-point is in the range [0xF0, 0xFE], the first byte
                // will be 0xFF to escape it. If there isn't enough text in the block
                // it pads the block with 0xFF. In all cases just skip it.
                if (block[i] == BlockPadding) {
                    continue;
                }

                // Text is Latin-1, we need to convert to C# char (UTF-16).
                // Latin-1 match the range of UTF-16 so casting is fast.
                currentText.Append((char)block[i]);
            }

            return false;
        }

        private bool ReadFunction(DataReader reader, byte[] block, Message entry, StringBuilder currentText)
        {
            int id = block[0] | (block[1] << 8);
            int funcLength = block[2] | (block[3] << 8);

            bool foundEnd = false;
            if (id == QuestionId) {
                entry.QuestionOptions = ReadQuestionOptions(reader);
                return true;
            } else {
                // Only short values are present in the known functions.
                short? arg = null;
                if (funcLength == 4) {
                    arg = reader.ReadInt16();

                    foundEnd = AppendText(reader.ReadBytes(2), currentText);
                }

                var function = MessageFunction.FromId(id, arg);
                entry.Add(function);
            }

            return foundEnd;
        }

        private QuestionOptions ReadQuestionOptions(DataReader reader)
        {
            var question = new QuestionOptions();
            question.PreSelectedIndex = reader.ReadInt16();
            question.DefaultIndex = reader.ReadInt16();

            int numOptions = reader.ReadInt32();
            for (int i = 0; i < numOptions; i++) {
                long pointerPos = reader.Stream.Position;
                int pointer = reader.ReadUInt16();

                reader.Stream.Position = pointerPos + pointer;
                int id = reader.ReadInt32();
                int length = reader.ReadInt32();
                string option = reader.ReadString(length, Encoding.Latin1);
                question.Options.Add((id, option));

                reader.Stream.Position = pointerPos + 2;
            }

            reader.SkipPadding(4); // skip padding if we are inside a block.
            return question;
        }
    }
}
