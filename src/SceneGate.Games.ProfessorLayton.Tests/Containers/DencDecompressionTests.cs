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
using System;
using System.Collections;
using FluentAssertions;
using NUnit.Framework;
using SceneGate.Games.ProfessorLayton.Containers;
using Yarhl.IO;

namespace SceneGate.Games.ProfessorLayton.Tests.Containers
{
    [TestFixture]
    public class DencDecompressionTests
    {
        private DencDecompression decompression;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            decompression = new DencDecompression();
        }

        [TestCaseSource(nameof(GetCompressedFiles))]
        public void DecompressionWithFiles(string inputPath, long offset, long length, string infoPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(inputPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            using var input = new BinaryFormat(DataStreamFactory.FromFile(inputPath, FileOpenMode.Read, offset, length));
            var expected = BinaryInfo.FromYaml(infoPath);

            int initialStreams = DataStream.ActiveStreams;
            BinaryFormat decompressed = null;
            try {
                decompression.Invoking(converter => decompressed = converter.Convert(input))
                    .Should().NotThrow();
                decompressed.Should().MatchInfo(expected);
            } finally {
                decompressed?.Dispose();
            }

            DataStream.ActiveStreams.Should().Be(initialStreams);
        }

        [Test]
        public void NullDecompressionThrowsException()
        {
            Assert.That(() => decompression.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void InvalidStampThrows()
        {
            using var binary = new BinaryFormat();
            binary.Stream.Write(new byte[] { 0x41, 0x42, 0x43, 0x30 }, 0, 4);

            Assert.That(() => decompression.Convert(binary), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void InvalidCompressionThrows()
        {
            byte[] data = new byte[] {
                0x44, 0x45, 0x4E, 0x43, // DENC
                0x01, 0x00, 0x00, 0x00, // decompressed
                0x41, 0x42, 0x43, 0x44, // algorithm (ABCD)
                0x01, 0x00, 0x00, 0x00, // compressed
                0xAA,                   // data
            };

            using var binary = new BinaryFormat();
            binary.Stream.Write(data, 0, data.Length);

            Assert.That(() => decompression.Convert(binary), Throws.InstanceOf<NotImplementedException>());
        }

        private static IEnumerable GetCompressedFiles()
        {
            return TestData.GetSubstreamAndInfoCollection("denc.txt");
        }
    }
}
