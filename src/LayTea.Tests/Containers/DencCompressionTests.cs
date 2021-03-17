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
    using System;
    using System.Collections;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using Yarhl.IO;

    [TestFixture]
    public class DencCompressionTests
    {
        [Test]
        public void NullDecompressionThrowsException()
        {
            var compression = new DencCompression();
            Assert.That(() => compression.Convert((BinaryFormat)null), Throws.ArgumentNullException);
        }

        [Test]
        public void InvalidCompressionTypeThrows()
        {
            var compression = new DencCompression();
            compression.Initialize((DencCompressionKind)0x80);

            DataStream input = new DataStream();
            input.WriteByte(0xCA);
            using var binary = new BinaryFormat(input);

            Assert.That(() => compression.Convert(binary), Throws.InstanceOf<NotImplementedException>());
        }

        [TestCaseSource(nameof(GetCompressedFiles))]
        public void CompressionWithFilesThreeWays(string inputPath, long offset, long length, string infoPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(inputPath);

            var inputStream = DataStreamFactory.FromFile(inputPath, FileOpenMode.Read, offset, length);
            using var input = new BinaryFormat(inputStream);
            inputStream.Position = 0x08;
            bool isLzss = new DataReader(inputStream).ReadString(4) == "LZSS";

            var decompression = new DencDecompression();
            using BinaryFormat decompressed = decompression.Convert(input);

            int initialStreams = DataStream.ActiveStreams;
            BinaryFormat compressed = null;
            BinaryFormat decompressedAfter = null;
            try {
                var compression = new DencCompression();
                compression.Initialize(isLzss ? DencCompressionKind.Lzss : DencCompressionKind.None);
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

        private static IEnumerable GetCompressedFiles()
        {
            return TestData.GetSubstreamAndInfoCollection("denc.txt");
        }
    }
}