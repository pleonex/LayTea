// Copyright (c) 2023 SceneGate

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
using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SceneGate.Games.ProfessorLayton.Graphics;
using Yarhl.FileFormat;
using Yarhl.IO;

[TestFixture]
public class NDeserializerTests
{
    private static readonly byte[] ValidHeader = {
        (byte)'P', (byte)'L', (byte)'E', (byte)'O',
        0xFF, 0xFE,
        0x00, 0x01,
        0x00, 0x00, 0x00, 0x00,
        0x00, 0x00,
        0x00, 0x00,
    };

    [Test]
    public void ValidHeaderDoesNotThrow()
    {
        using var input = new BinaryFormat(DataStreamFactory.FromArray(ValidHeader));

        var deserializer = new Mock<NDeserializer<object>>();
        deserializer.Setup(x => x.Stamp).Returns("PLEO");
        deserializer.Setup(x => x.SupportedVersion).Returns(0x01_00);

        deserializer.Object.Invoking(x => x.Convert(input))
            .Should().NotThrow();
    }

    [Test]
    public void InvalidStampThrowsException()
    {
        byte[] format = ValidHeader.ToArray();
        format[0] = (byte)'N';
        format[1] = (byte)'E';
        format[2] = (byte)'X';
        format[3] = (byte)'U';
        using var input = new BinaryFormat(DataStreamFactory.FromArray(format));

        var deserializer = new Mock<NDeserializer<object>>();
        deserializer.Setup(x => x.Stamp).Returns("PLEO");

        deserializer.Object.Invoking(x => x.Convert(input))
            .Should().Throw<FormatException>()
            .WithMessage("Invalid stamp NEXU");
    }

    [Test]
    public void InvalidEndiannessThrowsException()
    {
        byte[] format = ValidHeader.ToArray();
        format[4] = 0xFE;
        format[5] = 0xCA;
        using var input = new BinaryFormat(DataStreamFactory.FromArray(format));

        var deserializer = new Mock<NDeserializer<object>>();
        deserializer.Setup(x => x.Stamp).Returns("PLEO");

        deserializer.Object.Invoking(x => x.Convert(input))
            .Should().Throw<FormatException>()
            .WithMessage("Unknown endianness: CAFE");
    }

    [Test]
    public void FormatBigEndianValidVersion()
    {
        byte[] format = ValidHeader.ToArray();
        format[4] = 0xFE;
        format[5] = 0xFF;
        format[6] = 0x01;
        format[7] = 0x02;
        using var input = new BinaryFormat(DataStreamFactory.FromArray(format));

        var deserializer = new Mock<NDeserializer<object>>();
        deserializer.Setup(x => x.Stamp).Returns("PLEO");
        deserializer.Setup(x => x.SupportedVersion).Returns(0x01_02)
            .Verifiable(Times.Once);

        deserializer.Object.Invoking(x => x.Convert(input))
            .Should().NotThrow();

        deserializer.Verify();
    }

    [Test]
    public void UnsupportedVersionThrowsException()
    {
        byte[] format = ValidHeader.ToArray();
        format[6] = 0x02;
        format[7] = 0x01;
        using var input = new BinaryFormat(DataStreamFactory.FromArray(format));

        var deserializer = new Mock<NDeserializer<object>>();
        deserializer.Setup(x => x.Stamp).Returns("PLEO");
        deserializer.Setup(x => x.SupportedVersion).Returns(0x01_00);

        deserializer.Object.Invoking(x => x.Convert(input))
            .Should().Throw<FormatException>()
            .WithMessage("Unknown version: 0102");
    }

    [Test]
    public void ReadSectionCalledForEachSection()
    {
        byte[] format = new byte[] {
            (byte)'H', (byte)'E', (byte)'A', (byte)'D', 0xFF, 0xFE, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x02, 0x00,
            (byte)'P', (byte)'L', (byte)'E', (byte)'O', 0x08, 0x00, 0x00, 0x00,
            (byte)'N', (byte)'E', (byte)'X', (byte)'U', 0x0C, 0x00, 0x00, 0x00,
            0xBE, 0xBA, 0xCA, 0xFE,
        };
        using var input = new BinaryFormat(DataStreamFactory.FromArray(format));

        var deserializer = new Mock<NDeserializer<object>>();
        deserializer.Setup(x => x.Stamp).Returns("HEAD");
        deserializer.Setup(x => x.SupportedVersion).Returns(0x01_00);
        deserializer.Protected()
            .Setup(
                "ReadSection",
                ItExpr.IsAny<DataReader>(),
                ItExpr.IsAny<object>(),
                "PLEO",
                8)
            .Verifiable(Times.Once);
        deserializer.Protected()
            .Setup(
                "ReadSection",
                ItExpr.IsAny<DataReader>(),
                ItExpr.IsAny<object>(),
                "NEXU",
                12)
            .Verifiable(Times.Once);

        _ = deserializer.Object.Convert(input);

        deserializer.Verify();
    }
}
