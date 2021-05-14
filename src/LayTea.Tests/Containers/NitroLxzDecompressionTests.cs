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
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SceneGate.Games.ProfessorLayton.Containers;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace SceneGate.Games.ProfessorLayton.Tests.Containers
{
    [TestFixture]
    public class NitroLzxDecompressionTests
    {
        public static IEnumerable GetFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "containers");
            string listPath = Path.Combine(basePath, "lzx.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        [Test]
        public void Guards()
        {
            var converter = new NitroLzxDecompression();
            converter.Invoking(x => x.Convert((Stream)null)).Should().Throw<ArgumentNullException>();
            converter.Invoking(x => x.Convert((BinaryFormat)null)).Should().Throw<ArgumentNullException>();
        }

        [TestCaseSource(nameof(GetFiles))]
        public void DeserializeAndCheckImageHash(string infoPath, string binPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(binPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            var info = BinaryInfo.FromYaml(infoPath);
            NodeFactory.FromFile(binPath, FileOpenMode.Read)
                .TransformWith<NitroLzxDecompression>()
                .Stream.Should().MatchInfo(info);
        }

        [Test]
        public void InvalidStampThrows()
        {
            var stream = new DataStream();
            stream.Write(new byte[] { 0x10, 0x20, 0x00, 0x00 }, 0, 4);

            var converter = new NitroLzxDecompression();
            converter.Invoking(x => x.Convert(stream)).Should().Throw<FormatException>();
        }
    }
}
