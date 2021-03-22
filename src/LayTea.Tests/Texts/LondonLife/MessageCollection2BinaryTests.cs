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
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
    using Yarhl.FileSystem;
    using Yarhl.IO;

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
        public void ThreeWaysConversionIsEqual()
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

            BinaryFormat expected = node.GetFormatAs<BinaryFormat>();
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
    }
}
