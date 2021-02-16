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
namespace SceneGate.Games.ProfessorLayton.Tests.Containers
{
    using System.Collections;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using Yarhl.IO;

    [TestFixture]
    public class LzssdDecompressionTests
    {
        private LzssdDecompression decompression;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            decompression = new LzssdDecompression();
        }

        [Test]
        public void NullDecompressionThrowsException()
        {
            Assert.That(() => decompression.Convert((BinaryFormat)null), Throws.ArgumentNullException);
            Assert.That(() => decompression.Convert((DataStream)null), Throws.ArgumentNullException);
        }

        [TestCaseSource(nameof(GetCompressedFiles))]
        [Timeout(1000)]
        public void DecompressionWithFiles(string inputPath, long offset, long length, string infoPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(inputPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            using var input = new BinaryFormat(DataStreamFactory.FromFile(inputPath, FileOpenMode.Read, offset, length));
            var expected = BinaryInfo.FromYaml(infoPath);

            int initialStreams = DataStream.ActiveStreams;
            BinaryFormat decompressed = null;
            try {
                Assert.That(() => decompressed = decompression.Convert(input), Throws.Nothing);
                expected.AssertIsEqual(decompressed);
            } finally {
                decompressed?.Dispose();
            }

            Assert.That(DataStream.ActiveStreams, Is.EqualTo(initialStreams), "Missing stream disposes");
        }

        [Test]
        public void CopyRawToken()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x06, 0xCA, 0xFE, 0xBE }, 0, 4);

            using var decompressed = decompression.Convert(stream);
            decompressed.Position = 0;

            Assert.That(decompressed.Length, Is.EqualTo(3));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
        }

        [Test]
        public void CopyRawTokenZeroLength()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x00 }, 0, 1);

            using var decompressed = decompression.Convert(stream);

            Assert.That(decompressed.Length, Is.EqualTo(0));
        }

        [Test]
        public void CopyBufferTokenFullPast()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x06, 0xCA, 0xFE, 0xBE, 0x07, 0x10 }, 0, 6);

            using var decompressed = decompression.Convert(stream);
            decompressed.Position = 0;

            Assert.That(decompressed.Length, Is.EqualTo(6));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
        }

        [Test]
        public void CopyBufferTokenWithFuture()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x06, 0xCA, 0xFE, 0xBE, 0x05, 0x20 }, 0, 6);

            using var decompressed = decompression.Convert(stream);
            decompressed.Position = 0;

            Assert.That(decompressed.Length, Is.EqualTo(7));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
        }

        [Test]
        public void CopyBufferTokenFullFuture()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x06, 0xCA, 0xFE, 0xBE, 0x03, 0x20 }, 0, 6);

            using var decompressed = decompression.Convert(stream);
            decompressed.Position = 0;

            Assert.That(decompressed.Length, Is.EqualTo(7));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
            Assert.That(decompressed.ReadByte(), Is.EqualTo(0xBE));
        }

        [Test]
        public void CopyBufferTokenPosZeroThrows()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x06, 0xCA, 0xFE, 0xBE, 0x01, 0x20 }, 0, 6);

            Assert.That(() => decompression.Convert(stream), Throws.InvalidOperationException);
        }

        private static IEnumerable GetCompressedFiles()
        {
            return TestData.GetSubstreamAndInfoCollection("lzssd.txt");
        }
    }
}
