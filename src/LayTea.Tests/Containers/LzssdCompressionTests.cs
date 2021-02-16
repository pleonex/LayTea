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
    public class LzssdCompressionTests
    {
        private const int MaxRaw = 127;
        private const int MaxSequence = 17;
        private const int MaxBackPos = 2047;

        private LzssdDecompression decompression;
        private LzssdCompression compression;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            decompression = new LzssdDecompression();
            compression = new LzssdCompression();
        }

        [Test]
        public void NullDecompressionThrowsException()
        {
            Assert.That(() => compression.Convert((BinaryFormat)null), Throws.ArgumentNullException);
            Assert.That(() => compression.Convert((DataStream)null), Throws.ArgumentNullException);
        }

        [TestCaseSource(nameof(GetCompressedFiles))]
        public void CompressionWithFilesThreeWays(string inputPath, long offset, long length, string infoPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(inputPath);

            var inputStream = DataStreamFactory.FromFile(inputPath, FileOpenMode.Read, offset, length);
            using var input = new BinaryFormat(inputStream);
            using BinaryFormat decompressed = decompression.Convert(input);

            int initialStreams = DataStream.ActiveStreams;
            BinaryFormat compressed = null;
            BinaryFormat decompressedAfter = null;
            try {
                compressed = compression.Convert(decompressed);

                decompressedAfter = decompression.Convert(compressed);

                Assert.That(decompressed.Stream.Compare(decompressedAfter?.Stream), Is.True);
                Assert.That(compressed.Stream.Length, Is.LessThanOrEqualTo(length));
            } finally {
                compressed?.Dispose();
                decompressedAfter?.Dispose();
            }

            Assert.That(DataStream.ActiveStreams, Is.EqualTo(initialStreams), "Missing stream disposes");
        }

        [Test]
        public void FirstBytesEncodedAsRaw()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0xCA, 0xFE }, 0, 2);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(3));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x04));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFE));
        }

        [Test]
        public void WriteMaxRawBytes()
        {
            var stream = new DataStream();
            for (int i = 0; i < MaxRaw; i++) {
                stream.WriteByte((byte)i);
            }

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(MaxRaw + 1));
            Assert.That(compressed.ReadByte(), Is.EqualTo(MaxRaw << 1));
            for (int i = 0; i < MaxRaw; i++) {
                Assert.That(compressed.ReadByte(), Is.EqualTo(i));
            }
        }

        [Test]
        public void WriteRawAfterMaxRawBytes()
        {
            var stream = new DataStream();
            for (int i = 0; i < MaxRaw + 1; i++) {
                stream.WriteByte((byte)i);
            }

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(MaxRaw + 3));
            Assert.That(compressed.ReadByte(), Is.EqualTo(MaxRaw << 1));
            for (int i = 0; i < MaxRaw; i++) {
                Assert.That(compressed.ReadByte(), Is.EqualTo(i));
            }

            Assert.That(compressed.ReadByte(), Is.EqualTo(1 << 1));
            Assert.That(compressed.ReadByte(), Is.EqualTo(MaxRaw));
        }

        [Test]
        public void IgnoreSequencesSmallerThan2()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0xFE, 0xFE, 0xCA, 0xFE }, 0, 4);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(5));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x08));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFE));
        }

        [Test]
        public void FindSequenceFromLength2()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0xCA, 0xFE, 0xFF, 0xCA, 0xFE }, 0, 5);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(6));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x06));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFF));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x07));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x00));
        }

        [Test]
        public void FindSelfRepeatingSequence()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0xCA, 0xFE, 0xCA, 0xFE, 0xCA, 0xFE }, 0, 6);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(5));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x04));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x05));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x20));
        }

        [Test]
        public void FindSequenceInPresent()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0xCA, 0xFE, 0xCA, 0xFE }, 0, 4);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(5));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x04));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xCA));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x05));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x00));
        }

        [Test]
        public void FindPatternInAlreadyDecompressedPattern()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x01, 0x01, 0x01, 0x02, 0x01, 0x02 }, 0, 6);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(8));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x00));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x05));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x00));
        }

        [Test]
        public void FindLargestPattern()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x01 }, 0, 7);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(8));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x00));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x09));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x10));
        }

        [Test]
        public void FindPatternAfterMaxPattern()
        {
            var stream = new DataStream();
            for (int i = 0; i < (1 + MaxSequence + 2); i++) {
                stream.WriteByte(0x01);
            }

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(6));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xF0));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x00));
        }

        [Test]
        public void FindSingleExtraPatternAfterMaxPattern()
        {
            var stream = new DataStream();
            for (int i = 0; i < (1 + MaxSequence + 1); i++) {
                stream.WriteByte(0x01);
            }

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.Length, Is.EqualTo(6));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0xF0));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));
        }

        [Test]
        public void IgnorePatternBeforeMaxPos()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x01, 0x01 }, 0, 2);
            for (int i = 0; i < (MaxBackPos - 1); i++) {
                stream.WriteByte(0x02);
            }

            stream.Write(new byte[] { 0x01, 0x01 }, 0, 2);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x06));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));

            for (int i = 0; i < 120; i++) {
                Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
                Assert.That(compressed.ReadByte(), Is.EqualTo(0xF0));
            }

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x30));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x04));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));
        }

        [Test]
        public void FindPatternInMaxPos()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x01, 0x01 }, 0, 2);
            for (int i = 0; i < (MaxBackPos - 2); i++) {
                stream.WriteByte(0x02);
            }

            stream.Write(new byte[] { 0x01, 0x01 }, 0, 2);

            var compressed = compression.Convert(stream);

            compressed.Position = 0;
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x06));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x01));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x02));

            for (int i = 0; i < 120; i++) {
                Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
                Assert.That(compressed.ReadByte(), Is.EqualTo(0xF0));
            }

            Assert.That(compressed.ReadByte(), Is.EqualTo(0x03));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x20));

            Assert.That(compressed.ReadByte(), Is.EqualTo(0xFF));
            Assert.That(compressed.ReadByte(), Is.EqualTo(0x0F));
        }

        private static IEnumerable GetCompressedFiles()
        {
            return TestData.GetSubstreamAndInfoCollection("lzssd.txt");
        }
    }
}
