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
namespace SceneGate.Games.ProfessorLayton.Tests.Containers;

using FluentAssertions;
using NUnit.Framework;
using Yarhl.Experimental.TestFramework;
using Yarhl.Experimental.TestFramework.FluentAssertions;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

public abstract class Binary2ContainerTests
{
    private int initialStreams;
    private BinaryFormat original;
    private NodeContainerInfo containerInfo;
    private IConverter<BinaryFormat, NodeContainerFormat> containerConverter;
    private IConverter<NodeContainerFormat, BinaryFormat> binaryConverter;

    [OneTimeSetUp]
    public void SetUpFixture()
    {
        containerInfo = GetContainerInfo();
        containerConverter = GetToContainerConverter();
        binaryConverter = GetToBinaryConverter();
    }

    [TearDown]
    public void TearDown()
    {
        original?.Dispose();

        // Make sure we didn't leave anything without dispose.
        Assert.That(DataStream.ActiveStreams, Is.EqualTo(initialStreams), "Missing stream disposes");
    }

    [SetUp]
    public void SetUp()
    {
        // By opening and disposing in each we prevent other tests failing
        // because the file is still open.
        initialStreams = DataStream.ActiveStreams;
        original = GetBinary();
    }

    [Test]
    public void NullToContainerThrowsException()
    {
        Assert.That(() => containerConverter.Convert(null), Throws.ArgumentNullException);
    }

    [Test]
    public void NullToBinaryThrowsException()
    {
        if (binaryConverter == null) {
            Assert.Ignore();
        }

        Assert.That(() => binaryConverter.Convert(null), Throws.ArgumentNullException);
    }

    [Test]
    public void TransformToContainer()
    {
        // Check nodes are expected
        using var nodes = containerConverter.Convert(original);
        nodes.Root.Should().MatchInfo(containerInfo);

        // Check everything is virtual node (only the binary stream)
        DataStream.ActiveStreams.Should().Be(initialStreams + 1);
    }

    [Test]
    public void TransformBothWays()
    {
        if (binaryConverter == null) {
            Assert.Ignore();
        }

        using var nodes = containerConverter.Convert(original);
        using var actualBinary = binaryConverter.Convert(nodes);

        Assert.That(original.Stream.Compare(actualBinary.Stream), Is.True, "Streams are not identical");
    }

    protected abstract BinaryFormat GetBinary();

    protected abstract NodeContainerInfo GetContainerInfo();

    protected abstract IConverter<BinaryFormat, NodeContainerFormat> GetToContainerConverter();

    protected abstract IConverter<NodeContainerFormat, BinaryFormat> GetToBinaryConverter();
}
