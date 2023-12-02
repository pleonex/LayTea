﻿// Copyright (c) 2021 SceneGate

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
    using System.IO;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [TestFixtureSource(typeof(TestData), nameof(TestData.DarcParams))]
    public class BinaryDarc2ContainerTests : Binary2ContainerTests
    {
        private readonly string yamlPath;
        private readonly string binaryPath;

        public BinaryDarc2ContainerTests(string yamlPath, string binaryPath)
        {
            this.yamlPath = yamlPath;
            this.binaryPath = binaryPath;

            TestDataBase.IgnoreIfFileDoesNotExist(yamlPath);
            TestDataBase.IgnoreIfFileDoesNotExist(binaryPath);
        }

        [Test]
        public void InvalidStampThrowsException()
        {
            using var binary = new BinaryFormat();
            binary.Stream.Write(new byte[] { 0x41, 0x42, 0x43, 0x44 }, 0, 4);

            var converter = GetToContainerConverter();
            Assert.That(() => converter.Convert(binary), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void NotBinaryChildThrowsException()
        {
            using var container = new NodeContainerFormat();
            container.Root.Add(new Node("child", new NodeContainerFormat()));

            var converter = GetToBinaryConverter();
            Assert.That(() => converter.Convert(container), Throws.InstanceOf<FormatException>());
        }

        protected override BinaryFormat GetBinary()
        {
            var stream = DataStreamFactory.FromFile(binaryPath, FileOpenMode.Read);
            return new BinaryFormat(stream);
        }

        protected override NodeContainerInfo GetContainerInfo()
        {
            return NodeContainerInfo.FromYaml(yamlPath);
        }

        protected override IConverter<NodeContainerFormat, BinaryFormat> GetToBinaryConverter() =>
            new NodeContainer2BinaryDarc();

        protected override IConverter<BinaryFormat, NodeContainerFormat> GetToContainerConverter() =>
            new BinaryDarc2Container();
    }
}
