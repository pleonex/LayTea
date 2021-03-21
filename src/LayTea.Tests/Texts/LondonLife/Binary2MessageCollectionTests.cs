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
namespace SceneGate.Games.ProfessorLayton.Tests.Texts.LondonLife
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [TestFixture]
    public class Binary2MessageCollectionTests
    {
        [Test]
        public void NullBinaryThrows()
        {
            var converter = new Binary2MessageCollection();
            Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void InvalidStampThrows()
        {
            var converter = new Binary2MessageCollection();
            using var binary = new BinaryFormat();
            binary.Stream.Write(new byte[] { 0x30, 0x31, 0x32, 0x33 }, 0, 4);

            Assert.That(() => converter.Convert(binary), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void ConvertFileSucceeds()
        {
            const int offset = 292;
            const int length = 384261;
            string commonPath = Path.Combine(TestDataBase.RootFromOutputPath, "containers", "ll_common.darc");
            TestDataBase.IgnoreIfFileDoesNotExist(commonPath);

            using var commonStream = DataStreamFactory.FromFile(commonPath, FileOpenMode.Read);
            using Node node = NodeFactory.FromSubstream("msg", commonStream, offset, length);
            try {
                node.TransformWith<LzssdDecompression>();
            } catch {
                Assert.Ignore("Failed to decompress");
            }

            Assert.That(() => node.TransformWith<Binary2MessageCollection>(), Throws.Nothing);

            var msg = node.GetFormatAs<MessageCollection>();
            Assert.That(msg.Messages, Has.Count.EqualTo(11_103));

            // Tricky message with code-points matching function IDs.
            // Also it contains a script end inside a text block.
            Assert.That(
                ((MessageRawText)msg.Messages[8606].Content[0]).Text,
                Is.EqualTo(".*F[cku?Üüû][©c??k][k???].*"));

            // First question (just to validate one)
            Assert.That(msg.Messages[2329].QuestionOptions.DefaultIndex, Is.EqualTo(1));
            Assert.That(msg.Messages[2329].QuestionOptions.PreSelectedIndex, Is.EqualTo(0));
            Assert.That(msg.Messages[2329].QuestionOptions.Options[0], Is.EqualTo("Yes"));
            Assert.That(msg.Messages[2329].QuestionOptions.Options[1], Is.EqualTo("No"));
        }
    }
}
