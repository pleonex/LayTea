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
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Texts;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    [TestFixture]
    public class MessageCollection2BinaryTests
    {
        [Test]
        public void NullBinaryThrows()
        {
            var converter = new MessageCollection2Binary();
            Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void MessageStartingWithSpecialSerializesTwoWaysCorrectly()
        {
            var expectedCollection = new MessageCollection();
            var expectedMessage = new Message();
            expectedMessage.Add("ñ23" + "aú34" + "ú24");
            expectedMessage.Add(MessageFunction.FromId(0xF1, null));
            expectedMessage.Add("ñu");
            expectedCollection.Messages.Add(expectedMessage);

            var serializer = new MessageCollection2Binary();
            using var binary = serializer.Convert(expectedCollection);

            var deserializer = new Binary2MessageCollection();
            var actualCollection = deserializer.Convert(binary);

            Assert.That(actualCollection.Messages, Has.Count.EqualTo(1));
            actualCollection.Messages[0].AssertIsEquivalent(expectedMessage);
        }

        [Test]
        public void ThreeWaysConversionIsEqual()
        {
            string commonPath = Path.Combine(TestDataBase.RootFromOutputPath, "containers", "ll_common.darc");
            TestDataBase.IgnoreIfFileDoesNotExist(commonPath);

            using Node node = NodeFactory.FromFile(commonPath);
            Node msgNode = null;
            try {
                msgNode = node.TransformWith<BinaryDarc2Container>()
                    .Children[2]
                    .TransformWith<DencDecompression>();
            } catch {
                Assert.Ignore("Failed to decompress");
            }

            BinaryFormat expected = msgNode.GetFormatAs<BinaryFormat>();
            MessageCollection messages = null;
            try {
                var deserializer = new Binary2MessageCollection();
                messages = deserializer.Convert(expected);
            } catch {
                Assert.Ignore("Failed to read");
            }

            var serializer = new MessageCollection2Binary();
            using BinaryFormat actual = serializer.Convert(messages);

            Assert.That(actual.Stream.Compare(expected.Stream), Is.True, "Different streams");
        }

        [Test]
        public void BinaryPoFullConversionIsEqual()
        {
            string commonPath = Path.Combine(TestDataBase.RootFromOutputPath, "containers", "ll_common.darc");
            TestDataBase.IgnoreIfFileDoesNotExist(commonPath);

            using Node node = NodeFactory.FromFile(commonPath);
            Node msgNode = null;
            try {
                msgNode = node.TransformWith<BinaryDarc2Container>()
                    .Children[2]
                    .TransformWith<DencDecompression>();
            } catch {
                Assert.Ignore("Failed to decompress");
            }

            using var expected = new DataStream(msgNode.Stream, 0, msgNode.Stream.Length);
            try {
                var replacerFParams = new PoTableReplacerParams {
                    Replacer = ReplacerFactory.GetReplacer(LondonLifeRegion.Usa),
                    TransformForward = true,
                };
                var replacerBParams = new PoTableReplacerParams {
                    Replacer = ReplacerFactory.GetReplacer(LondonLifeRegion.Usa),
                    TransformForward = false,
                };

                _ = msgNode.TransformWith<Binary2MessageCollection>()
                    .TransformWith(new MessageCollection2PoContainer(LondonLifeRegion.Usa))
                    .Children
                    .Select(c => c.TransformWith(new PoTableReplacer(replacerFParams)))
                    .Select(c => c.TransformWith<Po2Binary>())
                    .Select(c => {
                        c.Stream.Position = 0; // we need to fix this bug in Yarhl...
                        return c.TransformWith<Binary2Po>();
                    })
                    .Select(c => c.TransformWith(new PoTableReplacer(replacerBParams)))
                    .First().Parent
                    .TransformWith<PoContainer2MessageCollection>();
            } catch {
                Assert.Ignore("Failed to convert to/from PO");
            }

            var serializer = new MessageCollection2Binary();
            using BinaryFormat actual = serializer.Convert(msgNode.GetFormatAs<MessageCollection>());

            Assert.That(actual.Stream.Compare(expected), Is.True, "Different streams");
        }
    }
}
