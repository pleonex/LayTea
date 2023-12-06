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
    using System.IO;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter for the London Life message collection model into binary MSG.
    /// </summary>
    public sealed class MessageCollection2Binary : IConverter<MessageCollection, BinaryFormat>
    {
        private const string Stamp = "MSG ";
        private const byte FirstFunction = 0xF0;
        private const ushort NewLineId = 0x00F0;
        private const ushort QuestionId = 0x00F5;
        private const byte BlockPadding = 0xFF;
        private const byte BlockEnd = 0x00;

        /// <summary>
        /// Convert a collection of messages into a binary MSG format.
        /// </summary>
        /// <param name="source">The collection of messages.</param>
        /// <returns>The binary MSG.</returns>
        public BinaryFormat Convert(MessageCollection source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            writer.Write(Stamp, nullTerminator: false);
            writer.Write(source.Messages.Count);
            writer.WriteTimes(0x00, 4 * source.Messages.Count); // 32-bits pointers placeholder

            for (int i = 0; i < source.Messages.Count; i++) {
                int pointerOffset = 8 + (4 * i);
                uint pointerValue = (uint)(binary.Stream.Position - pointerOffset);
                binary.Stream.RunInPosition(() => writer.Write(pointerValue), pointerOffset);

                var msg = source.Messages[i];
                for (int j = 0; j < msg.Content.Count; j++) {
                    var element = msg.Content[j];
                    if (element is MessageRawText text) {
                        WriteText(writer, text);
                    } else if (element is MessageFunction function) {
                        WriteFunction(writer, function);
                    }
                }

                if (msg.QuestionOptions != null) {
                    WriteQuestionOptions(writer, msg.QuestionOptions);
                }

                // Each script ends with 0
                writer.Write(BlockEnd);
                writer.WritePadding(BlockEnd, 4);
            }

            return binary;
        }

        private void WriteText(DataWriter writer, MessageRawText text)
        {
            foreach (char ch in text.Text) {
                if (ch == '\n') {
                    writer.WritePadding(BlockPadding, 4);
                    writer.Write(NewLineId);
                    writer.Write((ushort)0x02); // func length is only this field
                } else {
                    // Make sure that if we write a char in the function range
                    // and it's the beginning of a block, we have the escape token.
                    // Theoretically this is possible and the original MSG.BIN use it
                    // but in practice, in most script parts the game crashes anyway.
                    if (ch >= FirstFunction && ((writer.Stream.Position % 4) == 0)) {
                        writer.Write(BlockPadding);
                    }

                    // Text is Latin-1, we need to convert to C# char (UTF-16).
                    // Latin-1 match the range of UTF-16 so casting is fast.
                    writer.Write((byte)ch);
                }
            }
        }

        private void WriteFunction(DataWriter writer, MessageFunction function)
        {
            writer.WritePadding(BlockPadding, 4);
            writer.Write((ushort)function.Id);

            if (function.Argument.HasValue) {
                writer.Write((ushort)4);
                writer.Write(function.Argument.Value);
            } else {
                writer.Write((ushort)2);
            }
        }

        private void WriteQuestionOptions(DataWriter writer, QuestionOptions options)
        {
            writer.WritePadding(BlockPadding, 4);
            writer.Write(QuestionId);

            long funcOffset = writer.Stream.Position;
            writer.Write((ushort)0x00); // placeholder for func length

            writer.Write((ushort)options.PreSelectedIndex);
            writer.Write((ushort)options.DefaultIndex);
            writer.Write(options.Options.Count);

            long pointersStartOffset = writer.Stream.Position;
            writer.WriteTimes(0x00, 2 * options.Options.Count); // pointers placeholder
            writer.WritePadding(0x00, 4);

            for (int i = 0; i < options.Options.Count; i++) {
                long optionPos = writer.Stream.Position;
                long pointerOffset = pointersStartOffset + (2 * i);
                ushort pointerValue = (ushort)(optionPos - pointerOffset);
                writer.Stream.Position = pointerOffset;
                writer.Write(pointerValue);
                writer.Stream.Position = optionPos;

                writer.Write(options.Options[i].Item1);
                writer.Write(options.Options[i].Item2.Length);
                writer.Write(options.Options[i].Item2, nullTerminator: true, Encoding.Latin1);

                // It seems they had a weird padding issue. They write the null terminator
                // then additionally they write it again (probably different funcs).
                // And then, except for the last section they pad it.
                // They probably don't pad the last one as it will be with the padding
                // of the full script (but not doing here to be included in the func length).
                writer.Write((byte)0x00);
                if (i + 1 != options.Options.Count) {
                    writer.WritePadding(0, 4);
                }
            }

            ushort length = (ushort)(writer.Stream.Position - funcOffset);
            writer.Stream.Position = funcOffset;
            writer.Write(length);
            writer.Stream.Seek(0, SeekOrigin.End);
        }
    }
}
