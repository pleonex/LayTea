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
namespace SceneGate.Games.ProfessorLayton.Tests.Graphics;

using System;
using System.Collections;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SceneGate.Games.ProfessorLayton.Graphics;
using SceneGate.Games.ProfessorLayton.Tests;
using SixLabors.ImageSharp.Formats.Bmp;
using Texim.Formats;
using Yarhl.FileSystem;
using Yarhl.IO;

[TestFixture]
public class BinaryNccl2PaletteCollectionTests
{
    public static IEnumerable GetFiles()
    {
        string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "graphics");
        string listPath = Path.Combine(basePath, "nccl.txt");
        return TestDataBase.ReadTestListFile(listPath)
            .Select(line => line.Split(','))
            .Select(data => new TestCaseData(
                Path.Combine(basePath, data[0]),
                Path.Combine(basePath, data[1]))
                .SetArgDisplayNames(data[0], data[1]));
    }

    [Test]
    public void Guards()
    {
        var converter = new BinaryNccl2PaletteCollection();
        Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
    }

    [TestCaseSource(nameof(GetFiles))]
    public void DeserializeAndCheckImageHash(string infoPath, string ncclPath)
    {
        TestDataBase.IgnoreIfFileDoesNotExist(ncclPath);
        TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

        var info = NodeContainerInfo.FromYaml(infoPath);
        using var paletteNode = NodeFactory.FromFile(ncclPath, FileOpenMode.Read)
            .TransformWith<BinaryNccl2PaletteCollection>()
            .TransformWith(new PaletteCollection2ContainerBitmap(new BmpEncoder()));

        _ = paletteNode.Should().MatchInfo(info);
    }

    [Test]
    public void FormatWithCmntSectionThrowsException()
    {
        byte[] format = new byte[] {
            (byte)'N', (byte)'C', (byte)'C', (byte)'L', 0xFF, 0xFE, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x00,
            (byte)'C', (byte)'M', (byte)'N', (byte)'T', 0x0C, 0x00, 0x00, 0x00,
            0x30, 0x31, 0x00, 0x00,
        };
        using var input = new BinaryFormat(DataStreamFactory.FromArray(format));

        var converter = new BinaryNccl2PaletteCollection();

        converter.Invoking(x => x.Convert(input))
            .Should().Throw<FormatException>()
            .WithMessage("Unknown CMNT section");
    }

    [Test]
    public void FormatWithUnknownSectionThrowsException()
    {
        byte[] format = new byte[] {
            (byte)'N', (byte)'C', (byte)'C', (byte)'L', 0xFF, 0xFE, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x00,
            (byte)'P', (byte)'L', (byte)'E', (byte)'O', 0x0C, 0x00, 0x00, 0x00,
            0xBE, 0xBA, 0xCA, 0xFE,
        };
        using var input = new BinaryFormat(DataStreamFactory.FromArray(format));

        var converter = new BinaryNccl2PaletteCollection();

        converter.Invoking(x => x.Convert(input))
            .Should().Throw<FormatException>()
            .WithMessage("Unknown section: PLEO");
    }
}
